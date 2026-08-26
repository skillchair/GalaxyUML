import React from 'react';
import * as RadixTooltip from '@radix-ui/react-tooltip';

export interface TooltipProps {
  content: React.ReactNode;
  shortcut?: string;
  side?: 'top' | 'right' | 'bottom' | 'left';
  align?: 'start' | 'center' | 'end';
  children: React.ReactNode;
}

export const Tooltip: React.FC<TooltipProps> = ({
  content,
  shortcut,
  side = 'top',
  align = 'center',
  children,
}) => {
  return (
    <RadixTooltip.Provider delayDuration={200}>
      <RadixTooltip.Root>
        <RadixTooltip.Trigger asChild>{children}</RadixTooltip.Trigger>
        <RadixTooltip.Portal>
          <RadixTooltip.Content
            side={side}
            align={align}
            sideOffset={4}
            className="z-50 overflow-hidden rounded bg-slate-900 px-2 py-1 text-[11px] font-medium text-white shadow-md animate-in fade-in-0 zoom-in-95 flex items-center gap-1.5"
          >
            <span>{content}</span>
            {shortcut && (
              <kbd className="rounded bg-slate-800 px-1 py-0.2 font-mono text-[9px] text-slate-300 border border-slate-700">
                {shortcut}
              </kbd>
            )}
            <RadixTooltip.Arrow className="fill-slate-900" />
          </RadixTooltip.Content>
        </RadixTooltip.Portal>
      </RadixTooltip.Root>
    </RadixTooltip.Provider>
  );
};
