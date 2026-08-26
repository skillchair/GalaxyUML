import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { ApiService } from '../services/api';
import { CanvasTransform, ClassBoxItem, LineItem, PortSide, TextItem, ToolMode } from '../types';
import { useWorkspace } from './WorkspaceContext';

interface CanvasContextType {
  boardId: string | null;
  classBoxes: ClassBoxItem[];
  lines: LineItem[];
  texts: TextItem[];
  selectedIds: string[];
  activeTool: ToolMode;
  connectingStartBoxId: string | null;
  connectingStartPort: PortSide | null;
  hoveredTargetPort: { boxId: string; port: PortSide } | null;
  transform: CanvasTransform;
  isGridSnapEnabled: boolean;
  hoveredBoxId: string | null;
  isLoadingBoard: boolean;
  setHoveredTargetPort: (target: { boxId: string; port: PortSide } | null) => void;
  setActiveTool: (tool: ToolMode) => void;
  setTransform: (transform: CanvasTransform | ((prev: CanvasTransform) => CanvasTransform)) => void;
  zoomIn: () => void;
  zoomOut: () => void;
  resetTransform: () => void;
  zoomToFit: () => void;
  toggleGridSnap: () => void;
  setSelectedIds: (ids: string[] | ((prev: string[]) => string[])) => void;
  selectElement: (id: string, multi?: boolean) => void;
  clearSelection: () => void;
  setHoveredBoxId: (id: string | null) => void;
  loadBoardElements: () => Promise<void>;
  addClassBoxAt: (
    x: number,
    y: number,
    className?: string,
    attributes?: string[],
    methods?: string[]
  ) => Promise<string | null>;
  updateClassBoxContent: (
    id: string,
    newAttributes: string[],
    newMethods: string[]
  ) => Promise<void>;
  moveBox: (id: string, dx: number, dy: number, isFinal?: boolean) => Promise<void>;
  resizeBox: (id: string, width: number, height: number) => Promise<void>;
  startConnectingLine: (startBoxId: string, startPort?: PortSide | null) => void;
  finishConnectingLine: (
    endBoxId: string,
    endPort?: PortSide | null,
    middleText?: string
  ) => Promise<string | null>;
  cancelConnectingLine: () => void;
  deleteSelectedElements: () => Promise<void>;
  deleteElementById: (id: string) => Promise<void>;
  clearBoard: () => Promise<void>;
  snapToGrid: (val: number) => number;
}

const CanvasContext = createContext<CanvasContextType | undefined>(undefined);

const GRID_SIZE = 20;

