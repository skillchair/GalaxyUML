import React, { useState, useRef, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useWorkspace } from '../../context/WorkspaceContext';
import { useCanvas } from '../../context/CanvasContext';
import { Button } from '../common/Button';
import { Badge } from '../common/Badge';
import { Input } from '../common/Input';
import { Tooltip } from '../common/Tooltip';
import {
  MessageSquare,
  Users,
  Code2,
  ChevronRight,
  ChevronLeft,
  Send,
  Shield,
  ShieldCheck,
  ShieldAlert,
  Edit2,
  Lock,
  Unlock,
  Check,
  Copy,
  Terminal,
  Trash2,
  Video,
} from 'lucide-react';

export interface RightDockProps {
  isCollapsed: boolean;
  onToggleCollapse: () => void;
}

export const RightDock: React.FC<RightDockProps> = ({ isCollapsed, onToggleCollapse }) => {
  const { user } = useAuth();
  const {
    activeMeeting,
    activeParticipants,
    canCurrentUserDraw,
    isCurrentUserOrganizerOrOwner,
    chatMessages,
    sendMessage,
    grantDrawPermission,
    endMeeting,
    leaveMeeting,
    unreadChatCount,
    resetUnreadChat,
  } = useWorkspace();

  const { classBoxes, selectedIds } = useCanvas();

  const [activeTab, setActiveTab] = useState<'meeting' | 'chat' | 'inspector'>('chat');
  const [chatInput, setChatInput] = useState('');
  const [isSending, setIsSending] = useState(false);
  const [codeLang, setCodeLang] = useState<'csharp' | 'typescript' | 'python'>('csharp');
  const [copiedCode, setCopiedCode] = useState(false);

  const chatEndRef = useRef<HTMLDivElement>(null);

  // Auto-scroll chat on new message
  useEffect(() => {
    if (activeTab === 'chat') {
      resetUnreadChat();
      chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }
  }, [chatMessages, activeTab, resetUnreadChat]);

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!chatInput.trim() || isSending) return;

    const content = chatInput.trim();
    setChatInput('');
    setIsSending(true);
    try {
      await sendMessage(content);
    } catch (err) {
      console.error('Failed to send chat message:', err);
    } finally {
      setIsSending(false);
    }
  };

  // Selected Class Box for Inspector
  const selectedBox = classBoxes.find((b) => selectedIds.includes(b.id));

  // Generate code snippet from selected UML class box
  const generateCode = () => {
    if (!selectedBox) return '// Izaberite klasu na tabli za prikaz koda.';

    const title = selectedBox.attributes[0] || 'MyClass';
    const attrs = selectedBox.attributes.slice(1);
    const methods = selectedBox.methods || [];

    if (codeLang === 'csharp') {
      let code = `namespace GalaxyUML.Models;\n\npublic class ${title}\n{\n`;
      attrs.forEach((a) => {
        const clean = a.replace(/^[+\-#~]\s*/, '');
        const parts = clean.split(':');
        const name = parts[0]?.trim() || 'Property';
        const type = parts[1]?.trim() || 'string';
        const isPublic = a.startsWith('+');
        const access = isPublic ? 'public' : 'private';
        code += `    ${access} ${type} ${name.charAt(0).toUpperCase() + name.slice(1)} { get; set; }\n`;
      });

      if (attrs.length > 0 && methods.length > 0) code += '\n';

      methods.forEach((m) => {
        const clean = m.replace(/^[+\-#~]\s*/, '');
        code += `    public void ${clean.replace(/[();]/g, '')}()\n    {\n        // TODO: implement\n    }\n\n`;
      });
      code += '}';
      return code;
    } else if (codeLang === 'typescript') {
      let code = `export interface ${title} {\n`;
      attrs.forEach((a) => {
        const clean = a.replace(/^[+\-#~]\s*/, '');
        const parts = clean.split(':');
        const name = parts[0]?.trim() || 'property';
        const type = parts[1]?.trim() || 'string';
        code += `  ${name}: ${type};\n`;
      });
      code += '}\n\n';
      code += `export class ${title}Class implements ${title} {\n`;
      attrs.forEach((a) => {
        const clean = a.replace(/^[+\-#~]\s*/, '');
        const parts = clean.split(':');
        const name = parts[0]?.trim() || 'property';
        const type = parts[1]?.trim() || 'string';
        code += `  public ${name}!: ${type};\n`;
      });
      code += '\n';
      methods.forEach((m) => {
        const clean = m.replace(/^[+\-#~]\s*/, '');
        code += `  public ${clean.replace(/[();]/g, '')}(): void {\n    // TODO: implement\n  }\n\n`;
      });
      code += '}';
      return code;
    } else {
      // Python
      let code = `class ${title}:\n    def __init__(self):\n`;
      if (attrs.length === 0) {
        code += '        pass\n';
      } else {
        attrs.forEach((a) => {
          const clean = a.replace(/^[+\-#~]\s*/, '');
          const name = clean.split(':')[0]?.trim() || 'prop';
          code += `        self.${name} = None\n`;
        });
      }
      code += '\n';
      methods.forEach((m) => {
        const clean = m.replace(/^[+\-#~]\s*/, '');
        code += `    def ${clean.replace(/[();]/g, '')}(self):\n        pass\n\n`;
      });
      return code;
    }
  };

  const handleCopyCode = () => {
    navigator.clipboard.writeText(generateCode());
    setCopiedCode(true);
    setTimeout(() => setCopiedCode(false), 2000);
  };

  return (
    <aside
      className={`h-full bg-white border-l border-slate-300 flex flex-col shrink-0 transition-all duration-200 z-20 select-none relative ${
        isCollapsed ? 'w-12' : 'w-80'
      }`}
    >
      {/* Toggle Button */}
      <button
        onClick={onToggleCollapse}
        className="absolute -left-3 top-3 h-6 w-6 bg-white border border-slate-300 rounded-full flex items-center justify-center text-slate-500 hover:text-slate-900 shadow-xs z-30 focus:outline-none cursor-pointer"
        title={isCollapsed ? 'Proširi panel' : 'Skupi panel'}
      >
        {isCollapsed ? <ChevronLeft className="h-3.5 w-3.5" /> : <ChevronRight className="h-3.5 w-3.5" />}
      </button>

      {isCollapsed ? (
        /* Collapsed Icon Bar */
        <div className="flex flex-col items-center py-3 gap-3">
          <Tooltip content="Chat Sastanka" side="left">
            <button
              onClick={() => {
                setActiveTab('chat');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors relative ${
                activeTab === 'chat'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <MessageSquare className="h-4 w-4" />
              {unreadChatCount > 0 && (
                <span className="absolute top-1 right-1 h-2 w-2 rounded-full bg-blue-500" />
              )}
            </button>
          </Tooltip>

          <Tooltip content="Učesnici Sastanka" side="left">
            <button
              onClick={() => {
                setActiveTab('meeting');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors ${
                activeTab === 'meeting'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <Users className="h-4 w-4" />
            </button>
          </Tooltip>

          <Tooltip content="Inspektor i Kod" side="left">
            <button
              onClick={() => {
                setActiveTab('inspector');
                onToggleCollapse();
              }}
              className={`h-8 w-8 rounded-md flex items-center justify-center transition-colors ${
                activeTab === 'inspector'
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
            >
              <Code2 className="h-4 w-4" />
            </button>
          </Tooltip>
        </div>
      ) : (
        /* Expanded Dock Content */
        <div className="flex flex-col h-full overflow-hidden">
          {/* Tabs Navigation */}
          <div className="grid grid-cols-3 border-b border-slate-200 bg-slate-50/70 p-1 gap-0.5 shrink-0">
            <button
              onClick={() => setActiveTab('chat')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors relative ${
                activeTab === 'chat'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <MessageSquare className="h-3 w-3" />
              <span>Chat</span>
              {unreadChatCount > 0 && (
                <span className="h-1.5 w-1.5 rounded-full bg-blue-600 shrink-0" />
              )}
            </button>

            <button
              onClick={() => setActiveTab('meeting')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors ${
                activeTab === 'meeting'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <Users className="h-3 w-3" />
              <span>Sastanak</span>
            </button>

            <button
              onClick={() => setActiveTab('inspector')}
              className={`py-1.5 px-2 text-[11px] font-semibold rounded-md flex items-center justify-center gap-1 transition-colors ${
                activeTab === 'inspector'
                  ? 'bg-white text-slate-900 shadow-2xs border border-slate-200'
                  : 'text-slate-600 hover:text-slate-900'
              }`}
            >
              <Code2 className="h-3 w-3" />
              <span>Kod</span>
            </button>
          </div>

          {/* TAB 1: REAL-TIME CHAT */}
          {activeTab === 'chat' && (
            <div className="flex-1 flex flex-col h-full overflow-hidden">
              <div className="p-2 border-b border-slate-200 bg-slate-50 text-[11px] text-slate-500 font-semibold flex items-center justify-between">
                <span>Poruke sastanka ({chatMessages.length})</span>
                {activeMeeting && (
                  <span className="font-mono text-[10px] text-slate-400">
                    ID: {(activeMeeting?.id || '').slice(0, 6)}
                  </span>
                )}
              </div>

              {/* Messages Stream */}
              <div className="flex-1 overflow-y-auto p-3 space-y-2.5">
                {!activeMeeting ? (
                  <div className="p-4 text-center text-xs text-slate-400">
                    Niste u sastanku. Povežite se sa sastankom tima da biste ćaskali.
                  </div>
                ) : chatMessages.length === 0 ? (
                  <div className="p-4 text-center text-xs text-slate-400">
                    Nema poruka. Napišite prvu poruku timu!
                  </div>
                ) : (
                  chatMessages.map((msg) => {
                    const isSelf =
                      user && msg.senderId.toLowerCase() === user.idUser.toLowerCase();

                    return (
                      <div
                        key={msg.id}
                        className={`flex flex-col ${isSelf ? 'items-end' : 'items-start'}`}
                      >
                        <div className="flex items-center gap-1.5 mb-0.5">
                          <span
                            className={`text-[10px] font-bold ${
                              isSelf ? 'text-blue-700' : 'text-slate-700'
                            }`}
                          >
                            {msg.senderUsername}
                          </span>
                          <span className="text-[9px] font-mono text-slate-400">
                            {new Date(msg.sentAt).toLocaleTimeString([], {
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </span>
                        </div>
                        <div
                          className={`max-w-[90%] px-3 py-1.5 rounded-lg text-xs break-words shadow-2xs ${
                            isSelf
                              ? 'bg-slate-900 text-white rounded-br-2xs'
                              : 'bg-slate-100 text-slate-900 border border-slate-200 rounded-bl-2xs'
                          }`}
                        >
                          {msg.content}
                        </div>
                      </div>
                    );
                  })
                )}
                <div ref={chatEndRef} />
              </div>

              {/* Input bar */}
              <form onSubmit={handleSendMessage} className="p-2 border-t border-slate-200 bg-white">
                <div className="flex gap-1.5 items-center">
                  <input
                    type="text"
                    value={chatInput}
                    onChange={(e) => setChatInput(e.target.value)}
                    placeholder={
                      activeMeeting ? 'Napišite poruku... (Enter)' : 'Povežite se sa sastankom...'
                    }
                    disabled={!activeMeeting}
                    className="flex-1 h-8 px-2.5 text-xs bg-slate-50 border border-slate-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-600 disabled:opacity-50"
                  />
                  <Button
                    type="submit"
                    variant="primary"
                    size="sm"
                    disabled={!activeMeeting || !chatInput.trim()}
                    isLoading={isSending}
                    className="h-8 px-2.5"
                  >
                    <Send className="h-3.5 w-3.5" />
                  </Button>
                </div>
              </form>
            </div>
          )}

          {/* TAB 2: LIVE MEETING ROSTER & DRAW PRIVILEGES */}
          {activeTab === 'meeting' && (
            <div className="flex-1 flex flex-col overflow-y-auto p-3 space-y-3">
              {!activeMeeting ? (
                <div className="p-4 bg-slate-50 border border-slate-200 rounded-lg text-center space-y-2">
                  <Video className="h-6 w-6 text-slate-400 mx-auto" />
                  <p className="text-xs font-semibold text-slate-700">Nema aktivnog sastanka</p>
                  <p className="text-[11px] text-slate-500">
                    Pokrenite sastanak iz levog menija da biste sarađivali.
                  </p>
                </div>
              ) : (
                <>
                  <div className="p-2.5 bg-emerald-50/70 border border-emerald-300 rounded-md space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="text-xs font-bold text-emerald-950">Aktivni učesnici</span>
                      <Badge variant="success" size="sm">
                        {activeParticipants.length} prisutan(a)
                      </Badge>
                    </div>
                    <p className="text-[10px] font-mono text-emerald-800">
                      Sastanak: {(activeMeeting?.id || '').slice(0, 12)}...
                    </p>
                  </div>

                  <div className="space-y-1.5 flex-1">
                    {activeParticipants.map((p) => {
                      const isSelf = user && p.userId.toLowerCase() === user.idUser.toLowerCase();

                      return (
                        <div
                          key={p.userId}
                          className="p-2 bg-white border border-slate-200 rounded-md text-xs space-y-1.5 shadow-2xs"
                        >
                          <div className="flex items-center justify-between">
                            <div className="flex items-center gap-1.5">
                              <span className="font-bold text-slate-900">{p.username}</span>
                              {isSelf && (
                                <span className="text-[9px] bg-slate-100 text-slate-600 px-1 py-0.2 rounded font-medium">
                                  Vi
                                </span>
                              )}
                            </div>

                            <Badge variant={p.canDraw ? 'success' : 'default'} size="sm">
                              {p.canDraw ? '✏️ Crta' : '👁️ Pregled'}
                            </Badge>
                          </div>

                          {/* Organizer controls for drawing privileges */}
                          {isCurrentUserOrganizerOrOwner && !isSelf && (
                            <div className="pt-1.5 border-t border-slate-100 flex items-center justify-between">
                              <span className="text-[10px] text-slate-500 font-medium">
                                Dozvola za crtanje:
                              </span>
                              <button
                                onClick={() => grantDrawPermission(p.userId, !p.canDraw)}
                                className={`text-[10px] font-semibold px-2 py-0.5 rounded border transition-colors flex items-center gap-1 ${
                                  p.canDraw
                                    ? 'bg-amber-50 text-amber-800 border-amber-300 hover:bg-amber-100'
                                    : 'bg-emerald-50 text-emerald-800 border-emerald-300 hover:bg-emerald-100'
                                }`}
                              >
                                {p.canDraw ? (
                                  <>
                                    <Lock className="h-2.5 w-2.5" />
                                    <span>Oduzmi</span>
                                  </>
                                ) : (
                                  <>
                                    <Unlock className="h-2.5 w-2.5" />
                                    <span>Dozvoli</span>
                                  </>
                                )}
                              </button>
                            </div>
                          )}
                        </div>
                      );
                    })}
                  </div>

                  <div className="pt-2 border-t border-slate-200 space-y-1.5 shrink-0">
                    <Button variant="outline" size="sm" className="w-full" onClick={leaveMeeting}>
                      Napusti sastanak
                    </Button>
                    {isCurrentUserOrganizerOrOwner && (
                      <Button variant="danger" size="sm" className="w-full" onClick={endMeeting}>
                        Završi sastanak za sve
                      </Button>
                    )}
                  </div>
                </>
              )}
            </div>
          )}

          {/* TAB 3: ELEMENT INSPECTOR & LIVE CODE GENERATOR */}
          {activeTab === 'inspector' && (
            <div className="flex-1 flex flex-col overflow-y-auto p-3 space-y-3">
              {selectedBox ? (
                <>
                  <div className="p-2.5 bg-slate-50 border border-slate-300 rounded-md text-xs space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="font-bold text-slate-900">
                        {selectedBox.attributes[0] || 'Klasa'}
                      </span>
                      <Badge variant="mono" size="sm">
                        UML ClassBox
                      </Badge>
                    </div>
                    <p className="text-[11px] font-mono text-slate-500">
                      Pozicija: ({selectedBox.x1}, {selectedBox.y1}) • Dimenzije:{' '}
                      {selectedBox.x2 - selectedBox.x1} × {selectedBox.y2 - selectedBox.y1}px
                    </p>
                  </div>

                  {/* Language Selector */}
                  <div className="space-y-1.5">
                    <div className="flex items-center justify-between">
                      <span className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                        Generisanje Koda
                      </span>
                      <button
                        onClick={handleCopyCode}
                        className="text-[11px] text-blue-600 hover:text-blue-800 font-semibold flex items-center gap-1"
                      >
                        {copiedCode ? (
                          <>
                            <Check className="h-3 w-3 text-emerald-600" />
                            <span>Kopirano!</span>
                          </>
                        ) : (
                          <>
                            <Copy className="h-3 w-3" />
                            <span>Kopiraj kod</span>
                          </>
                        )}
                      </button>
                    </div>

                    <div className="grid grid-cols-3 bg-slate-100 p-0.5 rounded-md border border-slate-200 text-xs font-semibold">
                      <button
                        onClick={() => setCodeLang('csharp')}
                        className={`py-1 rounded transition-colors ${
                          codeLang === 'csharp' ? 'bg-white shadow-2xs text-slate-900' : 'text-slate-500'
                        }`}
                      >
                        C#
                      </button>
                      <button
                        onClick={() => setCodeLang('typescript')}
                        className={`py-1 rounded transition-colors ${
                          codeLang === 'typescript'
                            ? 'bg-white shadow-2xs text-slate-900'
                            : 'text-slate-500'
                        }`}
                      >
                        TypeScript
                      </button>
                      <button
                        onClick={() => setCodeLang('python')}
                        className={`py-1 rounded transition-colors ${
                          codeLang === 'python' ? 'bg-white shadow-2xs text-slate-900' : 'text-slate-500'
                        }`}
                      >
                        Python
                      </button>
                    </div>

                    {/* Monospace Code Display */}
                    <div className="relative">
                      <pre className="p-3 bg-slate-900 text-slate-100 rounded-lg text-[11px] font-mono leading-relaxed overflow-x-auto border border-slate-800 max-h-72">
                        <code>{generateCode()}</code>
                      </pre>
                    </div>
                  </div>
                </>
              ) : (
                <div className="p-6 bg-slate-50 border border-slate-200 rounded-lg text-center space-y-2">
                  <Terminal className="h-6 w-6 text-slate-400 mx-auto" />
                  <p className="text-xs font-semibold text-slate-700">Nema selektovanog elementa</p>
                  <p className="text-[11px] text-slate-500">
                    Kliknite na bilo koju UML klasu na tabli da biste videli njena svojstva i automatski
                    generisali C# / TypeScript / Python kod.
                  </p>
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </aside>
  );
};
