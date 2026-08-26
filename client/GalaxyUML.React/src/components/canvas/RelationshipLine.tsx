import React, { useState } from 'react';
import { useCanvas } from '../../context/CanvasContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { ClassBoxItem, LineItem } from '../../types';
import { Trash2 } from 'lucide-react';

export interface RelationshipLineProps {
  line: LineItem;
  startBox?: ClassBoxItem;
  endBox?: ClassBoxItem;
  isSelected?: boolean;
}

export const getEdgeToEdgePorts = (
  startBox: ClassBoxItem,
  endBox: ClassBoxItem,
  fixedStartPort?: string | null,
  fixedEndPort?: string | null
) => {
  const startW = Math.max(140, startBox.x2 - startBox.x1);
  const startH = Math.max(90, startBox.y2 - startBox.y1);
  const endW = Math.max(140, endBox.x2 - endBox.x1);
  const endH = Math.max(90, endBox.y2 - endBox.y1);

  const startPorts: Record<string, { name: string; x: number; y: number }> = {
    top: { name: 'top', x: startBox.x1 + startW / 2, y: startBox.y1 },
    right: { name: 'right', x: startBox.x1 + startW, y: startBox.y1 + startH / 2 },
    bottom: { name: 'bottom', x: startBox.x1 + startW / 2, y: startBox.y1 + startH },
    left: { name: 'left', x: startBox.x1, y: startBox.y1 + startH / 2 },
  };

  const endPorts: Record<string, { name: string; x: number; y: number }> = {
    top: { name: 'top', x: endBox.x1 + endW / 2, y: endBox.y1 },
    right: { name: 'right', x: endBox.x1 + endW, y: endBox.y1 + endH / 2 },
    bottom: { name: 'bottom', x: endBox.x1 + endW / 2, y: endBox.y1 + endH },
    left: { name: 'left', x: endBox.x1, y: endBox.y1 + endH / 2 },
  };

  const startCandidates =
    fixedStartPort && startPorts[fixedStartPort]
      ? [startPorts[fixedStartPort]]
      : Object.values(startPorts);

  const endCandidates =
    fixedEndPort && endPorts[fixedEndPort]
      ? [endPorts[fixedEndPort]]
      : Object.values(endPorts);

  let bestStart = startCandidates[0];
  let bestEnd = endCandidates[0];
  let minDistance = Infinity;

  for (const sp of startCandidates) {
    for (const ep of endCandidates) {
      const dist = (sp.x - ep.x) ** 2 + (sp.y - ep.y) ** 2;
      if (dist < minDistance) {
        minDistance = dist;
        bestStart = sp;
        bestEnd = ep;
      }
    }
  }

  return { startPoint: bestStart, endPoint: bestEnd };
};

export const RelationshipLine: React.FC<RelationshipLineProps> = ({
  line,
  startBox,
  endBox,
  isSelected,
}) => {
  const { deleteElementById, selectElement } = useCanvas();
  const { canCurrentUserDraw } = useWorkspace();
  const [isHovered, setIsHovered] = useState(false);

  if (!startBox || !endBox) {
    if (line.x1 !== undefined && line.y1 !== undefined && line.x2 !== undefined && line.y2 !== undefined) {
      return (
        <line
          x1={line.x1}
          y1={line.y1}
          x2={line.x2}
          y2={line.y2}
          stroke="#475569"
          strokeWidth={1.5}
        />
      );
    }
    return null;
  }

  const { startPoint, endPoint } = getEdgeToEdgePorts(startBox, endBox, line.text1, line.text2);

  const midX = (startPoint.x + endPoint.x) / 2;
  const midY = (startPoint.y + endPoint.y) / 2;

  return (
    <g
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      onClick={(e) => {
        e.stopPropagation();
        selectElement(line.id);
      }}
      className="cursor-pointer select-none"
    >
      {/* Invisible Thick Hover/Click Target */}
      <line
        x1={startPoint.x}
        y1={startPoint.y}
        x2={endPoint.x}
        y2={endPoint.y}
        stroke="transparent"
        strokeWidth={16}
      />

      {/* Visible Edge-to-Edge Relationship Line */}
      <line
        x1={startPoint.x}
        y1={startPoint.y}
        x2={endPoint.x}
        y2={endPoint.y}
        stroke={isSelected ? '#2563EB' : isHovered ? '#1E293B' : '#475569'}
        strokeWidth={isSelected ? 2 : 1.5}
        markerEnd="url(#uml-arrow)"
      />

      {/* Middle Text / Cardinality Badge */}
      {line.middleText && (
        <g transform={`translate(${midX}, ${midY})`}>
          <rect
            x={-(line.middleText.length * 4 + 10)}
            y={-10}
            width={line.middleText.length * 8 + 20}
            height={20}
            fill="#FFFFFF"
            stroke={isSelected ? '#2563EB' : '#94A3B8'}
            strokeWidth={1}
            rx={3}
            className="shadow-2xs"
          />
          <text
            x={0}
            y={4}
            textAnchor="middle"
            fill="#0F172A"
            fontSize={11}
            fontWeight="bold"
            fontFamily="JetBrains Mono, monospace"
          >
            {line.middleText}
          </text>
        </g>
      )}

      {/* Delete button on hover */}
      {isHovered && canCurrentUserDraw && (
        <foreignObject
          x={midX - 12}
          y={midY - (line.middleText ? 34 : 12)}
          width={24}
          height={24}
          className="overflow-visible pointer-events-auto"
        >
          <button
            onClick={(e) => {
              e.stopPropagation();
              deleteElementById(line.id);
            }}
            className="h-6 w-6 bg-rose-600 hover:bg-rose-700 text-white rounded-full flex items-center justify-center shadow-md transition-transform hover:scale-110"
            title="Obriši liniju"
          >
            <Trash2 className="h-3 w-3" />
          </button>
        </foreignObject>
      )}
    </g>
  );
};