export const CanvasProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { activeMeeting, onDiagramSignalREvent, canCurrentUserDraw } = useWorkspace();
  const [boardId, setBoardId] = useState<string | null>(null);
  const [classBoxes, setClassBoxes] = useState<ClassBoxItem[]>([]);
  const [lines, setLines] = useState<LineItem[]>([]);
  const [texts, setTexts] = useState<TextItem[]>([]);
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [activeTool, setActiveTool] = useState<ToolMode>('select');
  const [transform, setTransform] = useState<CanvasTransform>({ x: 120, y: 80, scale: 1 });
  const [isGridSnapEnabled, setIsGridSnapEnabled] = useState(true);
  const [connectingStartBoxId, setConnectingStartBoxId] = useState<string | null>(null);
  const [connectingStartPort, setConnectingStartPort] = useState<PortSide | null>(null);
  const [hoveredTargetPort, setHoveredTargetPort] = useState<{ boxId: string; port: PortSide } | null>(null);
  const [hoveredBoxId, setHoveredBoxId] = useState<string | null>(null);
  const [isLoadingBoard, setIsLoadingBoard] = useState(false);
  const pendingMovesRef = useRef<Map<string, { dx: number; dy: number }>>(new Map());
  const outgoingMoveEchoesRef = useRef<Map<string, Array<{ dx: number; dy: number; time: number }>>>(new Map());

  // Determine boardId from activeMeeting
  useEffect(() => {
    if (!activeMeeting) {
      setBoardId(null);
      setClassBoxes([]);
      setLines([]);
      setTexts([]);
      setSelectedIds([]);
      setConnectingStartBoxId(null);
      setConnectingStartPort(null);
      setHoveredTargetPort(null);
      return;
    }

    const bId = activeMeeting.boardId || (activeMeeting.board && activeMeeting.board.id);
    if (bId) {
      setBoardId(bId);
    }
  }, [activeMeeting]);

  const snapToGrid = useCallback(
    (val: number) => {
      if (!isGridSnapEnabled) return Math.round(val);
      return Math.round(val / GRID_SIZE) * GRID_SIZE;
    },
    [isGridSnapEnabled]
  );

  const loadBoardElements = useCallback(async () => {
    if (!boardId) return;
    setIsLoadingBoard(true);
    try {
      const data = await ApiService.getBoardElements(boardId);
      setClassBoxes(data.classBoxes || []);
      setLines(data.lines || []);
      setTexts(data.texts || []);
    } catch (err) {
      console.error('Failed to load board elements:', err);
    } finally {
      setIsLoadingBoard(false);
    }
  }, [boardId]);

  useEffect(() => {
    if (boardId) {
      loadBoardElements();
    }
  }, [boardId, loadBoardElements]);

  // Subscribe to real-time diagram events from SignalR
  useEffect(() => {
    const unsubscribe = onDiagramSignalREvent((type, data) => {
      if (type === 'ClassBoxAdded') {
        const item = data as {
          id: string;
          x1: number;
          y1: number;
          x2: number;
          y2: number;
          attributes: string[];
          methods: string[];
        };
        setClassBoxes((prev) => {
          if (prev.some((b) => b.id.toLowerCase() === item.id.toLowerCase())) return prev;
          return [...prev, item];
        });
      } else if (type === 'ElementMoved') {
        const { elementId, dx, dy } = data as { elementId: string; dx: number; dy: number };
        const key = elementId.toLowerCase();
        const echoes = outgoingMoveEchoesRef.current.get(key) || [];
        const now = Date.now();

        // Filter out stale echoes older than 8s
        const validEchoes = echoes.filter((e) => now - e.time < 8000);
        const matchIndex = validEchoes.findIndex((e) => e.dx === dx && e.dy === dy);

        if (matchIndex !== -1) {
          // Local move initiated by this client — do not reapply
          validEchoes.splice(matchIndex, 1);
          outgoingMoveEchoesRef.current.set(key, validEchoes);
          return;
        }
        outgoingMoveEchoesRef.current.set(key, validEchoes);

        // Move came from another participant
        setClassBoxes((prev) =>
          prev.map((b) => {
            if (b.id.toLowerCase() === key) {
              return {
                ...b,
                x1: b.x1 + dx,
                y1: b.y1 + dy,
                x2: b.x2 + dx,
                y2: b.y2 + dy,
              };
            }
            return b;
          })
        );
      } else if (type === 'LineAdded') {
        const item = data as {
          id: string;
          startBoxId: string;
          endBoxId: string;
          middleText: string | null;
          text1?: string | null;
          text2?: string | null;
        };
        setLines((prev) => {
          if (prev.some((l) => l.id.toLowerCase() === item.id.toLowerCase())) return prev;
          return [...prev, item];
        });
      } else if (type === 'ElementDeleted') {
        const { elementId } = data as { elementId: string };
        setClassBoxes((prev) => prev.filter((b) => b.id.toLowerCase() !== elementId.toLowerCase()));
        setLines((prev) =>
          prev.filter(
            (l) =>
              l.id.toLowerCase() !== elementId.toLowerCase() &&
              l.startBoxId.toLowerCase() !== elementId.toLowerCase() &&
              l.endBoxId.toLowerCase() !== elementId.toLowerCase()
          )
        );
        setTexts((prev) => prev.filter((t) => t.id.toLowerCase() !== elementId.toLowerCase()));
        setSelectedIds((prev) => prev.filter((id) => id.toLowerCase() !== elementId.toLowerCase()));
      } else if (type === 'BoardCleared') {
        setClassBoxes([]);
        setLines([]);
        setTexts([]);
        setSelectedIds([]);
      }
    });

    return unsubscribe;
  }, [onDiagramSignalREvent]);

  // Zoom & Pan Handlers
  const zoomIn = useCallback(() => {
    setTransform((prev) => ({
      ...prev,
      scale: Math.min(2.5, +(prev.scale + 0.15).toFixed(2)),
    }));
  }, []);

  const zoomOut = useCallback(() => {
    setTransform((prev) => ({
      ...prev,
      scale: Math.max(0.25, +(prev.scale - 0.15).toFixed(2)),
    }));
  }, []);

  const resetTransform = useCallback(() => {
    setTransform({ x: 120, y: 80, scale: 1 });
  }, []);

  const zoomToFit = useCallback(() => {
    if (classBoxes.length === 0) {
      resetTransform();
      return;
    }
    let minX = Infinity;
    let minY = Infinity;
    let maxX = -Infinity;
    let maxY = -Infinity;

    classBoxes.forEach((b) => {
      minX = Math.min(minX, b.x1);
      minY = Math.min(minY, b.y1);
      maxX = Math.max(maxX, b.x2);
      maxY = Math.max(maxY, b.y2);
    });

    const padding = 80;
    const width = maxX - minX + padding * 2;
    const height = maxY - minY + padding * 2;

    const vpWidth = window.innerWidth - 380;
    const vpHeight = window.innerHeight - 140;

    const scale = Math.min(1.5, Math.max(0.4, Math.min(vpWidth / width, vpHeight / height)));
    const x = Math.round((vpWidth - (maxX + minX) * scale) / 2);
    const y = Math.round((vpHeight - (maxY + minY) * scale) / 2);

    setTransform({ x, y, scale: +scale.toFixed(2) });
  }, [classBoxes, resetTransform]);

  const toggleGridSnap = useCallback(() => {
    setIsGridSnapEnabled((prev) => !prev);
  }, []);

  // Selection Handlers
  const selectElement = useCallback((id: string, multi = false) => {
    setSelectedIds((prev) => {
      if (multi) {
        return prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id];
      }
      return [id];
    });
  }, []);

  const clearSelection = useCallback(() => {
    setSelectedIds([]);
  }, []);

  // Diagram Operations
  const addClassBoxAt = useCallback(
    async (
      x: number,
      y: number,
      className = 'NewClass',
      attributes = ['+ id: int'],
      methods = ['+ execute(): void']
    ) => {
      if (!boardId || !canCurrentUserDraw) return null;
      const width = 200;
      const height = 130;
      const x1 = snapToGrid(x);
      const y1 = snapToGrid(y);
      const x2 = x1 + width;
      const y2 = y1 + height;

      try {
        const res = await ApiService.addClassBox(boardId, {
          x1,
          y1,
          x2,
          y2,
          attributes: [className, ...attributes],
          methods,
        });

        const newBox: ClassBoxItem = {
          id: res.id,
          x1,
          y1,
          x2,
          y2,
          attributes: [className, ...attributes],
          methods,
        };
        setClassBoxes((prev) => {
          if (prev.some((b) => b.id === res.id)) return prev;
          return [...prev, newBox];
        });
        setSelectedIds([res.id]);
        return res.id;
      } catch (err) {
        console.error('Add class box error:', err);
        throw err;
      }
    },
    [boardId, canCurrentUserDraw, snapToGrid]
  );

  const updateClassBoxContent = useCallback(
    async (id: string, newAttributes: string[], newMethods: string[]) => {
      if (!boardId || !canCurrentUserDraw) return;
      const targetBox = classBoxes.find((b) => b.id.toLowerCase() === id.toLowerCase());
      if (!targetBox) return;

      const dynamicHeight = Math.max(
        90,
        50 + Math.max(1, newAttributes.length - 1) * 20 + Math.max(1, newMethods.length) * 20 + 20
      );
      const updatedBox: ClassBoxItem = {
        ...targetBox,
        y2: targetBox.y1 + Math.max(targetBox.y2 - targetBox.y1, dynamicHeight),
        attributes: newAttributes,
        methods: newMethods,
      };

      setClassBoxes((prev) =>
        prev.map((b) => (b.id.toLowerCase() === id.toLowerCase() ? updatedBox : b))
      );

      const connectedLines = lines.filter(
        (l) =>
          l.startBoxId.toLowerCase() === id.toLowerCase() ||
          l.endBoxId.toLowerCase() === id.toLowerCase()
      );

      try {
        await ApiService.deleteElement(id);
        const res = await ApiService.addClassBox(boardId, {
          x1: updatedBox.x1,
          y1: updatedBox.y1,
          x2: updatedBox.x2,
          y2: updatedBox.y2,
          attributes: newAttributes,
          methods: newMethods,
        });

        const newId = res.id;
        for (const line of connectedLines) {
          const newStart =
            line.startBoxId.toLowerCase() === id.toLowerCase() ? newId : line.startBoxId;
          const newEnd =
            line.endBoxId.toLowerCase() === id.toLowerCase() ? newId : line.endBoxId;
          await ApiService.addLine(boardId, {
            startBoxId: newStart,
            endBoxId: newEnd,
            middleText: line.middleText,
            text1: line.text1,
            text2: line.text2,
          });
        }

        setClassBoxes((prev) =>
          prev.map((b) => (b.id.toLowerCase() === id.toLowerCase() ? { ...b, id: newId } : b))
        );
        setSelectedIds([newId]);
        await loadBoardElements();
      } catch (err) {
        console.error('Update class box error:', err);
        loadBoardElements();
      }
    },
    [boardId, canCurrentUserDraw, classBoxes, lines, loadBoardElements]
  );

  const moveBox = useCallback(
    async (id: string, dx: number, dy: number, isFinal = false) => {
      setClassBoxes((prev) =>
        prev.map((b) => {
          if (b.id.toLowerCase() === id.toLowerCase()) {
            return {
              ...b,
              x1: b.x1 + dx,
              y1: b.y1 + dy,
              x2: b.x2 + dx,
              y2: b.y2 + dy,
            };
          }
          return b;
        })
      );

      const current = pendingMovesRef.current.get(id) || { dx: 0, dy: 0 };
      current.dx += dx;
      current.dy += dy;
      pendingMovesRef.current.set(id, current);

      if (isFinal && canCurrentUserDraw) {
        const accumulated = pendingMovesRef.current.get(id);
        if (accumulated && (accumulated.dx !== 0 || accumulated.dy !== 0)) {
          pendingMovesRef.current.delete(id);
          const roundedDx = Math.round(accumulated.dx);
          const roundedDy = Math.round(accumulated.dy);

          const key = id.toLowerCase();
          const echoes = outgoingMoveEchoesRef.current.get(key) || [];
          echoes.push({ dx: roundedDx, dy: roundedDy, time: Date.now() });
          outgoingMoveEchoesRef.current.set(key, echoes);

          try {
            await ApiService.moveElement(id, roundedDx, roundedDy);
          } catch (err) {
            console.error('Move element error:', err);
            loadBoardElements();
          }
        }
      }
    },
    [canCurrentUserDraw, loadBoardElements]
  );

  const resizeBox = useCallback(
    async (id: string, width: number, height: number) => {
      if (!canCurrentUserDraw) return;
      const safeWidth = Math.max(140, snapToGrid(width));
      const safeHeight = Math.max(90, snapToGrid(height));

      setClassBoxes((prev) =>
        prev.map((b) => {
          if (b.id.toLowerCase() === id.toLowerCase()) {
            return {
              ...b,
              x2: b.x1 + safeWidth,
              y2: b.y1 + safeHeight,
            };
          }
          return b;
        })
      );

      try {
        await ApiService.resizeElement(id, safeWidth, safeHeight);
      } catch (err) {
        console.error('Resize error:', err);
        loadBoardElements();
      }
    },
    [canCurrentUserDraw, snapToGrid, loadBoardElements]
  );

  const startConnectingLine = useCallback(
    (startBoxId: string, startPort: PortSide | null = null) => {
      setConnectingStartBoxId(startBoxId);
      setConnectingStartPort(startPort);
      setHoveredTargetPort(null);
      setActiveTool('line');
    },
    []
  );

  const finishConnectingLine = useCallback(
    async (endBoxId: string, endPort: PortSide | null = null, middleText?: string) => {
      if (!boardId || !connectingStartBoxId || !canCurrentUserDraw) {
        setConnectingStartBoxId(null);
        setConnectingStartPort(null);
        setHoveredTargetPort(null);
        return null;
      }

      if (connectingStartBoxId.toLowerCase() === endBoxId.toLowerCase()) {
        setConnectingStartBoxId(null);
        setConnectingStartPort(null);
        setHoveredTargetPort(null);
        return null;
      }

      try {
        const res = await ApiService.addLine(boardId, {
          startBoxId: connectingStartBoxId,
          endBoxId,
          middleText: middleText || null,
          text1: connectingStartPort || null,
          text2: endPort || null,
        });

        const newLine: LineItem = {
          id: res.id,
          startBoxId: connectingStartBoxId,
          endBoxId,
          middleText: middleText || null,
          text1: connectingStartPort || null,
          text2: endPort || null,
        };

        setLines((prev) => [...prev, newLine]);
        setConnectingStartBoxId(null);
        setConnectingStartPort(null);
        setHoveredTargetPort(null);
        setActiveTool('select');
        return res.id;
      } catch (err) {
        console.error('Add line error:', err);
        setConnectingStartBoxId(null);
        setConnectingStartPort(null);
        setHoveredTargetPort(null);
        throw err;
      }
    },
    [boardId, connectingStartBoxId, connectingStartPort, canCurrentUserDraw]
  );

  const cancelConnectingLine = useCallback(() => {
    setConnectingStartBoxId(null);
    setConnectingStartPort(null);
    setHoveredTargetPort(null);
    setActiveTool('select');
  }, []);
  const deleteElementById = useCallback(
    async (id: string) => {
      if (!canCurrentUserDraw) return;
      try {
        await ApiService.deleteElement(id);
        setClassBoxes((prev) => prev.filter((b) => b.id.toLowerCase() !== id.toLowerCase()));
        setLines((prev) =>
          prev.filter(
            (l) =>
              l.id.toLowerCase() !== id.toLowerCase() &&
              l.startBoxId.toLowerCase() !== id.toLowerCase() &&
              l.endBoxId.toLowerCase() !== id.toLowerCase()
          )
        );
        setTexts((prev) => prev.filter((t) => t.id.toLowerCase() !== id.toLowerCase()));
        setSelectedIds((prev) => prev.filter((i) => i.toLowerCase() !== id.toLowerCase()));
      } catch (err) {
        console.error('Delete element error:', err);
        throw err;
      }
    },
    [canCurrentUserDraw]
  );

  const deleteSelectedElements = useCallback(async () => {
    if (!canCurrentUserDraw || selectedIds.length === 0) return;
    const idsToDelete = [...selectedIds];
    for (const id of idsToDelete) {
      try {
        await deleteElementById(id);
      } catch (err) {
        console.error(`Failed to delete element ${id}:`, err);
      }
    }
  }, [canCurrentUserDraw, selectedIds, deleteElementById]);

  const clearBoard = useCallback(async () => {
    if (!boardId || !canCurrentUserDraw) return;
    try {
      await ApiService.clearBoard(boardId);
      setClassBoxes([]);
      setLines([]);
      setTexts([]);
      setSelectedIds([]);
    } catch (err) {
      console.error('Clear board error:', err);
      throw err;
    }
  }, [boardId, canCurrentUserDraw]);

  return (
    <CanvasContext.Provider
      value={{
        boardId,
        classBoxes,
        lines,
        texts,
        selectedIds,
        activeTool,
        connectingStartBoxId,
        connectingStartPort,
        hoveredTargetPort,
        transform,
        isGridSnapEnabled,
        hoveredBoxId,
        isLoadingBoard,
        setHoveredTargetPort,
        setActiveTool,
        setTransform,
        zoomIn,
        zoomOut,
        resetTransform,
        zoomToFit,
        toggleGridSnap,
        setSelectedIds,
        selectElement,
        clearSelection,
        setHoveredBoxId,
        loadBoardElements,
        addClassBoxAt,
        updateClassBoxContent,
        moveBox,
        resizeBox,
        startConnectingLine,
        finishConnectingLine,
        cancelConnectingLine,
        deleteSelectedElements,
        deleteElementById,
        clearBoard,
        snapToGrid,
      }}
    >
      {children}
    </CanvasContext.Provider>
  );
};

export const useCanvas = (): CanvasContextType => {
  const context = useContext(CanvasContext);
  if (!context) {
    throw new Error('useCanvas must be used within a CanvasProvider');
  }
  return context;
};
