import React, { useState } from 'react';
import { useWorkspace } from '../../context/WorkspaceContext';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Modal } from '../common/Modal';
import { UserX, ShieldAlert } from 'lucide-react';
import { TeamMember } from '../../types';

export interface BanMemberModalProps {
  isOpen: boolean;
  member: TeamMember | null;
  onClose: () => void;
}

export const BanMemberModal: React.FC<BanMemberModalProps> = ({ isOpen, member, onClose }) => {
  const { banMember } = useWorkspace();
  const [reason, setReason] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  if (!member) return null;

  const handleBan = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      await banMember(member.userId, reason.trim() || undefined);
      setReason('');
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Greška pri banovanju člana.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={
        <div className="flex items-center gap-2 text-rose-700">
          <ShieldAlert className="h-4 w-4 text-rose-600" />
          <span>Banuj člana iz tima</span>
        </div>
      }
      description={`Da li ste sigurni da želite da banujete korisnika "${member.username}" (${member.firstName} ${member.lastName})?`}
      maxWidth="sm"
    >
      <form onSubmit={handleBan} className="space-y-4">
        <Input
          label="Razlog banovanja (opciono):"
          placeholder="npr. Kršenje pravila timskog rada"
          value={reason}
          onChange={(e) => setReason(e.target.value)}
          autoFocus
        />

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
            type="submit"
            variant="danger"
            size="sm"
            isLoading={isLoading}
            icon={<UserX className="h-3.5 w-3.5" />}
          >
            Potvrdi Ban
          </Button>
        </div>
      </form>
    </Modal>
  );
};
