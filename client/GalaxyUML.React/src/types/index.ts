// Domain & API Type Definitions for GalaxyUML

export type RoleType = 'Owner' | 'Organizer' | 'Member';

export interface User {
  idUser: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthSession {
  token: string;
  user: User;
}

export interface TeamMember {
  userId: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: RoleType;
  joinedAt: string;
}

export interface BannedUser {
  userId: string;
  bannedAt: string;
  reason?: string | null;
}

export interface Team {
  id: string;
  teamName: string;
  teamCode: string;
  ownerId: string;
  memberCount?: number;
  members?: Array<{ userId: string; role: RoleType; joinedAt: string }>;
  bans?: BannedUser[];
}

export interface MeetingParticipant {
  userId: string;
  username: string;
  role: string;
  canDraw: boolean;
  joinedAt: string;
}

export interface ChatMessage {
  id: string;
  senderId: string;
  senderUsername: string;
  content: string;
  sentAt: string;
}

export interface ClassBoxItem {
  id: string;
  x1: number;
  y1: number;
  x2: number;
  y2: number;
  attributes: string[];
  methods: string[];
}

export type PortSide = 'top' | 'right' | 'bottom' | 'left';

export interface LineItem {
  id: string;
  startBoxId: string;
  endBoxId: string;
  x1?: number;
  y1?: number;
  x2?: number;
  y2?: number;
  middleText?: string | null;
  text1?: string | null;
  text2?: string | null;
}

export interface TextItem {
  id: string;
  x1: number;
  y1: number;
  x2: number;
  y2: number;
  content: string;
  fontSize: number;
  color: string;
  format?: string | null;
}

export interface BoxItem {
  id: string;
  x1: number;
  y1: number;
  x2: number;
  y2: number;
}

export interface BoardElementsResponse {
  boxes: BoxItem[];
  classBoxes: ClassBoxItem[];
  texts: TextItem[];
  lines: LineItem[];
}

export interface Meeting {
  id: string;
  teamId: string;
  organizedBy: string;
  board?: {
    id: string;
    objectType: string;
    x1: number;
    y1: number;
    x2: number;
    y2: number;
    children?: unknown[];
  };
  boardId?: string;
  startedAtUtc?: string;
  isActive?: boolean;
  participants?: MeetingParticipant[];
  chat?: {
    messages: ChatMessage[];
  };
}

// Canvas & UI State Types
export type ToolMode = 'select' | 'classBox' | 'line' | 'text' | 'pan';

export interface CanvasTransform {
  x: number;
  y: number;
  scale: number;
}

export interface SelectionBox {
  x: number;
  y: number;
  width: number;
  height: number;
}

export type SignalRStatus = 'disconnected' | 'connecting' | 'connected' | 'reconnecting' | 'error';
