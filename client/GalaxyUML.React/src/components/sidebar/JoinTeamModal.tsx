import React, { useState } from 'react';
import { useWorkspace } from '../../context/WorkspaceContext';
import { ApiService } from '../../services/api';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Modal } from '../common/Modal';
import { Badge } from '../common/Badge';
import { KeyRound, Search, CheckCircle2, UserCheck } from 'lucide-react';
import { Team } from '../../types';

export interface JoinTeamModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export const JoinTeamModal: React.FC<JoinTeamModalProps> = ({ isOpen, onClose }) => {
  const { joinTeamByCode } = useWorkspace();
  const [code, setCode] = useState('');
  const [foundTeam, setFoundTeam] = useState<Team | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isSearching, setIsSearching] = useState(false);
  const [isJoining, setIsJoining] = useState(false);

  const handleFindTeam = async () => {
    if (!code.trim() || code.trim().length !== 6) {
      setError('Unesite tačan kod tima od 6 karaktera.');
      return;
    }

    setIsSearching(true);
    setError(null);
    setFoundTeam(null);
    try {
      const team = await ApiService.findTeamByCode(code.trim().toUpperCase());
      setFoundTeam(team);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Tim sa ovim kodom nije pronađen.');
    } finally {
      setIsSearching(false);
    }
  };

  const handleJoin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!code.trim()) {
      setError('Unesite kod tima.');
      return;
    }

    setIsJoining(true);
    setError(null);
    try {
      await joinTeamByCode(code.trim().toUpperCase());
      setCode('');
      setFoundTeam(null);
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Neuspešno učlanjenje u tim.');
    } finally {
      setIsJoining(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={
        <div className="flex items-center gap-2">
          <KeyRound className="h-4 w-4 text-emerald-600" />
          <span>Učlanjenje preko koda tima</span>
        </div>
      }
      description="Unesite 6-slovni kod koji ste dobili od vlasnika tima."
      maxWidth="sm"
    >
      <form onSubmit={handleJoin} className="space-y-4">
        <div className="flex gap-2 items-end">
          <div className="flex-1">
            <Input
              label="Kod tima (6 karaktera):"
              placeholder="npr. A9X4K2"
              value={code}
              onChange={(e) => {
                setCode(e.target.value.toUpperCase());
                setError(null);
                setFoundTeam(null);
              }}
              maxLength={6}
              isMonospace
              autoFocus
            />
          </div>
          <Button
            type="button"
            variant="outline"
            size="md"
            onClick={handleFindTeam}
            isLoading={isSearching}
            icon={<Search className="h-3.5 w-3.5" />}
          >
            Pronađi
          </Button>
        </div>

        {foundTeam && (
          <div className="p-3 bg-blue-50 border border-blue-200 rounded-md text-xs">
            <div className="flex items-center gap-1.5 font-bold text-blue-950 mb-1">
              <CheckCircle2 className="h-4 w-4 text-blue-600 shrink-0" />
              <span>Tim pronađen:</span>
            </div>
            <p className="font-semibold text-slate-800 text-sm">{foundTeam.teamName}</p>
            <p className="text-[11px] font-mono text-slate-500 mt-0.5">Kod: {foundTeam.teamCode}</p>
          </div>
        )}

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
            variant="primary"
            size="sm"
            isLoading={isJoining}
            disabled={!code.trim() || code.trim().length !== 6}
            icon={<UserCheck className="h-3.5 w-3.5" />}
          >
            Učlani se
          </Button>
        </div>
      </form>
    </Modal>
  );
};
