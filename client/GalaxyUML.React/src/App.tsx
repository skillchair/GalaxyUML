import React from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import { WorkspaceProvider } from './context/WorkspaceContext';
import { CanvasProvider } from './context/CanvasContext';
import { AuthView } from './components/auth/AuthView';
import { AppLayout } from './components/layout/AppLayout';
import { Layers } from 'lucide-react';

const RootContent: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="h-full w-full flex flex-col items-center justify-center bg-slate-50 text-slate-900 select-none">
        <div className="flex items-center gap-3 p-4 bg-white border border-slate-300 rounded-lg shadow-sm">
          <div className="h-7 w-7 bg-slate-900 rounded flex items-center justify-center text-white">
            <Layers className="h-4 w-4 text-blue-400 animate-pulse" />
          </div>
          <div>
            <h3 className="font-bold text-xs text-slate-900">GalaxyUML</h3>
            <p className="text-[11px] text-slate-500">Učitavanje radne stanice...</p>
          </div>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <AuthView />;
  }

  return (
    <WorkspaceProvider>
      <CanvasProvider>
        <AppLayout />
      </CanvasProvider>
    </WorkspaceProvider>
  );
};

export const App: React.FC = () => {
  return (
    <AuthProvider>
      <RootContent />
    </AuthProvider>
  );
};

export default App;
