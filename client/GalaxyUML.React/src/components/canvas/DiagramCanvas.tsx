import React, { useState, useRef, useEffect, useCallback } from 'react';
import { useCanvas } from '../../context/CanvasContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { ClassBoxNode } from './ClassBoxNode';
import { RelationshipLine } from './RelationshipLine';
import { EditClassModal } from './EditClassModal';
import { ExportDiagramModal } from './ExportDiagramModal';
import { Button } from '../common/Button';
import { Tooltip } from '../common/Tooltip';
import {
  ZoomIn,
  ZoomOut,
  Maximize2,
  Grid,
  Download,
  Trash2,
  RefreshCw,
  Plus,
  MousePointer,
  SquareDashedBottomCode,
  GitCommitHorizontal,
  Info,
} from 'lucide-react';
import { ClassBoxItem } from '../../types';

export const DiagramCanvas: React.FC = () => {
  const {
    classBoxes,
    lines,
    selectedIds,
    activeTool,
    setActiveTool,
    transform,
    setTransform,
    zoomIn,
    zoomOut,
    zoomToFit,
    resetTransform,
    isGridSnapEnabled,
    toggleGridSnap,
    clearSelection,
    addClassBoxAt,
    clearBoard,
    loadBoardElements,
    isLoadingBoard,
    connectingStartBoxId,
    connectingStartPort,
    hoveredTargetPort,
    hoveredBoxId,
    cancelConnectingLine,
    snapToGrid,
  } = useCanvas();
  const { canCurrentUserDraw, activeMeeting } = useWorkspace();

  const containerRef = useRef<HTMLDivElement>(null);
  const [isPanning, setIsPanning] = useState(false);
  const panStartRef = useRef<{ x: number; y: number }>({ x: 0, y: 0 });
  const [editingClassBox, setEditingClassBox] = useState<ClassBoxItem | null>(null);
  const [isExportModalOpen, setIsExportModalOpen] = useState(false);

  // Mouse position in canvas coordinates for ghost line
  const [cursorCanvasPos, setCursorCanvasPos] = useState<{ x: number; y: number }>({ x: 0, y: 0 });
  const dragStartCanvasPosRef = useRef<{ x: number; y: number }>({ x: 0, y: 0 });
  const hasDraggedCanvasRef = useRef(false);

  // Spacebar panning state
  const isSpacePressedRef = useRef(false);
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Don't intercept when typing in inputs/modals
      if (
        e.target instanceof HTMLInputElement ||
        e.target instanceof HTMLTextAreaElement ||
        (e.target as HTMLElement).isContentEditable
      ) {
        return;
      }

      if (e.code === 'Space' && !isSpacePressedRef.current) {
        isSpacePressedRef.current = true;
        if (containerRef.current) containerRef.current.style.cursor = 'grab';
      } else if (e.key === 'v' || e.key === 'V') {
        setActiveTool('select');
      } else if (e.key === 'c' || e.key === 'C') {
        if (canCurrentUserDraw) setActiveTool('classBox');
      } else if (e.key === 'l' || e.key === 'L') {
        if (canCurrentUserDraw) setActiveTool('line');
      } else if (e.key === '0') {
        zoomToFit();
      } else if (e.key === 'Escape') {
        cancelConnectingLine();
        clearSelection();
        setActiveTool('select');
      }
    };

    const handleKeyUp = (e: KeyboardEvent) => {
      if (e.code === 'Space') {
        isSpacePressedRef.current = false;
        if (containerRef.current) containerRef.current.style.cursor = 'default';
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    window.addEventListener('keyup', handleKeyUp);
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
      window.removeEventListener('keyup', handleKeyUp);
    };
  }, [canCurrentUserDraw, setActiveTool, zoomToFit, cancelConnectingLine, clearSelection]);

  // Convert screen client coordinates to canvas SVG coordinates
  const screenToCanvas = useCallback(
    (clientX: number, clientY: number) => {
      if (!containerRef.current) return { x: 0, y: 0 };
      const rect = containerRef.current.getBoundingClientRect();
      const rawX = (clientX - rect.left - transform.x) / transform.scale;
      const rawY = (clientY - rect.top - transform.y) / transform.scale;
      return { x: rawX, y: rawY };
    },
    [transform]
  );

  // Wheel zoom handler
  const handleWheel = (e: React.WheelEvent) => {
    e.preventDefault();
    if (!containerRef.current) return;

    const zoomFactor = e.deltaY < 0 ? 1.1 : 0.9;
    const newScale = Math.min(2.5, Math.max(0.25, +(transform.scale * zoomFactor).toFixed(2)));

    const rect = containerRef.current.getBoundingClientRect();
    const mouseX = e.clientX - rect.left;
    const mouseY = e.clientY - rect.top;

    const newX = mouseX - (mouseX - transform.x) * (newScale / transform.scale);
    const newY = mouseY - (mouseY - transform.y) * (newScale / transform.scale);

    setTransform({
      x: Math.round(newX),
      y: Math.round(newY),
      scale: newScale,
    });
  };

  // Canvas pointer down handler
  const handlePointerDown = (e: React.PointerEvent) => {
    // Middle click or Spacebar + click or pan tool
    if (e.button === 1 || isSpacePressedRef.current || activeTool === 'pan') {
      setIsPanning(true);
      panStartRef.current = { x: e.clientX - transform.x, y: e.clientY - transform.y };
      if (containerRef.current) containerRef.current.style.cursor = 'grabbing';
      return;
    }

    if (e.button === 0) {
      // Ignore if clicking on interactive child elements that bubbled up
      const targetTag = (e.target as Element)?.tagName?.toLowerCase();
      const isCanvasBackground =
        e.target === containerRef.current ||
        targetTag === 'svg' ||
        (targetTag === 'rect' && (e.target as Element).getAttribute('fill')?.includes('dynamic-grid'));

      if (!isCanvasBackground) {
        return;
      }

      const { x, y } = screenToCanvas(e.clientX, e.clientY);
      dragStartCanvasPosRef.current = { x, y };
      hasDraggedCanvasRef.current = false;

      if (activeTool === 'classBox' && canCurrentUserDraw) {
        addClassBoxAt(x, y);
        setActiveTool('select');
        return;
      }

      if (connectingStartBoxId) {
        cancelConnectingLine();
      }

      clearSelection();
    }
  };

  const handlePointerMove = (e: React.PointerEvent) => {
    // Update cursor canvas pos for ghost line
    const { x, y } = screenToCanvas(e.clientX, e.clientY);
    setCursorCanvasPos({ x, y });

    if (
      Math.abs(x - dragStartCanvasPosRef.current.x) > 4 ||
      Math.abs(y - dragStartCanvasPosRef.current.y) > 4
    ) {
      hasDraggedCanvasRef.current = true;
    }

    if (isPanning) {
      setTransform((prev) => ({
        ...prev,
        x: e.clientX - panStartRef.current.x,
        y: e.clientY - panStartRef.current.y,
      }));
    }
  };

  const handlePointerUp = () => {
    if (isPanning) {
      setIsPanning(false);
      if (containerRef.current) {
        containerRef.current.style.cursor = isSpacePressedRef.current ? 'grab' : 'default';
      }
    }

    if (connectingStartBoxId && hasDraggedCanvasRef.current) {
      // Released after dragging on empty canvas
      cancelConnectingLine();
    }
  };

  // Get connecting start box edge port coordinates for ghost line
  const connectingBox = classBoxes.find(
    (b) => b.id.toLowerCase() === connectingStartBoxId?.toLowerCase()
  );
  const connectingStartPoint = connectingBox
    ? (() => {
        const w = Math.max(140, connectingBox.x2 - connectingBox.x1);
        const h = Math.max(90, connectingBox.y2 - connectingBox.y1);
        const ports: Record<string, { x: number; y: number }> = {
          top: { x: connectingBox.x1 + w / 2, y: connectingBox.y1 },
          right: { x: connectingBox.x1 + w, y: connectingBox.y1 + h / 2 },
          bottom: { x: connectingBox.x1 + w / 2, y: connectingBox.y1 + h },
          left: { x: connectingBox.x1, y: connectingBox.y1 + h / 2 },
        };
        if (connectingStartPort && ports[connectingStartPort]) {
          return ports[connectingStartPort];
        }
        let closest = ports.top;
        let minDist = Infinity;
        for (const p of Object.values(ports)) {
          const dist = (p.x - cursorCanvasPos.x) ** 2 + (p.y - cursorCanvasPos.y) ** 2;
          if (dist < minDist) {
            minDist = dist;
            closest = p;
          }
        }
        return closest;
      })()
    : null;

  // Get snapped target endpoint for ghost line
  const ghostEndPoint = (() => {
    if (
      hoveredTargetPort &&
      connectingStartBoxId &&
      hoveredTargetPort.boxId.toLowerCase() !== connectingStartBoxId.toLowerCase()
    ) {
      const targetBox = classBoxes.find(
        (b) => b.id.toLowerCase() === hoveredTargetPort.boxId.toLowerCase()
      );
      if (targetBox) {
        const w = Math.max(140, targetBox.x2 - targetBox.x1);
        const h = Math.max(90, targetBox.y2 - targetBox.y1);
        const ports: Record<string, { x: number; y: number }> = {
          top: { x: targetBox.x1 + w / 2, y: targetBox.y1 },
          right: { x: targetBox.x1 + w, y: targetBox.y1 + h / 2 },
          bottom: { x: targetBox.x1 + w / 2, y: targetBox.y1 + h },
          left: { x: targetBox.x1, y: targetBox.y1 + h / 2 },
        };
        if (ports[hoveredTargetPort.port]) {
          return { point: ports[hoveredTargetPort.port], isSnapped: true };
        }
      }
    }

    if (
      hoveredBoxId &&
      connectingStartBoxId &&
      hoveredBoxId.toLowerCase() !== connectingStartBoxId.toLowerCase()
    ) {
      const targetBox = classBoxes.find((b) => b.id.toLowerCase() === hoveredBoxId.toLowerCase());
      if (targetBox) {
        const w = Math.max(140, targetBox.x2 - targetBox.x1);
        const h = Math.max(90, targetBox.y2 - targetBox.y1);
        const ports: Record<string, { x: number; y: number }> = {
          top: { x: targetBox.x1 + w / 2, y: targetBox.y1 },
          right: { x: targetBox.x1 + w, y: targetBox.y1 + h / 2 },
          bottom: { x: targetBox.x1 + w / 2, y: targetBox.y1 + h },
          left: { x: targetBox.x1, y: targetBox.y1 + h / 2 },
        };
        let closest = ports.top;
        let minDist = Infinity;
        for (const p of Object.values(ports)) {
          const dist = (p.x - cursorCanvasPos.x) ** 2 + (p.y - cursorCanvasPos.y) ** 2;
          if (dist < minDist) {
            minDist = dist;
            closest = p;
          }
        }
        return { point: closest, isSnapped: true };
      }
    }

    return { point: cursorCanvasPos, isSnapped: false };
  })();
  // Adaptive Dynamic Grid Math (adapts to pan, zoom scale, and level of detail)
  const baseGridSize = 20;
  let effectiveGridStep = baseGridSize;
  if (transform.scale < 0.45) {
    effectiveGridStep = baseGridSize * 4; // 80px when zoomed far out
  } else if (transform.scale < 0.8) {
    effectiveGridStep = baseGridSize * 2; // 40px when zoomed out
  } else if (transform.scale > 2.0) {
    effectiveGridStep = baseGridSize / 2; // 10px subgrid when zoomed deep in
  }

  const patternSize = effectiveGridStep * transform.scale;
  const patternOffsetX = ((transform.x % patternSize) + patternSize) % patternSize;
  const patternOffsetY = ((transform.y % patternSize) + patternSize) % patternSize;
  const dotRadius = Math.max(0.8, Math.min(2.0, 1.1 * Math.sqrt(transform.scale)));

  return (
    <div
      ref={containerRef}
      onWheel={handleWheel}
      onPointerDown={handlePointerDown}
      onPointerMove={handlePointerMove}
      onPointerUp={handlePointerUp}
      className="relative flex-1 h-full w-full bg-slate-50 overflow-hidden select-none"
    >
      {/* SVG Canvas Board */}
      <svg
        id="galaxy-uml-canvas"
        className="w-full h-full absolute inset-0 touch-none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <defs>
          {/* Dynamic Adaptive Dot Matrix Pattern */}
          <pattern
            id="dynamic-grid-dots"
            x={patternOffsetX}
            y={patternOffsetY}
            width={patternSize}
            height={patternSize}
            patternUnits="userSpaceOnUse"
          >
            <circle
              cx={patternSize / 2}
              cy={patternSize / 2}
              r={dotRadius}
              fill="#94A3B8"
              opacity={Math.min(0.65, Math.max(0.25, 0.35 * Math.sqrt(transform.scale)))}
            />
          </pattern>

          {/* Arrowhead Marker */}
          <marker
            id="uml-arrow"
            viewBox="0 0 10 10"
            refX="8"
            refY="5"
            markerWidth="7"
            markerHeight="7"
            orient="auto-start-reverse"
          >
            <path d="M 0 1 L 10 5 L 0 9 z" fill="#0F172A" />
          </marker>
        </defs>

        {/* Adaptive Grid Background Rect covering full canvas */}
        <rect width="100%" height="100%" fill="url(#dynamic-grid-dots)" />

        {/* Viewport Transform Group */}
        <g transform={`translate(${transform.x}, ${transform.y}) scale(${transform.scale})`}>

          {/* 1. Relationship Lines */}
          {lines.map((line) => {
            const startBox = classBoxes.find(
              (b) => b.id.toLowerCase() === line.startBoxId.toLowerCase()
            );
            const endBox = classBoxes.find(
              (b) => b.id.toLowerCase() === line.endBoxId.toLowerCase()
            );
            const isSelected = selectedIds.includes(line.id);

            return (
              <RelationshipLine
                key={line.id}
                line={line}
                startBox={startBox}
                endBox={endBox}
                isSelected={isSelected}
              />
            );
          })}

          {/* Ghost Line while connecting */}
          {connectingStartPoint && (
            <g className="pointer-events-none">
              <line
                x1={connectingStartPoint.x}
                y1={connectingStartPoint.y}
                x2={ghostEndPoint.point.x}
                y2={ghostEndPoint.point.y}
                stroke="#2563EB"
                strokeWidth={ghostEndPoint.isSnapped ? 2.5 : 2}
                strokeDasharray={ghostEndPoint.isSnapped ? undefined : '5 4'}
                markerEnd="url(#uml-arrow)"
              />
              {ghostEndPoint.isSnapped && (
                <circle
                  cx={ghostEndPoint.point.x}
                  cy={ghostEndPoint.point.y}
                  r={6}
                  fill="#2563EB"
                  stroke="#FFFFFF"
                  strokeWidth={2}
                />
              )}
            </g>
          )}

          {/* 2. Class Box Nodes */}
          {classBoxes.map((box) => (
            <ClassBoxNode
              key={box.id}
              box={box}
              isSelected={selectedIds.includes(box.id)}
              isConnectingStart={connectingStartBoxId === box.id}
              onEdit={(b) => setEditingClassBox(b)}
            />
          ))}
        </g>
      </svg>

      {/* Connection Mode Banner in top-center */}
      {connectingStartBoxId && (
        <div className="absolute top-4 left-1/2 -translate-x-1/2 z-20 bg-slate-900 text-white px-3.5 py-1.5 rounded-lg shadow-lg border border-slate-700 text-xs flex items-center gap-2 animate-in fade-in-0 slide-in-from-top-2">
          <GitCommitHorizontal className="h-4 w-4 text-blue-400 animate-pulse" />
          <span>Kliknite ili prevucite na drugu klasu ili tačku povezivanja.</span>
          <button
            onClick={cancelConnectingLine}
            className="ml-2 px-1.5 py-0.5 bg-slate-800 hover:bg-slate-700 rounded text-[10px] font-mono text-slate-300 border border-slate-600 cursor-pointer"
          >
            Esc: Otkaži
          </button>
        </div>
      )}

      {/* Floating Canvas Controls HUD (Bottom Right) */}
      <div className="absolute bottom-4 right-4 z-20 flex items-center gap-1.5 p-1 bg-white/95 backdrop-blur-xs border border-slate-300 rounded-lg shadow-md">
        <Tooltip content="Uvećaj [+]">
          <button
            onClick={zoomIn}
            className="h-7 w-7 flex items-center justify-center rounded text-slate-700 hover:bg-slate-100 active:bg-slate-200"
          >
            <ZoomIn className="h-3.5 w-3.5" />
          </button>
        </Tooltip>

        <span className="w-10 text-center font-mono text-[11px] font-semibold text-slate-600">
          {Math.round(transform.scale * 100)}%
        </span>

        <Tooltip content="Umanji [-]">
          <button
            onClick={zoomOut}
            className="h-7 w-7 flex items-center justify-center rounded text-slate-700 hover:bg-slate-100 active:bg-slate-200"
          >
            <ZoomOut className="h-3.5 w-3.5" />
          </button>
        </Tooltip>

        <div className="h-4 w-px bg-slate-200 mx-0.5" />

        <Tooltip content="Prilagodi prikaz dijagramu [0]">
          <button
            onClick={zoomToFit}
            className="h-7 w-7 flex items-center justify-center rounded text-slate-700 hover:bg-slate-100 active:bg-slate-200"
          >
            <Maximize2 className="h-3.5 w-3.5" />
          </button>
        </Tooltip>

        <Tooltip content={isGridSnapEnabled ? 'Isključi magnetnu mrežu' : 'Uključi magnetnu mrežu'}>
          <button
            onClick={toggleGridSnap}
            className={`h-7 w-7 flex items-center justify-center rounded transition-colors ${
              isGridSnapEnabled
                ? 'bg-blue-50 text-blue-700 border border-blue-200 font-bold'
                : 'text-slate-400 hover:bg-slate-100'
            }`}
          >
            <Grid className="h-3.5 w-3.5" />
          </button>
        </Tooltip>

        <div className="h-4 w-px bg-slate-200 mx-0.5" />

        <Tooltip content="Izvezi dijagram (SVG / PNG)">
          <button
            onClick={() => setIsExportModalOpen(true)}
            className="h-7 px-2 flex items-center gap-1 rounded text-slate-700 hover:bg-slate-100 active:bg-slate-200 text-xs font-semibold"
          >
            <Download className="h-3.5 w-3.5 text-blue-600" />
            <span>Izvoz</span>
          </button>
        </Tooltip>

        {canCurrentUserDraw && (
          <Tooltip content="Očisti celu tablu">
            <button
              onClick={() => {
                if (confirm('Da li ste sigurni da želite da obrišete sve elemente sa table?')) {
                  clearBoard();
                }
              }}
              className="h-7 w-7 flex items-center justify-center rounded text-rose-600 hover:bg-rose-50 active:bg-rose-100"
            >
              <Trash2 className="h-3.5 w-3.5" />
            </button>
          </Tooltip>
        )}
      </div>

      {/* Floating Canvas Quick Add Bar (Top Left of canvas) */}
      {canCurrentUserDraw && (
        <div className="absolute top-4 left-4 z-20 flex items-center gap-1.5 p-1 bg-white/95 backdrop-blur-xs border border-slate-300 rounded-lg shadow-sm">
          <Button
            variant={activeTool === 'classBox' ? 'primary' : 'secondary'}
            size="sm"
            onClick={() => {
              // Add centered class box
              if (containerRef.current) {
                const rect = containerRef.current.getBoundingClientRect();
                const center = screenToCanvas(rect.left + rect.width / 2, rect.top + rect.height / 2);
                addClassBoxAt(center.x - 100, center.y - 65);
              }
            }}
            icon={<Plus className="h-3.5 w-3.5" />}
          >
            Nova Klasa
          </Button>

          <Button
            variant="ghost"
            size="sm"
            onClick={loadBoardElements}
            className="h-7 w-7 p-0"
            title="Osveži elemente sa servera"
          >
            <RefreshCw className={`h-3.5 w-3.5 text-slate-500 ${isLoadingBoard ? 'animate-spin' : ''}`} />
          </Button>
        </div>
      )}

      {/* Minimap / Radar Viewport (Bottom Left) */}
      {classBoxes.length > 0 && (
        <div className="absolute bottom-4 left-4 z-20 w-36 h-24 bg-white/95 backdrop-blur-xs border border-slate-300 rounded-md shadow-md p-1 overflow-hidden pointer-events-none hidden sm:block">
          <div className="w-full h-full bg-slate-50 border border-slate-200 rounded relative">
            {classBoxes.map((b) => (
              <div
                key={b.id}
                className="absolute bg-slate-800 rounded-2xs opacity-75"
                style={{
                  left: `${Math.max(0, Math.min(90, (b.x1 / 2000) * 100))}%`,
                  top: `${Math.max(0, Math.min(90, (b.y1 / 1400) * 100))}%`,
                  width: '12px',
                  height: '8px',
                }}
              />
            ))}
          </div>
        </div>
      )}

      {/* Modals */}
      <EditClassModal
        isOpen={!!editingClassBox}
        classBox={editingClassBox}
        onClose={() => setEditingClassBox(null)}
      />

      <ExportDiagramModal
        isOpen={isExportModalOpen}
        onClose={() => setIsExportModalOpen(false)}
      />
    </div>
  );
};
