import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { useCanvas } from '../../context/CanvasContext';
import { Button } from '../common/Button';
import { Badge } from '../common/Badge';
import { Tooltip } from '../common/Tooltip';
import { ApiSettingsModal } from '../settings/ApiSettingsModal';
import {
  Layers,
  MousePointer,
  SquareDashedBottomCode,
  GitCommitHorizontal,
  ZoomIn,
  ZoomOut,
  Maximize2,
  Grid,
  Radio,
  User,
  LogOut,
  Settings,
  ChevronDown,
  Plus,
  Users,
  Video,
} from 'lucide-react';
import * as DropdownMenu from '@radix-ui/react-dropdown-menu';

export interface TopNavbarProps {
  onCreateTeamClick?: () => void;
  onJoinTeamClick?: () => void;
}

export const TopNavbar: React.FC<TopNavbarProps> = ({ onCreateTeamClick, onJoinTeamClick }) => {
  const { user, logout } = useAuth();
  const {
    teams,
    activeTeam,
    selectTeam,
    activeMeeting,
    canCurrentUserDraw,
    signalRStatus,
  } = useWorkspace();

  const {
    activeTool,
    setActiveTool,
    transform,
    zoomIn,
    zoomOut,
    zoomToFit,
    isGridSnapEnabled,
    toggleGridSnap,
  } = useCanvas();

  const [isApiSettingsOpen, setIsApiSettingsOpen] = useState(false);

  return (
    <header className="h-12 w-full bg-white border-b border-slate-300 px-3 flex items-center justify-between shrink-0 select-none z-30 shadow-2xs">
      {/* LEFT: Brand & Team Switcher */}
      <div className="flex items-center gap-3">
        {/* Brand */}
        <div className="flex items-center gap-2 pr-3 border-r border-slate-200">
          <div className="h-6 w-6 bg-slate-900 rounded flex items-center justify-center text-white shadow-xs">
            <Layers className="h-3.5 w-3.5 text-blue-400" />
          </div>
          <div className="flex flex-col">
            <span className="font-bold text-xs tracking-tight text-slate-900 leading-tight">
              GalaxyUML
            </span>
          </div>
        </div>

        {/* Team Selector Dropdown */}
        <DropdownMenu.Root>
          <DropdownMenu.Trigger asChild>
            <button className="h-8 px-2.5 bg-slate-50 hover:bg-slate-100 active:bg-slate-200 border border-slate-300 rounded-md flex items-center gap-2 text-xs font-semibold text-slate-800 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500/20">
              <Users className="h-3.5 w-3.5 text-slate-500" />
              <span className="max-w-[140px] truncate font-medium">
                {activeTeam ? activeTeam.teamName : 'Izaberite tim'}
              </span>
              {activeTeam && (
                <span className="font-mono text-[10px] bg-slate-200 text-slate-700 px-1 py-0.2 rounded">
                  {activeTeam.teamCode}
                </span>
              )}
              <ChevronDown className="h-3 w-3 text-slate-400" />
            </button>
          </DropdownMenu.Trigger>

          <DropdownMenu.Portal>
            <DropdownMenu.Content
              align="start"
              sideOffset={4}
              className="z-50 w-56 bg-white border border-slate-300 rounded-lg shadow-lg p-1 animate-in fade-in-0 zoom-in-95 text-xs text-slate-800"
            >
              <div className="px-2 py-1.5 text-[11px] font-semibold text-slate-400 uppercase tracking-wider">
                Moji Timovi
              </div>

              {teams.length === 0 ? (
                <div className="px-2 py-2 text-xs text-slate-500 text-center">Niste član nijednog tima</div>
              ) : (
                teams.map((t) => (
                  <DropdownMenu.Item
                    key={t.id}
                    onClick={() => selectTeam(t)}
                    className={`flex items-center justify-between px-2 py-1.5 rounded-md cursor-pointer outline-none transition-colors ${
                      activeTeam?.id === t.id
                        ? 'bg-blue-50 text-blue-900 font-semibold'
                        : 'hover:bg-slate-100 text-slate-700'
                    }`}
                  >
                    <span className="truncate">{t.teamName}</span>
                    <span className="font-mono text-[10px] text-slate-500 bg-slate-100 px-1 py-0.2 rounded border border-slate-200">
                      {t.teamCode}
                    </span>
                  </DropdownMenu.Item>
                ))
              )}

              <DropdownMenu.Separator className="h-px bg-slate-200 my-1" />

              <DropdownMenu.Item
                onClick={onCreateTeamClick}
                className="flex items-center gap-1.5 px-2 py-1.5 rounded-md cursor-pointer hover:bg-slate-100 text-slate-700 outline-none"
              >
                <Plus className="h-3.5 w-3.5 text-blue-600" />
                <span>Kreiraj novi tim</span>
              </DropdownMenu.Item>

              <DropdownMenu.Item
                onClick={onJoinTeamClick}
                className="flex items-center gap-1.5 px-2 py-1.5 rounded-md cursor-pointer hover:bg-slate-100 text-slate-700 outline-none"
              >
                <Users className="h-3.5 w-3.5 text-emerald-600" />
                <span>Učlani se preko koda</span>
              </DropdownMenu.Item>
            </DropdownMenu.Content>
          </DropdownMenu.Portal>
        </DropdownMenu.Root>

        {/* Meeting Live Beacon */}
        {activeMeeting ? (
          <div className="flex items-center gap-1.5 px-2 py-1 bg-emerald-50 border border-emerald-300 rounded-md text-emerald-800 text-[11px] font-semibold">
            <span className="relative flex h-2 w-2">
              <span className="animate-live-beacon absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75" />
              <span className="relative inline-flex rounded-full h-2 w-2 bg-emerald-600" />
            </span>
            <Video className="h-3 w-3 text-emerald-700" />
            <span>Sastanak u toku</span>
          </div>
        ) : (
          <div className="flex items-center gap-1.5 px-2 py-1 bg-slate-100 border border-slate-200 rounded-md text-slate-500 text-[11px] font-medium">
            <span className="h-1.5 w-1.5 rounded-full bg-slate-400" />
            <span>Nema sastanka</span>
          </div>
        )}
      </div>

      {/* CENTER: Canvas Tool Switcher & Zoom Controls */}
      <div className="flex items-center gap-1">
        {/* Tool Mode Buttons */}
        <div className="flex items-center p-0.5 bg-slate-100 border border-slate-300 rounded-md">
          <Tooltip content="Selektovanje i pomeranje elemenata" shortcut="V">
            <button
              onClick={() => setActiveTool('select')}
              className={`h-7 px-2.5 rounded text-xs font-semibold flex items-center gap-1.5 transition-all ${
                activeTool === 'select'
                  ? 'bg-white text-slate-900 shadow-xs border border-slate-300'
                  : 'text-slate-600 hover:text-slate-900 border border-transparent'
              }`}
            >
              <MousePointer className="h-3.5 w-3.5" />
              <span>Izaberi</span>
            </button>
          </Tooltip>

          <Tooltip content="Dodaj novi UML Class Box na tablu" shortcut="C">
            <button
              onClick={() => setActiveTool('classBox')}
              disabled={!canCurrentUserDraw}
              className={`h-7 px-2.5 rounded text-xs font-semibold flex items-center gap-1.5 transition-all disabled:opacity-40 disabled:cursor-not-allowed ${
                activeTool === 'classBox'
                  ? 'bg-white text-blue-700 shadow-xs border border-blue-300 font-bold'
                  : 'text-slate-600 hover:text-slate-900 border border-transparent'
              }`}
            >
              <SquareDashedBottomCode className="h-3.5 w-3.5" />
              <span>Klasa</span>
            </button>
          </Tooltip>

          <Tooltip content="Poveži dve klase relacionom linijom" shortcut="L">
            <button
              onClick={() => setActiveTool('line')}
              disabled={!canCurrentUserDraw}
              className={`h-7 px-2.5 rounded text-xs font-semibold flex items-center gap-1.5 transition-all disabled:opacity-40 disabled:cursor-not-allowed ${
                activeTool === 'line'
                  ? 'bg-white text-blue-700 shadow-xs border border-blue-300 font-bold'
                  : 'text-slate-600 hover:text-slate-900 border border-transparent'
              }`}
            >
              <GitCommitHorizontal className="h-3.5 w-3.5" />
              <span>Linija</span>
            </button>
          </Tooltip>
        </div>

        {/* Zoom & Grid HUD */}
        <div className="flex items-center pl-2 ml-1 border-l border-slate-200 gap-1">
          <Tooltip content="Uvećaj prikaz">
            <button
              onClick={zoomIn}
              className="h-7 w-7 flex items-center justify-center rounded text-slate-600 hover:text-slate-900 hover:bg-slate-100 border border-transparent active:bg-slate-200"
            >
              <ZoomIn className="h-3.5 w-3.5" />
            </button>
          </Tooltip>

          <span className="w-12 text-center font-mono text-[11px] font-semibold text-slate-600">
            {Math.round(transform.scale * 100)}%
          </span>

          <Tooltip content="Umanji prikaz">
            <button
              onClick={zoomOut}
              className="h-7 w-7 flex items-center justify-center rounded text-slate-600 hover:text-slate-900 hover:bg-slate-100 border border-transparent active:bg-slate-200"
            >
              <ZoomOut className="h-3.5 w-3.5" />
            </button>
          </Tooltip>

          <Tooltip content="Prilagodi prikaz dijagramu" shortcut="0">
            <button
              onClick={zoomToFit}
              className="h-7 w-7 flex items-center justify-center rounded text-slate-600 hover:text-slate-900 hover:bg-slate-100 border border-transparent active:bg-slate-200"
            >
              <Maximize2 className="h-3.5 w-3.5" />
            </button>
          </Tooltip>

          <Tooltip content={isGridSnapEnabled ? 'Isključi magnetnu mrežu' : 'Uključi magnetnu mrežu (20px)'}>
            <button
              onClick={toggleGridSnap}
              className={`h-7 px-2 rounded text-[11px] font-semibold flex items-center gap-1 border transition-colors ${
                isGridSnapEnabled
                  ? 'bg-blue-50 text-blue-700 border-blue-200'
                  : 'bg-transparent text-slate-400 border-transparent hover:text-slate-700'
              }`}
            >
              <Grid className="h-3 w-3" />
              <span>Mreža</span>
            </button>
          </Tooltip>
        </div>
      </div>

      {/* RIGHT: Draw Badge + SignalR status + Profile Menu */}
      <div className="flex items-center gap-2">
        {/* Draw Permission Badge */}
        {activeMeeting && (
          <Badge
            variant={canCurrentUserDraw ? 'success' : 'warning'}
            dot
            className="font-medium text-[11px]"
          >
            {canCurrentUserDraw ? 'Crtanje dozvoljeno' : 'Samo pregled'}
          </Badge>
        )}

        {/* SignalR Badge */}
        <Badge
          variant={
            signalRStatus === 'connected'
              ? 'success'
              : signalRStatus === 'connecting' || signalRStatus === 'reconnecting'
              ? 'warning'
              : 'default'
          }
          dot
          className="font-mono text-[10px]"
        >
          <Radio className="h-3 w-3 inline mr-0.5" />
          {signalRStatus === 'connected'
            ? 'SignalR: Povezan'
            : signalRStatus === 'connecting'
            ? 'Povezivanje...'
            : signalRStatus === 'reconnecting'
            ? 'Rekonekcija...'
            : 'SignalR: Offline'}
        </Badge>

        {/* Settings button */}
        <Tooltip content="Podešavanje API Endpointa">
          <button
            onClick={() => setIsApiSettingsOpen(true)}
            className="h-8 w-8 flex items-center justify-center rounded-md border border-slate-300 text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-colors"
          >
            <Settings className="h-3.5 w-3.5" />
          </button>
        </Tooltip>

        {/* User Profile Dropdown */}
        {user && (
          <DropdownMenu.Root>
            <DropdownMenu.Trigger asChild>
              <button className="h-8 pl-1.5 pr-2.5 bg-slate-100 hover:bg-slate-200 border border-slate-300 rounded-md flex items-center gap-2 text-xs font-semibold text-slate-800 transition-colors focus:outline-none">
                <div className="h-5 w-5 bg-slate-900 text-white rounded flex items-center justify-center text-[10px] font-bold">
                  {user.username ? user.username[0].toUpperCase() : 'U'}
                </div>
                <span className="max-w-[100px] truncate">{user.username}</span>
                <ChevronDown className="h-3 w-3 text-slate-400" />
              </button>
            </DropdownMenu.Trigger>

            <DropdownMenu.Portal>
              <DropdownMenu.Content
                align="end"
                sideOffset={4}
                className="z-50 w-52 bg-white border border-slate-300 rounded-lg shadow-lg p-1 animate-in fade-in-0 zoom-in-95 text-xs text-slate-800"
              >
                <div className="px-3 py-2 border-b border-slate-100">
                  <p className="font-bold text-slate-900 truncate">{user.username}</p>
                  <p className="text-[11px] font-mono text-slate-500 truncate">{user.email}</p>
                </div>

                <DropdownMenu.Item
                  onClick={() => setIsApiSettingsOpen(true)}
                  className="flex items-center gap-2 px-2.5 py-1.5 rounded-md cursor-pointer hover:bg-slate-100 text-slate-700 outline-none mt-1"
                >
                  <Settings className="h-3.5 w-3.5 text-slate-500" />
                  <span>API Podešavanja</span>
                </DropdownMenu.Item>

                <DropdownMenu.Separator className="h-px bg-slate-200 my-1" />

                <DropdownMenu.Item
                  onClick={logout}
                  className="flex items-center gap-2 px-2.5 py-1.5 rounded-md cursor-pointer hover:bg-rose-50 text-rose-700 font-semibold outline-none"
                >
                  <LogOut className="h-3.5 w-3.5 text-rose-600" />
                  <span>Odjavi se</span>
                </DropdownMenu.Item>
              </DropdownMenu.Content>
            </DropdownMenu.Portal>
          </DropdownMenu.Root>
        )}
      </div>

      <ApiSettingsModal isOpen={isApiSettingsOpen} onClose={() => setIsApiSettingsOpen(false)} />
    </header>
  );
};
