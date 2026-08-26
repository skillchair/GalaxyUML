import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { ApiService } from '../../services/api';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Badge } from '../common/Badge';
import { ApiSettingsModal } from '../settings/ApiSettingsModal';
import { LogIn, UserPlus, Layers, Server, ShieldCheck, ArrowRight } from 'lucide-react';

export const AuthView: React.FC = () => {
  const { login, register } = useAuth();
  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);

  // Login form state
  const [loginUsername, setLoginUsername] = useState('');
  const [loginPassword, setLoginPassword] = useState('');
  const [loginError, setLoginError] = useState<string | null>(null);
  const [isLoggingIn, setIsLoggingIn] = useState(false);

  // Register form state
  const [regFirstName, setRegFirstName] = useState('');
  const [regLastName, setRegLastName] = useState('');
  const [regUsername, setRegUsername] = useState('');
  const [regEmail, setRegEmail] = useState('');
  const [regPassword, setRegPassword] = useState('');
  const [regError, setRegError] = useState<string | null>(null);
  const [regSuccess, setRegSuccess] = useState<string | null>(null);
  const [isRegistering, setIsRegistering] = useState(false);

  const handleLoginSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoginError(null);
    if (!loginUsername.trim() || !loginPassword) {
      setLoginError('Unesite korisničko ime i lozinku.');
      return;
    }

    setIsLoggingIn(true);
    try {
      await login({
        username: loginUsername.trim(),
        password: loginPassword,
      });
    } catch (err) {
      setLoginError(err instanceof Error ? err.message : 'Neuspešna prijava na sistem.');
    } finally {
      setIsLoggingIn(false);
    }
  };

  const handleRegisterSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setRegError(null);
    setRegSuccess(null);

    if (
      !regFirstName.trim() ||
      !regLastName.trim() ||
      !regUsername.trim() ||
      !regEmail.trim() ||
      !regPassword
    ) {
      setRegError('Sva polja su obavezna.');
      return;
    }

    if (regPassword.length < 8) {
      setRegError('Lozinka mora imati najmanje 8 karaktera.');
      return;
    }

    setIsRegistering(true);
    try {
      await register({
        firstName: regFirstName.trim(),
        lastName: regLastName.trim(),
        username: regUsername.trim(),
        email: regEmail.trim(),
        password: regPassword,
      });
      setRegSuccess('Nalog je uspešno kreiran! Možete se prijaviti.');
      setActiveTab('login');
      setLoginUsername(regUsername.trim());
      setLoginPassword(regPassword);
    } catch (err) {
      setRegError(err instanceof Error ? err.message : 'Neuspešna registracija.');
    } finally {
      setIsRegistering(false);
    }
  };

  return (
    <div className="h-full w-full flex flex-col items-center justify-center p-6 bg-slate-100/70 relative overflow-y-auto">
      {/* Top Header info */}
      {/* Top Header Logo */}
      <div className="w-full max-w-md mb-4 text-center">
        <div className="inline-flex items-center justify-center gap-2.5 px-3 py-1 bg-white border border-slate-300 rounded-lg shadow-xs">
          <div className="h-5 w-5 bg-slate-900 rounded flex items-center justify-center">
            <Layers className="h-3.5 w-3.5 text-blue-400" />
          </div>
          <span className="font-bold text-sm tracking-tight text-slate-900">GalaxyUML</span>
          <span className="text-[11px] font-mono text-slate-400 border-l border-slate-200 pl-2">
            v1.0.0
          </span>
        </div>
      </div>

      {/* Main Auth Card */}
      <div className="w-full max-w-md bg-white border border-slate-300 rounded-lg shadow-sm overflow-hidden">
        {/* Navigation Tabs */}
        <div className="grid grid-cols-2 border-b border-slate-200 bg-slate-50/80">
          <button
            type="button"
            onClick={() => setActiveTab('login')}
            className={`py-2.5 px-4 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors border-b-2 ${
              activeTab === 'login'
                ? 'bg-white text-slate-900 border-slate-900'
                : 'text-slate-600 border-transparent hover:text-slate-900'
            }`}
          >
            <LogIn className="h-3.5 w-3.5" />
            <span>Prijava</span>
          </button>
          <button
            type="button"
            onClick={() => setActiveTab('register')}
            className={`py-2.5 px-4 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors border-b-2 ${
              activeTab === 'register'
                ? 'bg-white text-slate-900 border-slate-900'
                : 'text-slate-600 border-transparent hover:text-slate-900'
            }`}
          >
            <UserPlus className="h-3.5 w-3.5" />
            <span>Registracija</span>
          </button>
        </div>

        <div className="p-6">
          {regSuccess && (
            <div className="mb-4 p-2.5 bg-emerald-50 border border-emerald-200 text-emerald-800 rounded-md text-xs">
              {regSuccess}
            </div>
          )}

          {activeTab === 'login' ? (
            <form onSubmit={handleLoginSubmit} className="space-y-4">
              <Input
                label="Korisničko ime:"
                placeholder="npr. petar.petrovic"
                value={loginUsername}
                onChange={(e) => setLoginUsername(e.target.value)}
                autoFocus
              />

              <Input
                label="Lozinka:"
                type="password"
                placeholder="••••••••"
                value={loginPassword}
                onChange={(e) => setLoginPassword(e.target.value)}
              />

              {loginError && (
                <div className="p-2.5 bg-rose-50 border border-rose-200 text-rose-700 rounded-md text-xs">
                  {loginError}
                </div>
              )}

              <Button
                type="submit"
                variant="primary"
                className="w-full h-9"
                isLoading={isLoggingIn}
                icon={<ArrowRight className="h-4 w-4" />}
              >
                Prijavi se na sistem
              </Button>
            </form>
          ) : (
            <form onSubmit={handleRegisterSubmit} className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <Input
                  label="Ime:"
                  placeholder="Petar"
                  value={regFirstName}
                  onChange={(e) => setRegFirstName(e.target.value)}
                  autoFocus
                />
                <Input
                  label="Prezime:"
                  placeholder="Petrović"
                  value={regLastName}
                  onChange={(e) => setRegLastName(e.target.value)}
                />
              </div>

              <Input
                label="Korisničko ime:"
                placeholder="petar123"
                value={regUsername}
                onChange={(e) => setRegUsername(e.target.value)}
              />

              <Input
                label="Email adresa:"
                type="email"
                placeholder="petar@fakultet.rs"
                value={regEmail}
                onChange={(e) => setRegEmail(e.target.value)}
              />

              <Input
                label="Lozinka (min. 8 karaktera):"
                type="password"
                placeholder="••••••••"
                value={regPassword}
                onChange={(e) => setRegPassword(e.target.value)}
              />

              {regError && (
                <div className="p-2.5 bg-rose-50 border border-rose-200 text-rose-700 rounded-md text-xs">
                  {regError}
                </div>
              )}

              <Button
                type="submit"
                variant="primary"
                className="w-full h-9 mt-2"
                isLoading={isRegistering}
                icon={<UserPlus className="h-4 w-4" />}
              >
                Registruj novi nalog
              </Button>
            </form>
          )}
        </div>

        {/* Footer info bar */}
        <div className="px-6 py-3 bg-slate-50 border-t border-slate-200 flex items-center justify-between text-[11px] text-slate-500">
          <div className="flex items-center gap-1.5">
            <Server className="h-3.5 w-3.5 text-slate-400" />
            <span className="font-mono text-[10px] text-slate-600">{ApiService.getBaseUrl()}</span>
          </div>
          <button
            type="button"
            onClick={() => setIsSettingsOpen(true)}
            className="text-blue-600 hover:text-blue-800 font-semibold underline underline-offset-2"
          >
            Promeni endpoint
          </button>
        </div>
      </div>

      {/* API Config Modal */}
      <ApiSettingsModal isOpen={isSettingsOpen} onClose={() => setIsSettingsOpen(false)} />
    </div>
  );
};
