import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { Button } from '../common/Button';
import { Badge } from '../common/Badge';
import { Tooltip } from '../common/Tooltip';
import { CreateTeamModal } from './CreateTeamModal';
import { JoinTeamModal } from './JoinTeamModal';
import { BanMemberModal } from './BanMemberModal';
import {
  Users,
  UserCheck,
  Video,
  Plus,
  KeyRound,
  Copy,
  Check,
  ChevronLeft,
  ChevronRight,
  Shield,
  ShieldAlert,
  Crown,
  UserMinus,
  Trash2,
  Play,
  LogOut,
  RefreshCw,
  MoreVertical,
  Layers,
} from 'lucide-react';
import * as DropdownMenu from '@radix-ui/react-dropdown-menu';
import { RoleType, TeamMember } from '../../types';

export interface SidebarProps {
  isCollapsed: boolean;
  onToggleCollapse: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ isCollapsed, onToggleCollapse }) => {
  const { user } = useAuth();
  const {
    teams,
    activeTeam,
    activeTeamMembers,
    activeMeeting,
    selectTeam,
    refreshTeams,
    refreshMembers,
    changeMemberRole,
    leaveTeam,
    deleteTeam,
    startMeeting,
    joinMeeting,
    leaveMeeting,
    endMeeting,
    isCurrentUserOrganizerOrOwner,
    isLoadingTeams,
    isLoadingMeeting,
  } = useWorkspace();

  const [activeTab, setActiveTab] = useState<'teams' | 'members' | 'meetings'>('teams');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isJoinModalOpen, setIsJoinModalOpen] = useState(false);
  const [banningMember, setBanningMember] = useState<TeamMember | null>(null);
  const [copiedCode, setCopiedCode] = useState(false);
  const [isStartingMeeting, setIsStartingMeeting] = useState(false);

  const handleCopyCode = () => {
    if (!activeTeam) return;
    navigator.clipboard.writeText(activeTeam.teamCode);
    setCopiedCode(true);
    setTimeout(() => setCopiedCode(false), 2000);
  };

  const handleStartMeeting = async () => {
    setIsStartingMeeting(true);
    try {
      await startMeeting();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Greška pri pokretanju sastanka');
    } finally {
      setIsStartingMeeting(false);
    }
  };

  const isOwnerOfActiveTeam =
    user && activeTeam && activeTeam.ownerId.toLowerCase() === user.idUser.toLowerCase();

  return (
    <aside
      className={`h-full bg-white border-r border-slate-300 flex flex-col shrink-0 transition-all duration-200 z-20 select-none relative ${
        isCollapsed ? 'w-12' : 'w-72'
      }`}
    >
      {/* Collapse / Expand Toggle Button */}
      <button
        onClick={onToggleCollapse}
        className="absolute -right-3 top-3 h-6 w-6 bg-white border border-slate-300 rounded-full flex items-center justify-center text-slate-500 hover:text-slate-900 shadow-xs z-30 focus:outline-none cursor-pointer"
        title={isCollapsed ? 'Proširi bočni meni' : 'Skupi bočni meni'}
      >
        {isCollapsed ? <ChevronRight className="h-3.5 w-3.5" /> : <ChevronLeft className="h-3.5 w-3.5" />}
      </button>

      {isCollapsed ? (
        /* Collapsed Icon Bar */
        <div className="flex flex-col items-center py-3 gap-3">
          <Tooltip content="Moji Timovi" side="right">
            <button
              onClick={() => {
                setActiveTab('teams');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors ${
                activeTab === 'teams'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <Users className="h-4 w-4" />
            </button>
          </Tooltip>

          <Tooltip content="Članovi Tima" side="right">
            <button
              onClick={() => {
                setActiveTab('members');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors ${
                activeTab === 'members'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <UserCheck className="h-4 w-4" />
            </button>
          </Tooltip>

          <Tooltip content="Sastanci i Tabla" side="right">
            <button
              onClick={() => {
                setActiveTab('meetings');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors relative ${
                activeTab === 'meetings'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <Video className="h-4 w-4" />
              {activeMeeting && (
                <span className="absolute top-1 right-1 h-2 w-2 rounded-full bg-emerald-500" />
              )}
            </button>
          </Tooltip>
        </div>
      ) : (
        /* Expanded Sidebar Content */
        <div className="flex flex-col h-full overflow-hidden">
          {/* Tab Navigation Header */}
          <div className="grid grid-cols-3 border-b border-slate-200 bg-slate-50/70 p-1 gap-0.5 shrink-0">
            <button
              onClick={() => setActiveTab('teams')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors ${
                activeTab === 'teams'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <Users className="h-3 w-3" />
              <span>Timovi</span>
            </button>

            <button
              onClick={() => setActiveTab('members')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors ${
                activeTab === 'members'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <UserCheck className="h-3 w-3" />
              <span>Članovi</span>
            </button>

            <button
              onClick={() => setActiveTab('meetings')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors relative ${
                activeTab === 'meetings'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <Video className="h-3 w-3" />
              <span>Sastanci</span>
              {activeMeeting && (
                <span className="h-1.5 w-1.5 rounded-full bg-emerald-500 shrink-0" />
              )}
            </button>
          </div>

          {/* TAB 1: TEAMS HUB */}
          {activeTab === 'teams' && (
            <div className="flex-1 flex flex-col overflow-y-auto p-3 space-y-3">
              {/* Active Team Header Card */}
              {activeTeam ? (
                <div className="p-3 bg-slate-50 border border-slate-300 rounded-lg shadow-2xs">
                  <div className="flex items-start justify-between">
                    <div>
                      <span className="text-[10px] font-bold uppercase tracking-wider text-slate-400">
                        Aktivni Tim
                      </span>
                      <h3 className="font-bold text-xs text-slate-900 truncate mt-0.5">
                        {activeTeam.teamName}
                      </h3>
                    </div>
                    {isOwnerOfActiveTeam && (
                      <Badge variant="warning" size="sm">
                        <Crown className="h-2.5 w-2.5 inline mr-0.5" />
                        Vlasnik
                      </Badge>
                    )}
                  </div>

                  {/* Join Code Display */}
                  <div className="mt-2.5 pt-2 border-t border-slate-200 flex items-center justify-between">
                    <span className="text-[11px] text-slate-500">Kod za pridruživanje:</span>
                    <button
                      onClick={handleCopyCode}
                      className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-white hover:bg-slate-100 active:bg-slate-200 border border-slate-300 rounded font-mono text-[11px] font-bold text-slate-800 transition-colors"
                      title="Kopiraj kod tima"
                    >
                      <span>{activeTeam.teamCode}</span>
                      {copiedCode ? (
                        <Check className="h-3 w-3 text-emerald-600" />
                      ) : (
                        <Copy className="h-3 w-3 text-slate-400" />
                      )}
                    </button>
                  </div>
                </div>
              ) : (
                <div className="p-3 bg-slate-50 border border-slate-200 rounded-md text-xs text-slate-500 text-center">
                  Izaberite ili kreirajte tim.
                </div>
              )}

              {/* Teams List */}
              <div className="flex-1">
                <div className="flex items-center justify-between mb-1.5">
                  <span className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                    Svi vaši timovi ({teams.length})
                  </span>
                  <button
                    onClick={refreshTeams}
                    className="text-slate-400 hover:text-slate-700 p-0.5"
                    title="Osveži listu timova"
                  >
                    <RefreshCw className={`h-3 w-3 ${isLoadingTeams ? 'animate-spin' : ''}`} />
                  </button>
                </div>

                <div className="space-y-1">
                  {teams.map((t) => (
                    <div
                      key={t.id}
                      onClick={() => selectTeam(t)}
                      className={`p-2 rounded-md border text-xs cursor-pointer transition-all flex items-center justify-between ${
                        activeTeam?.id === t.id
                          ? 'bg-blue-50/80 border-blue-300 text-blue-950 font-semibold'
                          : 'bg-white border-slate-200 text-slate-700 hover:border-slate-300 hover:bg-slate-50'
                      }`}
                    >
                      <div className="truncate pr-2">
                        <p className="truncate text-xs">{t.teamName}</p>
                        <p className="text-[10px] font-mono text-slate-400 mt-0.5">
                          Kod: {t.teamCode}
                        </p>
                      </div>
                      <span className="text-[10px] bg-slate-100 text-slate-600 px-1.5 py-0.5 rounded shrink-0 border border-slate-200">
                        {t.members?.length || t.memberCount || 1} član(a)
                      </span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Action Buttons */}
              <div className="pt-2 border-t border-slate-200 space-y-1.5 shrink-0">
                <Button
                  variant="secondary"
                  size="sm"
                  className="w-full justify-start"
                  onClick={() => setIsCreateModalOpen(true)}
                  icon={<Plus className="h-3.5 w-3.5 text-blue-600" />}
                >
                  Kreiraj novi tim
                </Button>

                <Button
                  variant="outline"
                  size="sm"
                  className="w-full justify-start"
                  onClick={() => setIsJoinModalOpen(true)}
                  icon={<KeyRound className="h-3.5 w-3.5 text-emerald-600" />}
                >
                  Učlani se preko koda
                </Button>

                {activeTeam && (
                  <div className="pt-1.5 flex gap-1.5">
                    <Button
                      variant="ghost"
                      size="sm"
                      className="flex-1 text-[11px] text-slate-600 hover:text-slate-900"
                      onClick={() => {
                        if (confirm(`Napusti tim "${activeTeam.teamName}"?`)) {
                          leaveTeam(activeTeam.id);
                        }
                      }}
                      icon={<LogOut className="h-3 w-3" />}
                    >
                      Napusti tim
                    </Button>

                    {isOwnerOfActiveTeam && (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-[11px] text-rose-600 hover:bg-rose-50"
                        onClick={() => {
                          if (confirm(`Potvrdite trajno brisanje tima "${activeTeam.teamName}"?`)) {
                            deleteTeam(activeTeam.id);
                          }
                        }}
                        icon={<Trash2 className="h-3 w-3 text-rose-600" />}
                      >
                        Obriši
                      </Button>
                    )}
                  </div>
                )}
              </div>
            </div>
          )}

          {/* TAB 2: TEAM MEMBERS & ROLES */}
          {activeTab === 'members' && (
            <div className="flex-1 flex flex-col overflow-y-auto p-3 space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                  Članovi tima ({activeTeamMembers.length})
                </span>
                <button
                  onClick={refreshMembers}
                  className="text-slate-400 hover:text-slate-700 p-0.5"
                  title="Osveži listu članova"
                >
                  <RefreshCw className="h-3 w-3" />
                </button>
              </div>

              {!activeTeam ? (
                <div className="p-3 bg-slate-50 border border-slate-200 rounded-md text-xs text-slate-500 text-center">
                  Izaberite tim za prikaz članova.
                </div>
              ) : activeTeamMembers.length === 0 ? (
                <div className="p-3 bg-slate-50 border border-slate-200 rounded-md text-xs text-slate-500 text-center">
                  Učitavanje članova...
                </div>
              ) : (
                <div className="space-y-1.5 flex-1 overflow-y-auto">
                  {activeTeamMembers.map((m) => {
                    const isSelf = user && m.userId.toLowerCase() === user.idUser.toLowerCase();
                    const isMemberOwner = m.role === 'Owner';

                    return (
                      <div
                        key={m.userId}
                        className="p-2 bg-white border border-slate-200 rounded-md text-xs flex items-center justify-between gap-2 shadow-2xs hover:border-slate-300"
                      >
                        <div className="min-w-0 flex-1">
                          <div className="flex items-center gap-1.5">
                            <span className="font-bold text-slate-900 truncate">{m.username}</span>
                            {isSelf && (
                              <span className="text-[9px] bg-slate-100 text-slate-600 px-1 py-0.2 rounded font-medium">
                                Vi
                              </span>
                            )}
                          </div>
                          <p className="text-[11px] text-slate-500 truncate">
                            {m.firstName} {m.lastName}
                          </p>
                          <p className="text-[10px] font-mono text-slate-400 truncate">{m.email}</p>
                        </div>

                        <div className="flex items-center gap-1.5 shrink-0">
                          <Badge
                            variant={
                              m.role === 'Owner'
                                ? 'warning'
                                : m.role === 'Organizer'
                                ? 'primary'
                                : 'default'
                            }
                            size="sm"
                          >
                            {m.role === 'Owner' ? 'Owner' : m.role === 'Organizer' ? 'Organizer' : 'Member'}
                          </Badge>

                          {/* Owner/Admin actions menu */}
                          {isOwnerOfActiveTeam && !isSelf && !isMemberOwner && (
                            <DropdownMenu.Root>
                              <DropdownMenu.Trigger asChild>
                                <button className="p-1 text-slate-400 hover:text-slate-700 rounded hover:bg-slate-100 focus:outline-none">
                                  <MoreVertical className="h-3.5 w-3.5" />
                                </button>
                              </DropdownMenu.Trigger>
                              <DropdownMenu.Portal>
                                <DropdownMenu.Content
                                  align="end"
                                  className="z-50 w-44 bg-white border border-slate-300 rounded-lg shadow-lg p-1 text-xs text-slate-800 animate-in fade-in-0 zoom-in-95"
                                >
                                  <div className="px-2 py-1 text-[10px] font-semibold text-slate-400 uppercase">
                                    Upravljanje ulogom
                                  </div>
                                  <DropdownMenu.Item
                                    onClick={() =>
                                      changeMemberRole(
                                        m.userId,
                                        m.role === 'Organizer' ? 'Member' : 'Organizer'
                                      )
                                    }
                                    className="flex items-center gap-1.5 px-2 py-1.5 rounded-md cursor-pointer hover:bg-slate-100 outline-none"
                                  >
                                    <Shield className="h-3.5 w-3.5 text-blue-600" />
                                    <span>
                                      {m.role === 'Organizer'
                                        ? 'Smanji na Member'
                                        : 'Unapredi u Organizer'}
                                    </span>
                                  </DropdownMenu.Item>

                                  <DropdownMenu.Separator className="h-px bg-slate-200 my-1" />

                                  <DropdownMenu.Item
                                    onClick={() => setBanningMember(m)}
                                    className="flex items-center gap-1.5 px-2 py-1.5 rounded-md cursor-pointer hover:bg-rose-50 text-rose-700 outline-none font-semibold"
                                  >
                                    <UserMinus className="h-3.5 w-3.5 text-rose-600" />
                                    <span>Banuj člana</span>
                                  </DropdownMenu.Item>
                                </DropdownMenu.Content>
                              </DropdownMenu.Portal>
                            </DropdownMenu.Root>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          )}

          {/* TAB 3: MEETINGS & DRAWING SESSIONS */}
          {activeTab === 'meetings' && (
            <div className="flex-1 flex flex-col overflow-y-auto p-3 space-y-3">
              <span className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                Sastanci tima
              </span>

              {/* Active Meeting Card */}
              {activeMeeting ? (
                <div className="p-3 bg-emerald-50/60 border border-emerald-300 rounded-lg shadow-2xs space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="font-bold text-xs text-emerald-950 flex items-center gap-1.5">
                      <span className="h-2 w-2 rounded-full bg-emerald-600 animate-live-beacon" />
                      Aktivan Sastanak
                    </span>
                    <Badge variant="success" size="sm">
                      U toku
                    </Badge>
                  </div>

                  <div className="text-[11px] space-y-1 text-slate-600 font-mono">
                    <p className="truncate">
                      Sastanak ID: {(activeMeeting?.id || '').slice(0, 8)}...
                    </p>
                    <p className="truncate">
                      Tabla ID: {(activeMeeting?.boardId || activeMeeting?.board?.id || '').slice(0, 8)}...
                    </p>
                  </div>

                  <div className="pt-2 border-t border-emerald-200/60 flex flex-col gap-1.5">
                    <Button
                      variant="primary"
                      size="sm"
                      className="w-full bg-emerald-700 hover:bg-emerald-800 border-emerald-800"
                      onClick={() => joinMeeting(activeMeeting.id)}
                      icon={<Layers className="h-3.5 w-3.5" />}
                    >
                      Poveži se sa tablom
                    </Button>

                    <div className="flex gap-1.5">
                      <Button
                        variant="outline"
                        size="sm"
                        className="flex-1 text-[11px]"
                        onClick={leaveMeeting}
                        icon={<LogOut className="h-3 w-3" />}
                      >
                        Napusti
                      </Button>

                      {isCurrentUserOrganizerOrOwner && (
                        <Button
                          variant="danger"
                          size="sm"
                          className="flex-1 text-[11px]"
                          onClick={() => {
                            if (confirm('Završi sastanak za sve učesnike?')) {
                              endMeeting();
                            }
                          }}
                          icon={<Trash2 className="h-3 w-3" />}
                        >
                          Završi
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              ) : (
                <div className="p-4 bg-slate-50 border border-slate-200 rounded-lg text-center space-y-3">
                  <div className="h-9 w-9 bg-slate-200 text-slate-600 rounded-full flex items-center justify-center mx-auto">
                    <Video className="h-4 w-4" />
                  </div>
                  <div>
                    <h4 className="font-semibold text-xs text-slate-800">Nema aktivnog sastanka</h4>
                    <p className="text-[11px] text-slate-500 mt-0.5">
                      Pokrenite sastanak za deljenu UML tablu u realnom vremenu.
                    </p>
                  </div>

                  <Button
                    variant="primary"
                    size="sm"
                    className="w-full"
                    disabled={!activeTeam}
                    isLoading={isStartingMeeting}
                    onClick={handleStartMeeting}
                    icon={<Play className="h-3.5 w-3.5" />}
                  >
                    Pokreni novi sastanak
                  </Button>
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* Modals */}
      <CreateTeamModal isOpen={isCreateModalOpen} onClose={() => setIsCreateModalOpen(false)} />
      <JoinTeamModal isOpen={isJoinModalOpen} onClose={() => setIsJoinModalOpen(false)} />
      <BanMemberModal
        isOpen={!!banningMember}
        member={banningMember}
        onClose={() => setBanningMember(null)}
      />
    </aside>
  );
};
