import {
  AuthSession,
  BoardElementsResponse,
  ChatMessage,
  Meeting,
  MeetingParticipant,
  RoleType,
  Team,
  TeamMember,
} from '../types';

const API_STORAGE_KEY = 'galaxyuml_api_url';
const TOKEN_STORAGE_KEY = 'galaxyuml_jwt_token';
const USER_STORAGE_KEY = 'galaxyuml_user';

export class ApiService {
  private static baseUrl: string = localStorage.getItem(API_STORAGE_KEY) || 'http://localhost:5248';

  public static getBaseUrl(): string {
    return this.baseUrl;
  }

  public static setBaseUrl(url: string) {
    let clean = url.trim();
    if (clean.endsWith('/')) {
      clean = clean.slice(0, -1);
    }
    this.baseUrl = clean;
    localStorage.setItem(API_STORAGE_KEY, clean);
  }

  public static getToken(): string | null {
    return localStorage.getItem(TOKEN_STORAGE_KEY);
  }

  public static setToken(token: string | null) {
    if (token) {
      localStorage.setItem(TOKEN_STORAGE_KEY, token);
    } else {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
    }
  }

  public static getStoredUser() {
    const raw = localStorage.getItem(USER_STORAGE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  }

  public static setStoredUser(user: unknown) {
    if (user) {
      localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(USER_STORAGE_KEY);
    }
  }

  private static async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const token = this.getToken();
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      Accept: 'application/json',
      ...(options.headers as Record<string, string>),
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const url = `${this.baseUrl}${endpoint.startsWith('/') ? endpoint : `/${endpoint}`}`;
    
    let res: Response;
    try {
      res = await fetch(url, {
        ...options,
        headers,
      });
    } catch (err) {
      throw new Error(`Mrežna greška pri povezivanju na ${this.baseUrl}: ${err instanceof Error ? err.message : String(err)}`);
    }

    if (res.status === 204) {
      return null as T;
    }

    const text = await res.text();
    let data: unknown;
    try {
      data = text ? JSON.parse(text) : null;
    } catch {
      data = text;
    }

    if (!res.ok) {
      const errorMsg =
        typeof data === 'object' && data && 'error' in data
          ? (data as { error: string }).error
          : typeof data === 'string' && data
          ? data
          : `HTTP Greška ${res.status}: ${res.statusText}`;
      throw new Error(errorMsg);
    }

    return data as T;
  }

