import React, { useState } from 'react';
import { useCanvas } from '../../context/CanvasContext';
import { Button } from '../common/Button';
import { Modal } from '../common/Modal';
import { Download, FileImage, FileCode, Check } from 'lucide-react';

export interface ExportDiagramModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ExportDiagramModal: React.FC<ExportDiagramModalProps> = ({ isOpen, onClose }) => {
  const { classBoxes, lines } = useCanvas();
  const [format, setFormat] = useState<'svg' | 'png'>('svg');
  const [isExporting, setIsExporting] = useState(false);

  const handleExport = async () => {
    setIsExporting(true);
    try {
      const svgElement = document.getElementById('galaxy-uml-canvas');
      if (!svgElement) return;

      const svgClone = svgElement.cloneNode(true) as SVGSVGElement;

      // Controls are SVG foreignObjects so they can use regular HTML buttons.
      // They are useful on the board but are not part of the diagram artwork.
      svgClone.querySelectorAll('foreignObject').forEach((element) => element.remove());

      // Compute bounds of all boxes
      let minX = 0;
      let minY = 0;
      let maxX = 1600;
      let maxY = 1000;

      if (classBoxes.length > 0) {
        minX = Math.min(...classBoxes.map((b) => b.x1)) - 60;
        minY = Math.min(...classBoxes.map((b) => b.y1)) - 60;
        maxX = Math.max(...classBoxes.map((b) => b.x2)) + 60;
        maxY = Math.max(...classBoxes.map((b) => b.y2)) + 60;
      }

      const width = Math.max(800, maxX - minX);
      const height = Math.max(600, maxY - minY);

      svgClone.setAttribute('viewBox', `${minX} ${minY} ${width} ${height}`);
      svgClone.setAttribute('width', `${width}`);
      svgClone.setAttribute('height', `${height}`);

      const serializer = new XMLSerializer();
      const svgString = serializer.serializeToString(svgClone);

      if (format === 'svg') {
        const blob = new Blob([svgString], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `GalaxyUML-Diagram-${Date.now()}.svg`;
        a.click();
        URL.revokeObjectURL(url);
      } else {
        // Render the clean SVG into a 2x bitmap without creating a huge data URL.
        const img = new Image();
        const svgBlob = new Blob([svgString], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(svgBlob);

        await new Promise<void>((resolve, reject) => {
          img.onload = () => resolve();
          img.onerror = () => reject(new Error('Unable to render the diagram as PNG'));
          img.src = url;
        });

        try {
          const scale = 2;
          const canvas = document.createElement('canvas');
          canvas.width = Math.ceil(width * scale);
          canvas.height = Math.ceil(height * scale);
          const ctx = canvas.getContext('2d');
          if (!ctx) {
            throw new Error('Unable to create a PNG canvas');
          }

          ctx.fillStyle = '#FFFFFF';
          ctx.fillRect(0, 0, canvas.width, canvas.height);
          ctx.scale(scale, scale);
          ctx.drawImage(img, 0, 0, width, height);

          const pngBlob = await new Promise<Blob | null>((resolve) =>
            canvas.toBlob(resolve, 'image/png')
          );
          if (!pngBlob) {
            throw new Error('Unable to encode the diagram as PNG');
          }

          const pngUrl = URL.createObjectURL(pngBlob);
          const a = document.createElement('a');
          a.href = pngUrl;
          a.download = `GalaxyUML-Diagram-${Date.now()}.png`;
          a.click();
          URL.revokeObjectURL(pngUrl);
        } finally {
          URL.revokeObjectURL(url);
        }
      }

      onClose();
    } catch (err) {
      console.error('Export error:', err);
    } finally {
      setIsExporting(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={
        <div className="flex items-center gap-2">
          <Download className="h-4 w-4 text-blue-600" />
          <span>Izvoz UML Dijagrama</span>
        </div>
      }
      description="Preuzmite trenutni UML dijagram u vektorskom ili rasterskom formatu."
      maxWidth="sm"
    >
      <div className="space-y-4">
        <div className="grid grid-cols-2 gap-3">
          <button
            type="button"
            onClick={() => setFormat('svg')}
            className={`p-3 rounded-lg border text-left transition-all ${
              format === 'svg'
                ? 'bg-blue-50 border-blue-500 text-blue-950 ring-1 ring-blue-500'
                : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300'
            }`}
          >
            <FileCode className="h-5 w-5 text-blue-600 mb-1.5" />
            <p className="font-bold text-xs">Vektorski SVG</p>
            <p className="text-[11px] text-slate-500 mt-0.5">Beskonačna oštrina, idealno za dokumentaciju.</p>
          </button>

          <button
            type="button"
            onClick={() => setFormat('png')}
            className={`p-3 rounded-lg border text-left transition-all ${
              format === 'png'
                ? 'bg-blue-50 border-blue-500 text-blue-950 ring-1 ring-blue-500'
                : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300'
            }`}
          >
            <FileImage className="h-5 w-5 text-emerald-600 mb-1.5" />
            <p className="font-bold text-xs">High-Res PNG</p>
            <p className="text-[11px] text-slate-500 mt-0.5">2x Retina slika sa belom pozadinom.</p>
          </button>
        </div>

        <div className="p-3 bg-slate-50 border border-slate-200 rounded-md text-xs text-slate-600 space-y-1">
          <p>
            <span className="font-semibold text-slate-800">Elementi za izvoz:</span> {classBoxes.length} klasa,{' '}
            {lines.length} relacija.
          </p>
        </div>

        <div className="flex items-center justify-end gap-2 pt-2 border-t border-slate-100">
          <Button variant="outline" size="sm" onClick={onClose}>
            Otkaži
          </Button>
          <Button
            variant="primary"
            size="sm"
            isLoading={isExporting}
            onClick={handleExport}
            icon={<Download className="h-3.5 w-3.5" />}
          >
            Preuzmi {format.toUpperCase()}
          </Button>
        </div>
      </div>
    </Modal>
  );
};
