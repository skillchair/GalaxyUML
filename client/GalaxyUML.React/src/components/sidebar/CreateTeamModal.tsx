import React, { useState } from 'react';
import { useWorkspace } from '../../context/WorkspaceContext';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Modal } from '../common/Modal';
import { Plus, Users } from 'lucide-react';

export interface CreateTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export const CreateTeamModal: React.FC<CreateTeamModalProps> = ({ isOpen, onClose }) => {
  const { createTeam } = useWorkspace();
  const [teamName, setTeamName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!teamName.trim()) {
      setError('Naziv tima je obavezan.');
      return;
    }

    setIsLoading(true);
    setError(null);
    try {
      await createTeam(teamName.trim());
      setTeamName('');
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Greška pri kreiranju tima.');
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
          <Users className="h-4 w-4 text-blue-600" />
          <span>Kreiraj novi tim</span>
        </div>
      }
      description="Kreirajte tim za saradnju i pozovite članove putem jedinstvenog koda."
      maxWidth="sm"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        <Input
          label="Naziv tima:"
          placeholder="npr. AIPS Grupa 4 / Backend Tim"
          value={teamName}
          onChange={(e) => {
            setTeamName(e.target.value);
            setError(null);
          }}
          autoFocus
          error={error || undefined}
        />

        <div className="flex items-center justify-end gap-2 pt-2 border-t border-slate-100">
          <Button type="button" variant="outline" size="sm" onClick={onClose}>
            Otkaži
          </Button>
          <Button
            type="submit"
            variant="primary"
            size="sm"
            isLoading={isLoading}
            icon={<Plus className="h-3.5 w-3.5" />}
          >
            Kreiraj tim
          </Button>
        </div>
      </form>
    </Modal>
  );
};