  // --- AUTH ---
  public static async login(dto: { username: string; password: string }): Promise<AuthSession> {
    const res = await this.request<AuthSession>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(dto),
    });
    if (res.token) {
      this.setToken(res.token);
      this.setStoredUser(res.user);
    }
    return res;
  }

  public static async register(dto: {
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    password: string;
  }): Promise<string> {
    return await this.request<string>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(dto),
    });
  }

  public static logout() {
    this.setToken(null);
    this.setStoredUser(null);
  }

  // --- TEAMS ---
  public static async getMyTeams(): Promise<Team[]> {
    return await this.request<Team[]>('/api/teams/me');
  }

  public static async getTeamMembers(teamId: string): Promise<TeamMember[]> {
    return await this.request<TeamMember[]>(`/api/teams/${teamId}/members`);
  }

  public static async createTeam(teamName: string): Promise<Team> {
    return await this.request<Team>('/api/teams', {
      method: 'POST',
      body: JSON.stringify({ teamName }),
    });
  }

  public static async joinTeamByCode(joinCode: string): Promise<void> {
    await this.request<void>('/api/teams/join-by-code', {
      method: 'POST',
      body: JSON.stringify({ joinCode: joinCode.trim().toUpperCase() }),
    });
  }

  public static async findTeamByCode(code: string): Promise<Team> {
    return await this.request<Team>(`/api/teams/by-code/${encodeURIComponent(code)}`);
  }

  public static async changeMemberRole(teamId: string, targetUserId: string, role: RoleType): Promise<void> {
    await this.request<void>(`/api/teams/${teamId}/role`, {
      method: 'POST',
      body: JSON.stringify({ targetUserId, role }),
    });
  }

  public static async banMember(teamId: string, targetUserId: string, reason?: string): Promise<void> {
    await this.request<void>(`/api/teams/${teamId}/ban`, {
      method: 'POST',
      body: JSON.stringify({ targetUserId, reason: reason || null }),
    });
  }

  public static async leaveTeam(teamId: string): Promise<void> {
    await this.request<void>(`/api/teams/${teamId}/leave`, {
      method: 'POST',
    });
  }

  public static async deleteTeam(teamId: string): Promise<void> {
    await this.request<void>(`/api/teams/${teamId}`, {
      method: 'DELETE',
    });
  }

  // --- MEETINGS ---
  public static normalizeMeeting(raw: unknown): Meeting | null {
    if (!raw || typeof raw !== 'object') return null;
    const m = raw as Record<string, unknown>;
    const id = String(m.id || m.meetingId || m.MeetingId || '');
    if (!id) return null;
    const boardId = String(
      m.boardId ||
      m.BoardId ||
      (m.board && typeof m.board === 'object' && (m.board as Record<string, unknown>).id) ||
      ''
    );
    return {
      ...m,
      id,
      meetingId: id,
      boardId,
      teamId: String(m.teamId || m.TeamId || ''),
      organizedBy: String(m.organizedBy || m.OrganizedBy || ''),
      startedAtUtc: String(m.startedAtUtc || m.StartedAtUtc || ''),
      isActive: m.isActive !== undefined ? Boolean(m.isActive) : true,
    } as Meeting;
  }

  public static async getActiveMeeting(teamId: string): Promise<Meeting | null> {
    const raw = await this.request<unknown>(`/api/meetings/by-team/${teamId}/active`);
    return this.normalizeMeeting(raw);
  }

  public static async createMeeting(teamId: string): Promise<Meeting> {
    const raw = await this.request<unknown>('/api/meetings', {
      method: 'POST',
      body: JSON.stringify({ teamId }),
    });
    const normalized = this.normalizeMeeting(raw);
    if (!normalized) {
      throw new Error('Neispravan odgovor servera pri kreiranju sastanka.');
    }
    return normalized;
  }

  public static async joinMeeting(meetingId: string): Promise<void> {
    await this.request<void>(`/api/meetings/${meetingId}/join`, {
      method: 'POST',
    });
  }

  public static async leaveMeeting(meetingId: string): Promise<void> {
    await this.request<void>(`/api/meetings/${meetingId}/leave`, {
      method: 'POST',
    });
  }

  public static async endMeeting(meetingId: string): Promise<void> {
    await this.request<void>(`/api/meetings/${meetingId}/end`, {
      method: 'POST',
    });
  }

  public static async getMeetingParticipants(meetingId: string): Promise<MeetingParticipant[]> {
    return await this.request<MeetingParticipant[]>(`/api/meetings/${meetingId}/participants`);
  }

  public static async grantDraw(meetingId: string, targetId: string, canDraw: boolean): Promise<void> {
    await this.request<void>(`/api/meetings/${meetingId}/grant-draw`, {
      method: 'POST',
      body: JSON.stringify({ targetId, canDraw }),
    });
  }

  public static async getMessages(meetingId: string): Promise<ChatMessage[]> {
    return await this.request<ChatMessage[]>(`/api/meetings/${meetingId}/messages`);
  }

  public static async sendMessage(meetingId: string, content: string): Promise<ChatMessage> {
    return await this.request<ChatMessage>(`/api/meetings/${meetingId}/message`, {
      method: 'POST',
      body: JSON.stringify({ content }),
    });
  }

  // --- DIAGRAM & BOARD ---
  public static async getBoardElements(boardId: string): Promise<BoardElementsResponse> {
    return await this.request<BoardElementsResponse>(`/api/diagram/${boardId}/elements`);
  }

  public static async addClassBox(
    boardId: string,
    dto: {
      x1: number;
      y1: number;
      x2: number;
      y2: number;
      attributes?: string[];
      methods?: string[];
    }
  ): Promise<{ id: string }> {
    return await this.request<{ id: string }>(`/api/diagram/${boardId}/class-box`, {
      method: 'POST',
      body: JSON.stringify({
        x1: Math.round(dto.x1),
        y1: Math.round(dto.y1),
        x2: Math.round(dto.x2),
        y2: Math.round(dto.y2),
        attributes: dto.attributes || [],
        methods: dto.methods || [],
      }),
    });
  }

  public static async addLine(
    boardId: string,
    dto: {
      startBoxId: string;
      endBoxId: string;
      middleText?: string | null;
      text1?: string | null;
      text2?: string | null;
    }
  ): Promise<{ id: string }> {
    return await this.request<{ id: string }>(`/api/diagram/${boardId}/line`, {
      method: 'POST',
      body: JSON.stringify(dto),
    });
  }

  public static async moveElement(elementId: string, dx: number, dy: number): Promise<void> {
    await this.request<void>(`/api/diagram/${elementId}/move`, {
      method: 'POST',
      body: JSON.stringify({ dx: Math.round(dx), dy: Math.round(dy) }),
    });
  }

  public static async resizeElement(elementId: string, width: number, height: number): Promise<void> {
    await this.request<void>(`/api/diagram/${elementId}/resize`, {
      method: 'POST',
      body: JSON.stringify({ width: Math.round(width), height: Math.round(height) }),
    });
  }

  public static async deleteElement(elementId: string): Promise<void> {
    await this.request<void>(`/api/diagram/${elementId}`, {
      method: 'DELETE',
    });
  }

  public static async clearBoard(boardId: string): Promise<void> {
    await this.request<void>(`/api/diagram/${boardId}/clear`, {
      method: 'POST',
    });
  }
}
