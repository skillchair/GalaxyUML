import * as signalR from '@microsoft/signalr';
import { ApiService } from './api';
import { SignalRStatus } from '../types';

export type SignalREventHandlers = {
  onClassBoxAdded?: (
    id: string,
    x1: number,
    y1: number,
    x2: number,
    y2: number,
    attributes: string[],
    methods: string[]
  ) => void;
  onElementMoved?: (elementId: string, dx: number, dy: number) => void;
  onLineAdded?: (id: string, startBoxId: string, endBoxId: string, middleText: string | null) => void;
  onElementDeleted?: (elementId: string) => void;
  onBoardCleared?: (boardId: string) => void;
  onChatMessageReceived?: (
    id: string,
    senderId: string,
    username: string,
    content: string,
    sentAt: string
  ) => void;
  onDrawPermissionChanged?: (targetUserId: string, canDraw: boolean) => void;
  onParticipantsUpdated?: (meetingId: string) => void;
  onMeetingEnded?: (meetingId: string) => void;
  onStatusChange?: (status: SignalRStatus) => void;
};

export class SignalRService {
  private static connection: signalR.HubConnection | null = null;
  private static currentMeetingId: string | null = null;
  private static handlers: SignalREventHandlers = {};
  private static status: SignalRStatus = 'disconnected';

  public static getStatus(): SignalRStatus {
    return this.status;
  }

  public static setHandlers(handlers: SignalREventHandlers) {
    this.handlers = { ...this.handlers, ...handlers };
  }

  private static setStatus(newStatus: SignalRStatus) {
    this.status = newStatus;
    this.handlers.onStatusChange?.(newStatus);
  }

  public static async connect(meetingId: string): Promise<void> {
    if (this.connection && this.currentMeetingId === meetingId && this.status === 'connected') {
      return;
    }

    await this.disconnect();

    const baseUrl = ApiService.getBaseUrl();
    const token = ApiService.getToken();
    const hubUrl = `${baseUrl}/diagramHub`;

    this.setStatus('connecting');

    const builder = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token || '',
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount < 5) return 2000;
          return 5000;
        },
      })
      .configureLogging(signalR.LogLevel.Warning);

    const conn = builder.Build ? (builder as unknown as { Build(): signalR.HubConnection }).Build() : builder.build();
    this.connection = conn;
    this.currentMeetingId = meetingId;

    conn.onreconnecting(() => this.setStatus('reconnecting'));
    conn.onreconnected(async () => {
      this.setStatus('connected');
      if (this.currentMeetingId) {
        try {
          await conn.invoke('JoinMeeting', this.currentMeetingId);
        } catch (err) {
          console.error('SignalR re-join meeting error:', err);
        }
      }
    });
    conn.onclose(() => this.setStatus('disconnected'));

    // Register Server Event Listeners
    conn.on(
      'ClassBoxAdded',
      (
        id: string,
        x1: number,
        y1: number,
        x2: number,
        y2: number,
        attributes: string[],
        methods: string[]
      ) => {
        this.handlers.onClassBoxAdded?.(id, x1, y1, x2, y2, attributes || [], methods || []);
      }
    );

    conn.on('ElementMoved', (elementId: string, dx: number, dy: number) => {
      this.handlers.onElementMoved?.(elementId, dx, dy);
    });

    conn.on(
      'LineAdded',
      (id: string, startBoxId: string, endBoxId: string, middleText: string | null) => {
        this.handlers.onLineAdded?.(id, startBoxId, endBoxId, middleText);
      }
    );

    conn.on('ElementDeleted', (elementId: string) => {
      this.handlers.onElementDeleted?.(elementId);
    });

    conn.on('BoardCleared', (boardId: string) => {
      this.handlers.onBoardCleared?.(boardId);
    });

    conn.on(
      'ChatMessageReceived',
      (id: string, senderId: string, username: string, content: string, sentAt: string) => {
        this.handlers.onChatMessageReceived?.(id, senderId, username, content, sentAt);
      }
    );

    conn.on('DrawPermissionChanged', (targetUserId: string, canDraw: boolean) => {
      this.handlers.onDrawPermissionChanged?.(targetUserId, canDraw);
    });

    conn.on('ParticipantsUpdated', (mId: string) => {
      this.handlers.onParticipantsUpdated?.(mId);
    });

    conn.on('MeetingEnded', (mId: string) => {
      this.handlers.onMeetingEnded?.(mId);
    });

    try {
      await conn.start();
      await conn.invoke('JoinMeeting', meetingId);
      this.setStatus('connected');
    } catch (err) {
      console.error('Failed to start SignalR connection:', err);
      this.setStatus('error');
      throw err;
    }
  }

  public static async disconnect(): Promise<void> {
    if (this.connection) {
      if (this.currentMeetingId && this.status === 'connected') {
        try {
          await this.connection.invoke('LeaveMeeting', this.currentMeetingId);
        } catch {
          // ignore error on exit
        }
      }
      try {
        await this.connection.stop();
      } catch {
        // ignore
      }
      this.connection = null;
      this.currentMeetingId = null;
      this.setStatus('disconnected');
    }
  }
}
