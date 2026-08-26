import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { ApiService } from '../services/api';
import { SignalRService } from '../services/signalr';
import {
  ChatMessage,
  Meeting,
  MeetingParticipant,
  RoleType,
  SignalRStatus,
  Team,
  TeamMember,
} from '../types';
import { useAuth } from './AuthContext';

interface WorkspaceContextType {
  teams: Team[];
  activeTeam: Team | null;
  activeTeamMembers: TeamMember[];
  activeMeeting: Meeting | null;
  activeParticipants: MeetingParticipant[];
  canCurrentUserDraw: boolean;
  isCurrentUserOrganizerOrOwner: boolean;
  chatMessages: ChatMessage[];
  signalRStatus: SignalRStatus;
  isLoadingTeams: boolean;
  isLoadingMeeting: boolean;
  unreadChatCount: number;
  resetUnreadChat: () => void;
  selectTeam: (team: Team | null) => Promise<void>;
  refreshTeams: () => Promise<void>;
  refreshMembers: () => Promise<void>;
  createTeam: (name: string) => Promise<Team>;
  joinTeamByCode: (code: string) => Promise<void>;
  changeMemberRole: (targetUserId: string, role: RoleType) => Promise<void>;
  banMember: (targetUserId: string, reason?: string) => Promise<void>;
  leaveTeam: (teamId: string) => Promise<void>;
  deleteTeam: (teamId: string) => Promise<void>;
  startMeeting: () => Promise<Meeting>;
  joinMeeting: (meetingId: string) => Promise<void>;
  leaveMeeting: () => Promise<void>;
  endMeeting: () => Promise<void>;
  grantDrawPermission: (targetUserId: string, canDraw: boolean) => Promise<void>;
  sendMessage: (content: string) => Promise<void>;
  onDiagramSignalREvent: (handler: (type: string, data: unknown) => void) => () => void;
}

const WorkspaceContext = createContext<WorkspaceContextType | undefined>(undefined);

