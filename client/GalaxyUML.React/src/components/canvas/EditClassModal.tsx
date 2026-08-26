import React, { useState, useEffect } from 'react';
import { useCanvas } from '../../context/CanvasContext';
import { ApiService } from '../../services/api';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Modal } from '../common/Modal';
import { Plus, Trash2, SquareDashedBottomCode, Code, Check } from 'lucide-react';
import { ClassBoxItem } from '../../types';
import { parseClassBoxData } from './ClassBoxNode';

export interface EditClassModalProps {
  isOpen: boolean;
  classBox: ClassBoxItem | null;
  onClose: () => void;
}

export const EditClassModal: React.FC<EditClassModalProps> = ({ isOpen, classBox, onClose }) => {
  const { loadBoardElements, boardId } = useCanvas();

  const [className, setClassName] = useState('');
  const [attributes, setAttributes] = useState<string[]>([]);
  const [methods, setMethods] = useState<string[]>([]);
  const [newAttr, setNewAttr] = useState('');
  const [newMethod, setNewMethod] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (classBox) {
      const { title, attributesList, methodsList } = parseClassBoxData(
        classBox.attributes,
        classBox.methods
      );
      setClassName(title);
      setAttributes(attributesList);
      setMethods(methodsList);
    }
  }, [classBox]);

  if (!classBox) return null;

  const handleAddAttribute = () => {
    if (newAttr.trim()) {
      setAttributes([...attributes, newAttr.trim()]);
      setNewAttr('');
    }
  };

  const handleRemoveAttribute = (idx: number) => {
    setAttributes(attributes.filter((_, i) => i !== idx));
  };

  const handleAddMethod = () => {
    if (newMethod.trim()) {
      setMethods([...methods, newMethod.trim()]);
      setNewMethod('');
    }
  };

  const handleRemoveMethod = (idx: number) => {
    setMethods(methods.filter((_, i) => i !== idx));
  };

  const handleSave = async () => {
    if (!className.trim() || !boardId) return;
    setIsLoading(true);
    setError(null);
    try {
      // To update attributes/methods in backend: delete old box & re-add, or update
      // We re-add with same coordinates and update lines
      await ApiService.deleteElement(classBox.id);
      await ApiService.addClassBox(boardId, {
        x1: classBox.x1,
        y1: classBox.y1,
        x2: classBox.x2,
        y2: classBox.y2,
        attributes: [className.trim(), ...attributes],
        methods: methods,
      });

      await loadBoardElements();
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Greška pri čuvanju klase.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={
        <div className="flex items-center gap-2">
          <SquareDashedBottomCode className="h-4 w-4 text-blue-600" />
          <span>Uredi UML Klasu</span>
        </div>
      }
      description="Prilagodite naziv klase, listu atributa i metoda prema UML standardu."
      maxWidth="md"
    >
      <div className="space-y-4">
        {/* Class Name */}
        <Input
          label="Naziv klase:"
          value={className}
          onChange={(e) => setClassName(e.target.value)}
          placeholder="npr. Korisnik / Narudžbina"
          isMonospace
          autoFocus
        />

        {/* Attributes Section */}
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <label className="text-xs font-semibold text-slate-700">
              Atributi ({attributes.length})
            </label>
            <span className="text-[10px] text-slate-400 font-mono">+ javno, - privatno, # zaštićeno</span>
          </div>

          <div className="flex gap-2">
            <Input
              value={newAttr}
              onChange={(e) => setNewAttr(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddAttribute())}
              placeholder="+ id: int / - email: string"
              isMonospace
              className="h-7 text-xs"
            />
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleAddAttribute}
              icon={<Plus className="h-3 w-3" />}
            >
              Dodaj
            </Button>
          </div>

          <div className="max-h-28 overflow-y-auto space-y-1 bg-slate-50 p-2 rounded-md border border-slate-200">
            {attributes.length === 0 ? (
              <p className="text-[11px] text-slate-400 italic">Nema definisanih atributa</p>
            ) : (
              attributes.map((attr, idx) => (
                <div
                  key={idx}
                  className="flex items-center justify-between bg-white px-2 py-1 rounded border border-slate-200 text-xs font-mono text-slate-800"
                >
                  <span className="truncate">{attr}</span>
                  <button
                    onClick={() => handleRemoveAttribute(idx)}
                    className="text-slate-400 hover:text-rose-600 p-0.5"
                  >
                    <Trash2 className="h-3 w-3" />
                  </button>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Methods Section */}
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <label className="text-xs font-semibold text-slate-700">
              Metode ({methods.length})
            </label>
            <span className="text-[10px] text-slate-400 font-mono">+ potpis(): povratniTip</span>
          </div>

          <div className="flex gap-2">
            <Input
              value={newMethod}
              onChange={(e) => setNewMethod(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddMethod())}
              placeholder="+ login(): boolean"
              isMonospace
              className="h-7 text-xs"
            />
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleAddMethod}
              icon={<Plus className="h-3 w-3" />}
            >
              Dodaj
            </Button>
          </div>

          <div className="max-h-28 overflow-y-auto space-y-1 bg-slate-50 p-2 rounded-md border border-slate-200">
            {methods.length === 0 ? (
              <p className="text-[11px] text-slate-400 italic">Nema definisanih metoda</p>
            ) : (
              methods.map((method, idx) => (
                <div
                  key={idx}
                  className="flex items-center justify-between bg-white px-2 py-1 rounded border border-slate-200 text-xs font-mono text-slate-800"
                >
                  <span className="truncate">{method}</span>
                  <button
                    onClick={() => handleRemoveMethod(idx)}
                    className="text-slate-400 hover:text-rose-600 p-0.5"
                  >
                    <Trash2 className="h-3 w-3" />
                  </button>
                </div>
              ))
            )}
          </div>
        </div>

        {error && (
          <div className="p-2.5 bg-rose-50 border border-rose-200 text-rose-700 rounded-md text-xs">
            {error}
          </div>
        )}

        <div className="flex items-center justify-end gap-2 pt-2 border-t border-slate-100">
          <Button type="button" variant="outline" size="sm" onClick={onClose}>
            Otkaži
          </Button>
          <Button
            type="button"
            variant="primary"
            size="sm"
            isLoading={isLoading}
            onClick={handleSave}
            icon={<Check className="h-3.5 w-3.5" />}
          >
            Sačuvaj izmene
          </Button>
        </div>
      </div>
    </Modal>
  );
};
