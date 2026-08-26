import React, { useState } from 'react';
import { ApiService } from '../../services/api';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Modal } from '../common/Modal';
import { Badge } from '../common/Badge';
import { CheckCircle2, XCircle, RefreshCw, Server } from 'lucide-react';

export interface ApiSettingsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ApiSettingsModal: React.FC<ApiSettingsModalProps> = ({ isOpen, onClose }) => {
  const [url, setUrl] = useState(ApiService.getBaseUrl());
  const [isTesting, setIsTesting] = useState(false);
  const [testResult, setTestResult] = useState<{
    success: boolean;
    message: string;
  } | null>(null);

  const handleTestConnection = async () => {
    setIsTesting(true);
    setTestResult(null);
    try {
      let clean = url.trim();
      if (clean.endsWith('/')) clean = clean.slice(0, -1);
      
      const res = await fetch(`${clean}/swagger/v1/swagger.json`, {
        method: 'GET',
        headers: { Accept: 'application/json' },
      });
      if (res.ok) {
        setTestResult({
          success: true,
          message: 'Uspešno povezivanje sa GalaxyUML API serverom (v1).',
        });
      } else {
        setTestResult({
          success: false,
          message: `Server je odgovorio sa statusom ${res.status}: ${res.statusText}`,
        });
      }
    } catch (err) {
      setTestResult({
        success: false,
        message: `Neuspelo povezivanje: ${err instanceof Error ? err.message : String(err)}`,
      });
    } finally {
      setIsTesting(false);
    }
  };

  const handleSave = () => {
    ApiService.setBaseUrl(url);
    onClose();
    window.location.reload();
  };

  const handleReset = () => {
    setUrl('http://localhost:5248');
    setTestResult(null);
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={
        <div className="flex items-center gap-2">
          <Server className="h-4 w-4 text-blue-600" />
          <span>Podešavanje API Endpointa</span>
        </div>
      }
      description="Konfigurišite adresu pozadinskog ASP.NET Core servisa za GalaxyUML."
      maxWidth="lg"
    >
      <div className="space-y-4">
        <Input
          label="API Base URL:"
          value={url}
          onChange={(e) => {
            setUrl(e.target.value);
            setTestResult(null);
          }}
          placeholder="http://localhost:5248"
          isMonospace
          helperText="Podrazumevani lokalni port za ASP.NET Core API je 5248."
        />

        {testResult && (
          <div
            className={`p-3 rounded-md border text-xs flex items-start gap-2 ${
              testResult.success
                ? 'bg-emerald-50 border-emerald-200 text-emerald-800'
                : 'bg-rose-50 border-rose-200 text-rose-800'
            }`}
          >
            {testResult.success ? (
              <CheckCircle2 className="h-4 w-4 shrink-0 text-emerald-600 mt-0.5" />
            ) : (
              <XCircle className="h-4 w-4 shrink-0 text-rose-600 mt-0.5" />
            )}
            <div>
              <p className="font-semibold">{testResult.success ? 'Povezan!' : 'Greška u konekciji'}</p>
              <p className="mt-0.5 text-[11px] opacity-90">{testResult.message}</p>
            </div>
          </div>
        )}

        <div className="pt-3 border-t border-slate-200 flex flex-wrap items-center justify-between gap-2.5">
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handleTestConnection}
              isLoading={isTesting}
              icon={<RefreshCw className="h-3.5 w-3.5" />}
            >
              Testiraj konekciju
            </Button>
            <Button variant="ghost" size="sm" onClick={handleReset}>
              Vrati na 5248
            </Button>
          </div>

          <div className="flex items-center gap-2">
            <Button variant="outline" size="sm" onClick={onClose}>
              Otkaži
            </Button>
            <Button variant="primary" size="sm" onClick={handleSave}>
              Sačuvaj i osveži
            </Button>
          </div>
        </div>
      </div>
    </Modal>
  );
};
