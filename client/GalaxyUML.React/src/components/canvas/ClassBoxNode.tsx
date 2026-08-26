import React, { useState, useRef, useEffect } from 'react';
import { useCanvas } from '../../context/CanvasContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { ClassBoxItem, PortSide } from '../../types';
import { Edit3, GitCommitHorizontal, Trash2, Plus, X } from 'lucide-react';

export interface ClassBoxNodeProps {
  box: ClassBoxItem;
  isSelected: boolean;
  isConnectingStart: boolean;
  onEdit: (box: ClassBoxItem) => void;
}

export function parseClassBoxData(rawAttributes: string[] = [], rawMethods: string[] = []) {
  if (!rawAttributes || rawAttributes.length === 0) {
    return {
      title: 'NewClass',
      attributesList: [] as string[],
      methodsList: (rawMethods || []).filter((m) => Boolean(m && m.trim())),
    };
  }

  // Find class title (first item without +, -, #, ~ prefix and without colon)
  let titleIndex = -1;
  for (let i = 0; i < rawAttributes.length; i++) {
    const item = rawAttributes[i].trim();
    if (item && !/^([+\-#~]|public|private|protected)\s+/i.test(item) && !item.includes(':')) {
      titleIndex = i;
      break;
    }
  }

  if (titleIndex === -1) {
    titleIndex = 0;
  }

  const title = rawAttributes[titleIndex].trim() || 'NewClass';
  const attributesList: string[] = [];

  for (let i = 0; i < rawAttributes.length; i++) {
    if (i !== titleIndex) {
      const item = rawAttributes[i].trim();
      if (item) {
        attributesList.push(item);
      }
    }
  }

  return {
    title,
    attributesList,
    methodsList: (rawMethods || []).filter((m) => Boolean(m && m.trim())),
  };
}

export const ClassBoxNode: React.FC<ClassBoxNodeProps> = ({
  box,
  isSelected,
  isConnectingStart,
  onEdit,
}) => {
  const {
    selectElement,
    moveBox,
    resizeBox,
    startConnectingLine,
    finishConnectingLine,
    connectingStartBoxId,
    connectingStartPort,
    hoveredTargetPort,
    setHoveredTargetPort,
    hoveredBoxId,
    setHoveredBoxId,
    activeTool,
    deleteElementById,
    updateClassBoxContent,
    transform,
    snapToGrid,
  } = useCanvas();
  const { canCurrentUserDraw } = useWorkspace();

  const [isHovered, setIsHovered] = useState(false);
  const nodeRef = useRef<SVGGElement>(null);
  const hoverTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  const isDraggingRef = useRef(false);
  const hasMovedRef = useRef(false);
  const dragStartPosRef = useRef<{ x: number; y: number }>({ x: 0, y: 0 });
  const justFinishedRef = useRef<number>(0);
  const justStartedRef = useRef<number>(0);

  const isResizingRef = useRef(false);
  const resizeStartDimRef = useRef<{ width: number; height: number; mouseX: number; mouseY: number }>({
    width: 0,
    height: 0,
    mouseX: 0,
    mouseY: 0,
  });
  // Inline editing states (activated on double-click)
  const [isEditingTitle, setIsEditingTitle] = useState(false);
  const [editingTitleText, setEditingTitleText] = useState('');

  const [editingAttrIndex, setEditingAttrIndex] = useState<number | null>(null);
  const [editingAttrText, setEditingAttrText] = useState('');

  const [editingMethodIndex, setEditingMethodIndex] = useState<number | null>(null);
  const [editingMethodText, setEditingMethodText] = useState('');

  // Robust parsing of title, attributes, methods
  const { title, attributesList, methodsList } = parseClassBoxData(box.attributes, box.methods);

  // Keep editing draft text in sync when editing mode starts
  const startEditTitle = () => {
    if (!canCurrentUserDraw) return;
    setEditingTitleText(title);
    setIsEditingTitle(true);
  };

  const startEditAttr = (idx: number, currentText: string) => {
    if (!canCurrentUserDraw) return;
    setEditingAttrText(currentText);
    setEditingAttrIndex(idx);
  };

  const startEditMethod = (idx: number, currentText: string) => {
    if (!canCurrentUserDraw) return;
    setEditingMethodText(currentText);
    setEditingMethodIndex(idx);
  };

  // Tight, compact dimensions with clean vertical alignment
  const attrLines = Math.max(1, attributesList.length);
  const methodLines = Math.max(1, methodsList.length);
  const attrSectionHeight = attrLines * 18 + 8;
  const methodSectionHeight = methodLines * 18 + 8;
  const divider1Y = 26 + attrSectionHeight;
  const contentHeightNeeded = divider1Y + methodSectionHeight;

  const width = Math.max(140, box.x2 - box.x1);
  const height = Math.max(contentHeightNeeded, box.y2 - box.y1);

  // Calculate closest port side relative to this ClassBox
  const getClosestPort = (clientX: number, clientY: number): PortSide => {
    const elem = nodeRef.current;
    if (!elem) return 'top';
    const rect = elem.getBoundingClientRect();
    const relX = (clientX - rect.left) / (transform.scale || 1);
    const relY = (clientY - rect.top) / (transform.scale || 1);

    const portPositions: Record<PortSide, { x: number; y: number }> = {
      top: { x: width / 2, y: 0 },
      right: { x: width, y: height / 2 },
      bottom: { x: width / 2, y: height },
      left: { x: 0, y: height / 2 },
    };

    let closest: PortSide = 'top';
    let minDist = Infinity;

    for (const [side, pos] of Object.entries(portPositions) as Array<[PortSide, { x: number; y: number }]>) {
      const dist = (pos.x - relX) ** 2 + (pos.y - relY) ** 2;
      if (dist < minDist) {
        minDist = dist;
        closest = side;
      }
    }

    return closest;
  };

  // Graceful hover enter and leave with debounce timer
  const handlePointerEnter = () => {
    if (hoverTimeoutRef.current) {
      clearTimeout(hoverTimeoutRef.current);
      hoverTimeoutRef.current = null;
    }
    setIsHovered(true);
    if (connectingStartBoxId && connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
      setHoveredBoxId(box.id);
    }
  };

  const handlePointerLeave = () => {
    if (hoverTimeoutRef.current) {
      clearTimeout(hoverTimeoutRef.current);
    }
    hoverTimeoutRef.current = setTimeout(() => {
      setIsHovered(false);
      hoverTimeoutRef.current = null;
    }, 300);

    if (hoveredBoxId && hoveredBoxId.toLowerCase() === box.id.toLowerCase()) {
      setHoveredBoxId(null);
    }
  };

  useEffect(() => {
    return () => {
      if (hoverTimeoutRef.current) {
        clearTimeout(hoverTimeoutRef.current);
      }
    };
  }, []);

  // Dragging & Interaction handlers
  const handlePointerDown = (e: React.PointerEvent) => {
    if (e.button !== 0) return;
    e.stopPropagation();

    // If currently in line connecting mode, clicking/pressing on the box connects to it
    if (connectingStartBoxId) {
      if (connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
        const port = getClosestPort(e.clientX, e.clientY);
        justFinishedRef.current = Date.now();
        finishConnectingLine(box.id, port);
      }
      return;
    }

    if (activeTool === 'line') {
      const port = getClosestPort(e.clientX, e.clientY);
      justStartedRef.current = Date.now();
      startConnectingLine(box.id, port);
      return;
    }
    selectElement(box.id, e.shiftKey || e.ctrlKey);

    if (!canCurrentUserDraw || isEditingTitle || editingAttrIndex !== null || editingMethodIndex !== null) {
      return;
    }

    isDraggingRef.current = true;
    hasMovedRef.current = false;
    dragStartPosRef.current = { x: e.clientX, y: e.clientY };
  };
  const handlePointerMove = (e: React.PointerEvent) => {
    if (isDraggingRef.current) {
      e.stopPropagation();
      const dxRaw = (e.clientX - dragStartPosRef.current.x) / transform.scale;
      const dyRaw = (e.clientY - dragStartPosRef.current.y) / transform.scale;

      if (!hasMovedRef.current && (Math.abs(dxRaw) >= 3 || Math.abs(dyRaw) >= 3)) {
        hasMovedRef.current = true;
        try {
          e.currentTarget.setPointerCapture(e.pointerId);
        } catch {
          // ignore
        }
      }

      if (hasMovedRef.current) {
        moveBox(box.id, dxRaw, dyRaw, false);
        dragStartPosRef.current = { x: e.clientX, y: e.clientY };
      }
    } else if (isResizingRef.current) {
      e.stopPropagation();
      const deltaX = (e.clientX - resizeStartDimRef.current.mouseX) / transform.scale;
      const deltaY = (e.clientY - resizeStartDimRef.current.mouseY) / transform.scale;

      const newWidth = Math.max(140, resizeStartDimRef.current.width + deltaX);
      const newHeight = Math.max(contentHeightNeeded, resizeStartDimRef.current.height + deltaY);

      resizeBox(box.id, newWidth, newHeight);
    }
  };

  const handlePointerUp = (e: React.PointerEvent) => {
    // If wiring from another box and released over this box, finish connecting
    if (connectingStartBoxId && connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
      e.stopPropagation();
      const port = getClosestPort(e.clientX, e.clientY);
      justFinishedRef.current = Date.now();
      finishConnectingLine(box.id, port);
      return;
    }

    if (isDraggingRef.current) {
      e.stopPropagation();
      isDraggingRef.current = false;
      try {
        e.currentTarget.releasePointerCapture(e.pointerId);
      } catch {
        // ignore
      }
      if (hasMovedRef.current) {
        const currentX = box.x1;
        const currentY = box.y1;
        const snappedX = snapToGrid(currentX);
        const snappedY = snapToGrid(currentY);
        const correctionX = snappedX - currentX;
        const correctionY = snappedY - currentY;
        moveBox(box.id, correctionX, correctionY, true);
      }
      hasMovedRef.current = false;
    }

    if (isResizingRef.current) {
      e.stopPropagation();
      isResizingRef.current = false;
    }
  };

  const handleResizePointerDown = (e: React.PointerEvent) => {
    if (e.button !== 0 || !canCurrentUserDraw) return;
    e.stopPropagation();
    isResizingRef.current = true;
    resizeStartDimRef.current = {
      width,
      height,
      mouseX: e.clientX,
      mouseY: e.clientY,
    };
    (e.target as HTMLElement).setPointerCapture(e.pointerId);
  };

  // Anchor Port Click/Pointer handlers are attached directly to ports below

  // Inline Title Save
  const handleSaveTitle = (overrideVal?: string) => {
    const rawVal = overrideVal !== undefined ? overrideVal : editingTitleText;
    const finalVal = rawVal.trim();
    setIsEditingTitle(false);
    if (!finalVal || finalVal === title) {
      return;
    }
    const newAttrs = [finalVal, ...attributesList];
    updateClassBoxContent(box.id, newAttrs, methodsList);
  };

  // Inline Attribute Save
  const handleSaveAttribute = (idx: number, overrideVal?: string) => {
    const rawVal = overrideVal !== undefined ? overrideVal : editingAttrText;
    const finalVal = rawVal.trim();
    setEditingAttrIndex(null);

    if (!finalVal) {
      const updatedList = attributesList.filter((_, i) => i !== idx);
      updateClassBoxContent(box.id, [title, ...updatedList], methodsList);
    } else {
      const updatedList = [...attributesList];
      updatedList[idx] = finalVal;
      updateClassBoxContent(box.id, [title, ...updatedList], methodsList);
    }
  };

  // Add New Attribute
  const handleAddAttribute = () => {
    const updatedList = [...attributesList, '+ attr: string'];
    updateClassBoxContent(box.id, [title, ...updatedList], methodsList);
    setEditingAttrIndex(updatedList.length - 1);
    setEditingAttrText('+ attr: string');
  };

  // Delete Attribute
  const handleDeleteAttribute = (idx: number) => {
    const updatedList = attributesList.filter((_, i) => i !== idx);
    updateClassBoxContent(box.id, [title, ...updatedList], methodsList);
  };

  // Inline Method Save
  const handleSaveMethod = (idx: number, overrideVal?: string) => {
    const rawVal = overrideVal !== undefined ? overrideVal : editingMethodText;
    const finalVal = rawVal.trim();
    setEditingMethodIndex(null);

    if (!finalVal) {
      const updatedList = methodsList.filter((_, i) => i !== idx);
      updateClassBoxContent(box.id, [title, ...attributesList], updatedList);
    } else {
      const updatedList = [...methodsList];
      updatedList[idx] = finalVal;
      updateClassBoxContent(box.id, [title, ...attributesList], updatedList);
    }
  };

  // Add New Method
  const handleAddMethod = () => {
    const updatedList = [...methodsList, '+ method(): void'];
    updateClassBoxContent(box.id, [title, ...attributesList], updatedList);
    setEditingMethodIndex(updatedList.length - 1);
    setEditingMethodText('+ method(): void');
  };

  // Delete Method
  const handleDeleteMethod = (idx: number) => {
    const updatedList = methodsList.filter((_, i) => i !== idx);
    updateClassBoxContent(box.id, [title, ...attributesList], updatedList);
  };

  return (
    <g
      ref={nodeRef}
      transform={`translate(${box.x1}, ${box.y1})`}
      onPointerEnter={handlePointerEnter}
      onPointerLeave={handlePointerLeave}
      className="cursor-move select-none"
    >
      {/* Invisible Hover Zone Bridge: covers area above class box, pill, and borders */}
      <rect
        x={-14}
        y={-38}
        width={width + 28}
        height={height + 46}
        fill="#FFFFFF"
        opacity={0}
        pointerEvents="all"
      />
      {/* Selection Outer Border */}
      {isSelected && (
        <rect
          x={-3}
          y={-3}
          width={width + 6}
          height={height + 6}
          rx={3}
          fill="none"
          stroke="#2563EB"
          strokeWidth={1.5}
          strokeDasharray="3 2"
          className="pointer-events-none"
        />
      )}

      {/* Main Class Box Card */}
      <g
        onPointerDown={handlePointerDown}
        onPointerMove={handlePointerMove}
        onPointerUp={handlePointerUp}
      >
        {/* Background Card */}
        <rect
          x={0}
          y={0}
          width={width}
          height={height}
          fill="#FFFFFF"
          stroke={isSelected ? '#2563EB' : isHovered ? '#475569' : '#0F172A'}
          strokeWidth={isSelected ? 1.5 : 1}
          rx={2}
          className="shadow-2xs"
        />

        {/* 1. Header Compartment (Class Name) */}
        <rect
          x={0}
          y={0}
          width={width}
          height={26}
          fill={isSelected ? '#F0F7FF' : '#F8FAFC'}
          stroke={isSelected ? '#2563EB' : '#0F172A'}
          strokeWidth={1}
        />

        {/* Title: Double-click to edit inline */}
        {isEditingTitle && canCurrentUserDraw ? (
          <foreignObject x={2} y={1} width={width - 4} height={24} className="overflow-visible">
            <input
              type="text"
              autoFocus
              value={editingTitleText}
              onChange={(e) => setEditingTitleText(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') handleSaveTitle(e.currentTarget.value);
                if (e.key === 'Escape') setIsEditingTitle(false);
              }}
              onBlur={(e) => handleSaveTitle(e.target.value)}
              className="w-full h-6 px-1 text-center font-bold text-xs bg-white border border-blue-600 rounded text-slate-900 shadow-2xs focus:outline-none"
            />
          </foreignObject>
        ) : (
          <text
            x={width / 2}
            y={17}
            textAnchor="middle"
            fill="#0F172A"
            fontSize={12}
            fontWeight="bold"
            fontFamily="Inter, sans-serif"
            className={canCurrentUserDraw ? 'cursor-text' : ''}
            onDoubleClick={(e) => {
              e.stopPropagation();
              startEditTitle();
            }}
          >
            {title}
          </text>
        )}

        {/* 2. Attributes Compartment */}
        <g transform="translate(6, 30)">
          {attributesList.length === 0 ? (
            <text fill="#94A3B8" fontSize={10} y={13} fontStyle="italic" fontFamily="JetBrains Mono, monospace">
              // no attributes
            </text>
          ) : (
            attributesList.map((attr, idx) => (
              <g key={idx} transform={`translate(0, ${idx * 18})`}>
                {editingAttrIndex === idx && canCurrentUserDraw ? (
                  <foreignObject x={-2} y={0} width={width - 12} height={18} className="overflow-visible">
                    <input
                      type="text"
                      autoFocus
                      value={editingAttrText}
                      onChange={(e) => setEditingAttrText(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') handleSaveAttribute(idx, e.currentTarget.value);
                        if (e.key === 'Escape') setEditingAttrIndex(null);
                      }}
                      onBlur={(e) => handleSaveAttribute(idx, e.target.value)}
                      className="w-full h-[18px] px-1 font-mono text-[10px] bg-white border border-blue-600 rounded text-slate-900 focus:outline-none shadow-2xs leading-none"
                    />
                  </foreignObject>
                ) : (
                  <g
                    className={canCurrentUserDraw ? 'cursor-text group/attr' : ''}
                    onDoubleClick={(e) => {
                      if (canCurrentUserDraw) {
                        e.stopPropagation();
                        startEditAttr(idx, attr);
                      }
                    }}
                  >
                    <text
                      x={0}
                      y={13}
                      fill="#334155"
                      fontSize={11}
                      fontFamily="JetBrains Mono, monospace"
                      className="code-token"
                    >
                      {attr}
                    </text>

                    {isSelected && canCurrentUserDraw && (
                      <foreignObject
                        x={width - 24}
                        y={1}
                        width={16}
                        height={16}
                        className="overflow-visible opacity-0 group-hover/attr:opacity-100 transition-opacity"
                      >
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDeleteAttribute(idx);
                          }}
                          className="h-4 w-4 rounded text-slate-400 hover:text-rose-600 flex items-center justify-center"
                          title="Ukloni atribut"
                        >
                          <X className="h-3 w-3" />
                        </button>
                      </foreignObject>
                    )}
                  </g>
                )}
              </g>
            ))
          )}
        </g>

        {/* Minimalist Side '+' for Attributes */}
        {(isSelected || isHovered) && canCurrentUserDraw && (
          <foreignObject x={width - 18} y={28} width={16} height={16} className="overflow-visible">
            <button
              onClick={(e) => {
                e.stopPropagation();
                handleAddAttribute();
              }}
              className="h-4 w-4 bg-slate-100 hover:bg-blue-50 text-slate-500 hover:text-blue-600 rounded flex items-center justify-center border border-slate-200 shadow-2xs"
              title="Dodaj atribut (+)"
            >
              <Plus className="h-2.5 w-2.5" />
            </button>
          </foreignObject>
        )}

        {/* Compartment Divider Line */}
        <line
          x1={0}
          y1={divider1Y}
          x2={width}
          y2={divider1Y}
          stroke="#E2E8F0"
          strokeWidth={1}
        />

        {/* 3. Methods Compartment */}
        <g transform={`translate(6, ${divider1Y + 4})`}>
          {methodsList.length === 0 ? (
            <text fill="#94A3B8" fontSize={10} y={13} fontStyle="italic" fontFamily="JetBrains Mono, monospace">
              // no methods
            </text>
          ) : (
            methodsList.map((method, idx) => (
              <g key={idx} transform={`translate(0, ${idx * 18})`}>
                {editingMethodIndex === idx && canCurrentUserDraw ? (
                  <foreignObject x={-2} y={0} width={width - 12} height={18} className="overflow-visible">
                    <input
                      type="text"
                      autoFocus
                      value={editingMethodText}
                      onChange={(e) => setEditingMethodText(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') handleSaveMethod(idx, e.currentTarget.value);
                        if (e.key === 'Escape') setEditingMethodIndex(null);
                      }}
                      onBlur={(e) => handleSaveMethod(idx, e.target.value)}
                      className="w-full h-[18px] px-1 font-mono text-[10px] bg-white border border-blue-600 rounded text-slate-900 focus:outline-none shadow-2xs leading-none"
                    />
                  </foreignObject>
                ) : (
                  <g
                    className={canCurrentUserDraw ? 'cursor-text group/meth' : ''}
                    onDoubleClick={(e) => {
                      if (canCurrentUserDraw) {
                        e.stopPropagation();
                        startEditMethod(idx, method);
                      }
                    }}
                  >
                    <text
                      x={0}
                      y={13}
                      fill="#1E293B"
                      fontSize={11}
                      fontFamily="JetBrains Mono, monospace"
                      className="code-token"
                    >
                      {method}
                    </text>

                    {isSelected && canCurrentUserDraw && (
                      <foreignObject
                        x={width - 24}
                        y={1}
                        width={16}
                        height={16}
                        className="overflow-visible opacity-0 group-hover/meth:opacity-100 transition-opacity"
                      >
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDeleteMethod(idx);
                          }}
                          className="h-4 w-4 rounded text-slate-400 hover:text-rose-600 flex items-center justify-center"
                          title="Ukloni metodu"
                        >
                          <X className="h-3 w-3" />
                        </button>
                      </foreignObject>
                    )}
                  </g>
                )}
              </g>
            ))
          )}
        </g>

        {/* Minimalist Side '+' for Methods */}
        {(isSelected || isHovered) && canCurrentUserDraw && (
          <foreignObject x={width - 18} y={divider1Y + 2} width={16} height={16} className="overflow-visible">
            <button
              onClick={(e) => {
                e.stopPropagation();
                handleAddMethod();
              }}
              className="h-4 w-4 bg-slate-100 hover:bg-blue-50 text-slate-500 hover:text-blue-600 rounded flex items-center justify-center border border-slate-200 shadow-2xs"
              title="Dodaj metodu (+)"
            >
              <Plus className="h-2.5 w-2.5" />
            </button>
          </foreignObject>
        )}
      </g>

      {/* Connection Anchor Ports (North, East, South, West) */}
      {(isHovered || isSelected || activeTool === 'line' || Boolean(connectingStartBoxId)) && canCurrentUserDraw && (
        <g>
          {(
            [
              { side: 'top' as PortSide, x: width / 2, y: 0 },
              { side: 'right' as PortSide, x: width, y: height / 2 },
              { side: 'bottom' as PortSide, x: width / 2, y: height },
              { side: 'left' as PortSide, x: 0, y: height / 2 },
            ]
          ).map(({ side, x, y }) => {
            const isThisStartPort = isConnectingStart && connectingStartPort === side;
            const isThisTargetHovered =
              hoveredTargetPort?.boxId.toLowerCase() === box.id.toLowerCase() &&
              hoveredTargetPort?.port === side;

            return (
              <g
                key={side}
                transform={`translate(${x}, ${y})`}
                onPointerDown={(e) => {
                  if (e.button !== 0 || !canCurrentUserDraw) return;
                  e.stopPropagation();
                  if (connectingStartBoxId) {
                    if (connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
                      justFinishedRef.current = Date.now();
                      finishConnectingLine(box.id, side);
                    }
                  } else {
                    justStartedRef.current = Date.now();
                    startConnectingLine(box.id, side);
                  }
                }}
                onPointerEnter={() => {
                  if (connectingStartBoxId && connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
                    setHoveredTargetPort({ boxId: box.id, port: side });
                  }
                }}
                onPointerLeave={() => {
                  if (
                    hoveredTargetPort?.boxId.toLowerCase() === box.id.toLowerCase() &&
                    hoveredTargetPort?.port === side
                  ) {
                    setHoveredTargetPort(null);
                  }
                }}
                onPointerUp={(e) => {
                  if (e.button !== 0 || !canCurrentUserDraw) return;
                  e.stopPropagation();
                  if (connectingStartBoxId && connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
                    justFinishedRef.current = Date.now();
                    finishConnectingLine(box.id, side);
                  }
                }}
                onClick={(e) => {
                  e.stopPropagation();
                  if (!canCurrentUserDraw) return;
                  if (Date.now() - justFinishedRef.current < 350 || Date.now() - justStartedRef.current < 350) {
                    return;
                  }
                  if (connectingStartBoxId) {
                    if (connectingStartBoxId.toLowerCase() !== box.id.toLowerCase()) {
                      justFinishedRef.current = Date.now();
                      finishConnectingLine(box.id, side);
                    }
                  } else {
                    justStartedRef.current = Date.now();
                    startConnectingLine(box.id, side);
                  }
                }}
                className="cursor-crosshair group/port"
              >
                {/* Generous invisible hit target circle */}
                <circle cx={0} cy={0} r={16} fill="transparent" />

                {/* Target hover pulsing glow indicator */}
                {isThisTargetHovered && (
                  <circle
                    cx={0}
                    cy={0}
                    r={9}
                    fill="none"
                    stroke="#2563EB"
                    strokeWidth={2}
                    opacity={0.6}
                    className="animate-ping"
                  />
                )}

                {/* Visible Port Circle */}
                <circle
                  cx={0}
                  cy={0}
                  r={isThisTargetHovered ? 6 : isThisStartPort ? 5.5 : 4.5}
                  fill={isThisTargetHovered || isThisStartPort ? '#2563EB' : '#FFFFFF'}
                  stroke={isThisTargetHovered ? '#1D4ED8' : '#2563EB'}
                  strokeWidth={isThisTargetHovered ? 2.5 : 1.75}
                  className="transition-all duration-150 group-hover/port:stroke-blue-700 group-hover/port:fill-blue-100"
                />
              </g>
            );
          })}
        </g>
      )}

      {/* Resize Handle (Bottom-Right Corner) */}
      {isSelected && canCurrentUserDraw && (
        <g
          transform={`translate(${width - 8}, ${height - 8})`}
          onPointerDown={handleResizePointerDown}
          onPointerMove={handlePointerMove}
          onPointerUp={handlePointerUp}
          className="cursor-nwse-resize"
        >
          <rect x={0} y={0} width={8} height={8} fill="#2563EB" rx={1} />
          <path d="M2 6L6 2M4 6L6 4" stroke="#FFFFFF" strokeWidth={0.75} />
        </g>
      )}

      {/* Floating Quick Action Pill above class with smooth transition */}
      <foreignObject
        x={-20}
        y={-36}
        width={width + 40}
        height={34}
        className="overflow-visible pointer-events-none"
      >
        <div
          className={`flex items-center justify-center h-full pt-1 transition-all duration-200 ease-out ${
            isSelected || isHovered
              ? 'opacity-100 translate-y-0 pointer-events-auto scale-100'
              : 'opacity-0 -translate-y-1 pointer-events-none scale-95'
          }`}
        >
          <div className="inline-flex items-center gap-1 bg-slate-900/95 backdrop-blur-xs text-white px-2 py-1 rounded-md shadow-lg border border-slate-700/80 text-[10px]">
            <button
              onClick={(e) => {
                e.stopPropagation();
                onEdit(box);
              }}
              className="p-1 hover:bg-slate-800 rounded text-slate-200 hover:text-white transition-colors"
              title="Detaljan editor"
            >
              <Edit3 className="h-3 w-3" />
            </button>

            {canCurrentUserDraw && (
              <>
                <button
                  onPointerDown={(e) => {
                    if (e.button !== 0) return;
                    e.stopPropagation();
                    justStartedRef.current = Date.now();
                    startConnectingLine(box.id, 'top');
                  }}
                  onClick={(e) => {
                    e.stopPropagation();
                    if (Date.now() - justStartedRef.current < 350) return;
                    justStartedRef.current = Date.now();
                    startConnectingLine(box.id, 'top');
                  }}
                  className={`p-1 hover:bg-slate-800 rounded transition-colors ${
                    isConnectingStart ? 'bg-blue-600 text-white' : 'text-slate-200 hover:text-white'
                  }`}
                  title="Poveži linijom (klikni ili prevuci)"
                >
                  <GitCommitHorizontal className="h-3 w-3" />
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    if (confirm(`Obriši klasu "${title}"?`)) {
                      deleteElementById(box.id);
                    }
                  }}
                  className="p-1 hover:bg-rose-950 rounded text-rose-300 hover:text-rose-200 transition-colors"
                  title="Obriši klasu"
                >
                  <Trash2 className="h-3 w-3" />
                </button>
              </>
            )}
          </div>
        </div>
      </foreignObject>
    </g>
  );
};
