import React, { useState } from 'react';
import { TopNavbar } from './TopNavbar';
import { Sidebar } from '../sidebar/Sidebar';
import { DiagramCanvas } from '../canvas/DiagramCanvas';
import { RightDock } from '../dock/RightDock';
import { CreateTeamModal } from '../sidebar/CreateTeamModal';
import { JoinTeamModal } from '../sidebar/JoinTeamModal';

export const AppLayout: React.FC = () => {
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isDockCollapsed, setIsDockCollapsed] = useState(false);
  const [isCreateTeamOpen, setIsCreateTeamOpen] = useState(false);
  const [isJoinTeamOpen, setIsJoinTeamOpen] = useState(false);

  return (
    <div className="h-full w-full flex flex-col overflow-hidden bg-slate-50 text-slate-900">
      {/* Top Precision Navbar */}
      <TopNavbar
        onCreateTeamClick={() => setIsCreateTeamOpen(true)}
        onJoinTeamClick={() => setIsJoinTeamOpen(true)}
      />

      {/* Main Workspace Grid (Left Sidebar + Center Canvas + Right Dock) */}
      <div className="flex-1 flex overflow-hidden relative">
        {/* Left Sidebar: Teams / Members / Meetings */}
        <Sidebar
          isCollapsed={isSidebarCollapsed}
          onToggleCollapse={() => setIsSidebarCollapsed((prev) => !prev)}
        />

        {/* Center: Interactive UML Canvas */}
        <main className="flex-1 h-full relative overflow-hidden flex flex-col">
          <DiagramCanvas />
        </main>

        {/* Right Dock: Meeting Room / Real-time Chat / Inspector */}
        <RightDock
          isCollapsed={isDockCollapsed}
          onToggleCollapse={() => setIsDockCollapsed((prev) => !prev)}
        />
      </div>

      {/* Shared Modals triggered from TopNavbar */}
      <CreateTeamModal isOpen={isCreateTeamOpen} onClose={() => setIsCreateTeamOpen(false)} />
      <JoinTeamModal isOpen={isJoinTeamOpen} onClose={() => setIsJoinTeamOpen(false)} />
    </div>
  );
};