export const WorkspaceProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, isAuthenticated } = useAuth();
  const [teams, setTeams] = useState<Team[]>([]);
  const [activeTeam, setActiveTeam] = useState<Team | null>(null);
  const [activeTeamMembers, setActiveTeamMembers] = useState<TeamMember[]>([]);
  const [activeMeeting, setActiveMeeting] = useState<Meeting | null>(null);
  const [activeParticipants, setActiveParticipants] = useState<MeetingParticipant[]>([]);
  const [canCurrentUserDraw, setCanCurrentUserDraw] = useState(false);
  const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
  const [signalRStatus, setSignalRStatus] = useState<SignalRStatus>('disconnected');
  const [isLoadingTeams, setIsLoadingTeams] = useState(false);
  const [isLoadingMeeting, setIsLoadingMeeting] = useState(false);
  const [unreadChatCount, setUnreadChatCount] = useState(0);

  const diagramListenersRef = useRef<Array<(type: string, data: unknown) => void>>([]);

  const onDiagramSignalREvent = useCallback((handler: (type: string, data: unknown) => void) => {
    diagramListenersRef.current.push(handler);
    return () => {
      diagramListenersRef.current = diagramListenersRef.current.filter((h) => h !== handler);
    };
  }, []);

  const dispatchDiagramEvent = useCallback((type: string, data: unknown) => {
    diagramListenersRef.current.forEach((handler) => {
      try {
        handler(type, data);
      } catch (err) {
        console.error('Error in diagram event listener:', err);
      }
    });
  }, []);

  const resetUnreadChat = () => setUnreadChatCount(0);

  // Load user teams on auth
  const refreshTeams = useCallback(async () => {
    if (!isAuthenticated) {
      setTeams([]);
      setActiveTeam(null);
      return;
    }
    setIsLoadingTeams(true);
    try {
      const list = await ApiService.getMyTeams();
      setTeams(list);
      if (activeTeam) {
        const updated = list.find((t) => t.id === activeTeam.id);
        if (updated) setActiveTeam(updated);
      } else if (list.length > 0) {
        setActiveTeam(list[0]);
      }
    } catch (err) {
      console.error('Failed to load teams:', err);
    } finally {
      setIsLoadingTeams(false);
    }
  }, [isAuthenticated, activeTeam]);

  useEffect(() => {
    if (isAuthenticated) {
      refreshTeams();
    } else {
      setTeams([]);
      setActiveTeam(null);
      setActiveMeeting(null);
    }
  }, [isAuthenticated]);

  // Load team members and active meeting when activeTeam changes
  const refreshMembers = useCallback(async () => {
    if (!activeTeam) {
      setActiveTeamMembers([]);
      return;
    }
    try {
      const members = await ApiService.getTeamMembers(activeTeam.id);
      setActiveTeamMembers(members);
    } catch (err) {
      console.error('Failed to load team members:', err);
    }
  }, [activeTeam]);

  const checkActiveMeeting = useCallback(async (teamId: string) => {
    try {
      setIsLoadingMeeting(true);
      const meeting = await ApiService.getActiveMeeting(teamId);
      if (meeting) {
        setActiveMeeting(meeting);
      } else {
        setActiveMeeting(null);
      }
    } catch (err) {
      console.error('Check active meeting error:', err);
    } finally {
      setIsLoadingMeeting(false);
    }
  }, []);

  useEffect(() => {
    if (activeTeam) {
      refreshMembers();
      checkActiveMeeting(activeTeam.id);
    } else {
      setActiveTeamMembers([]);
      setActiveMeeting(null);
    }
  }, [activeTeam, refreshMembers, checkActiveMeeting]);

  // Refresh participants
  const refreshParticipants = useCallback(async (meetingId: string) => {
    try {
      const list = await ApiService.getMeetingParticipants(meetingId);
      setActiveParticipants(list);
      if (user) {
        const me = list.find((p) => p.userId.toLowerCase() === user.idUser.toLowerCase());
        if (me) {
          setCanCurrentUserDraw(me.canDraw);
        }
      }
    } catch (err) {
      console.error('Failed to load participants:', err);
    }
  }, [user]);

  // Connect SignalR and fetch initial messages when activeMeeting is set
  useEffect(() => {
    if (!activeMeeting) {
      SignalRService.disconnect();
      setSignalRStatus('disconnected');
      setChatMessages([]);
      setActiveParticipants([]);
      setCanCurrentUserDraw(false);
      return;
    }

    const meetingId = activeMeeting.id;

    // Load initial messages and participants
    const initMeeting = async () => {
      try {
        const [msgs, parts] = await Promise.all([
          ApiService.getMessages(meetingId),
          ApiService.getMeetingParticipants(meetingId),
        ]);
        setChatMessages(msgs);
        setActiveParticipants(parts);
        if (user) {
          const me = parts.find((p) => p.userId.toLowerCase() === user.idUser.toLowerCase());
          if (me) {
            setCanCurrentUserDraw(me.canDraw);
          }
        }
      } catch (err) {
        console.error('Failed to initialize meeting data:', err);
      }
    };

    initMeeting();

    SignalRService.setHandlers({
      onStatusChange: (status) => setSignalRStatus(status),
      onChatMessageReceived: (id, senderId, username, content, sentAt) => {
        setChatMessages((prev) => [
          ...prev,
          { id, senderId, senderUsername: username, content, sentAt },
        ]);
        setUnreadChatCount((prev) => prev + 1);
      },
      onDrawPermissionChanged: (targetUserId, canDraw) => {
        setActiveParticipants((prev) =>
          prev.map((p) => (p.userId.toLowerCase() === targetUserId.toLowerCase() ? { ...p, canDraw } : p))
        );
        if (user && user.idUser.toLowerCase() === targetUserId.toLowerCase()) {
          setCanCurrentUserDraw(canDraw);
        }
      },
      onParticipantsUpdated: (mId) => {
        if (mId === meetingId) {
          refreshParticipants(meetingId);
        }
      },
      onMeetingEnded: (mId) => {
        if (mId === meetingId) {
          setActiveMeeting(null);
          SignalRService.disconnect();
        }
      },
      onClassBoxAdded: (id, x1, y1, x2, y2, attributes, methods) => {
        dispatchDiagramEvent('ClassBoxAdded', { id, x1, y1, x2, y2, attributes, methods });
      },
      onElementMoved: (elementId, dx, dy) => {
        dispatchDiagramEvent('ElementMoved', { elementId, dx, dy });
      },
      onLineAdded: (id, startBoxId, endBoxId, middleText) => {
        dispatchDiagramEvent('LineAdded', { id, startBoxId, endBoxId, middleText });
      },
      onElementDeleted: (elementId) => {
        dispatchDiagramEvent('ElementDeleted', { elementId });
      },
      onBoardCleared: (boardId) => {
        dispatchDiagramEvent('BoardCleared', { boardId });
      },
    });

    SignalRService.connect(meetingId).catch((err) => {
      console.error('SignalR connect error:', err);
    });

    return () => {
      SignalRService.disconnect();
    };
  }, [activeMeeting, user, refreshParticipants, dispatchDiagramEvent]);

  // Helper getters
  const isCurrentUserOrganizerOrOwner = Boolean(
    user &&
      activeTeam &&
      (activeTeam.ownerId.toLowerCase() === user.idUser.toLowerCase() ||
        activeTeamMembers.some(
          (m) =>
            m.userId.toLowerCase() === user.idUser.toLowerCase() &&
            (m.role === 'Owner' || m.role === 'Organizer')
        ))
  );

  const selectTeam = async (team: Team | null) => {
    setActiveTeam(team);
  };

  const createTeam = async (name: string) => {
    const created = await ApiService.createTeam(name);
    await refreshTeams();
    setActiveTeam(created);
    return created;
  };

  const joinTeamByCode = async (code: string) => {
    await ApiService.joinTeamByCode(code);
    await refreshTeams();
  };

  const changeMemberRole = async (targetUserId: string, role: RoleType) => {
    if (!activeTeam) return;
    await ApiService.changeMemberRole(activeTeam.id, targetUserId, role);
    await refreshMembers();
  };

  const banMember = async (targetUserId: string, reason?: string) => {
    if (!activeTeam) return;
    await ApiService.banMember(activeTeam.id, targetUserId, reason);
    await refreshMembers();
  };

  const leaveTeam = async (teamId: string) => {
    await ApiService.leaveTeam(teamId);
    await refreshTeams();
  };

  const deleteTeam = async (teamId: string) => {
    await ApiService.deleteTeam(teamId);
    await refreshTeams();
  };

  const startMeeting = async () => {
    if (!activeTeam) throw new Error('Izaberite tim pre pokretanja sastanka');
    const meeting = await ApiService.createMeeting(activeTeam.id);
    setActiveMeeting(meeting);
    return meeting;
  };

  const joinMeeting = async (meetingId: string) => {
    await ApiService.joinMeeting(meetingId);
    if (activeTeam) {
      await checkActiveMeeting(activeTeam.id);
    }
  };

  const leaveMeeting = async () => {
    if (!activeMeeting) return;
    await ApiService.leaveMeeting(activeMeeting.id);
    setActiveMeeting(null);
  };

  const endMeeting = async () => {
    if (!activeMeeting) return;
    await ApiService.endMeeting(activeMeeting.id);
    setActiveMeeting(null);
  };

  const grantDrawPermission = async (targetUserId: string, canDraw: boolean) => {
    if (!activeMeeting) return;
    await ApiService.grantDraw(activeMeeting.id, targetUserId, canDraw);
  };

  const sendMessage = async (content: string) => {
    if (!activeMeeting || !content.trim()) return;
    await ApiService.sendMessage(activeMeeting.id, content.trim());
  };

  return (
    <WorkspaceContext.Provider
      value={{
        teams,
        activeTeam,
        activeTeamMembers,
        activeMeeting,
        activeParticipants,
        canCurrentUserDraw,
        isCurrentUserOrganizerOrOwner,
        chatMessages,
        signalRStatus,
        isLoadingTeams,
        isLoadingMeeting,
        unreadChatCount,
        resetUnreadChat,
        selectTeam,
        refreshTeams,
        refreshMembers,
        createTeam,
        joinTeamByCode,
        changeMemberRole,
        banMember,
        leaveTeam,
        deleteTeam,
        startMeeting,
        joinMeeting,
        leaveMeeting,
        endMeeting,
        grantDrawPermission,
        sendMessage,
        onDiagramSignalREvent,
      }}
    >
      {children}
    </WorkspaceContext.Provider>
  );
};

export const useWorkspace = (): WorkspaceContextType => {
  const context = useContext(WorkspaceContext);
  if (!context) {
    throw new Error('useWorkspace must be used within a WorkspaceProvider');
  }
  return context;
};
