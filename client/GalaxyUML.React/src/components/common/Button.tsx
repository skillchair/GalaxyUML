import React from 'react';
import { clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  icon?: React.ReactNode;
  shortcut?: string;
  isLoading?: boolean;
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      className,
      variant = 'secondary',
      size = 'md',
      icon,
      shortcut,
      isLoading,
      disabled,
      children,
      ...props
    },
    ref
  ) => {
    const baseStyles =
      'inline-flex items-center justify-center gap-1.5 font-medium transition-all duration-100 select-none cursor-pointer focus:outline-none focus:ring-2 focus:ring-blue-500/20 active:translate-y-[1px] disabled:pointer-events-none disabled:opacity-50 whitespace-nowrap shrink-0';
    const variants = {
      primary:
        'bg-slate-900 text-white hover:bg-slate-800 active:bg-black border border-slate-900 shadow-sm',
      secondary:
        'bg-white text-slate-800 hover:bg-slate-50 hover:text-slate-900 active:bg-slate-100 border border-slate-300 shadow-xs',
      outline:
        'bg-transparent text-slate-700 hover:bg-slate-100 hover:text-slate-900 border border-slate-300',
      ghost:
        'bg-transparent text-slate-700 hover:bg-slate-100 hover:text-slate-900 border border-transparent',
      danger:
        'bg-rose-600 text-white hover:bg-rose-700 active:bg-rose-800 border border-rose-700 shadow-sm',
    };

    const sizes = {
      sm: 'h-7 px-2.5 text-xs rounded-md',
      md: 'h-8 px-3 text-xs rounded-md',
      lg: 'h-9 px-4 text-sm rounded-md',
    };

    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={twMerge(clsx(baseStyles, variants[variant], sizes[size], className))}
        {...props}
      >
        {isLoading ? (
          <span className="h-3.5 w-3.5 animate-spin rounded-full border-2 border-current border-t-transparent" />
        ) : (
          icon && <span className="shrink-0">{icon}</span>
        )}
        {children && <span>{children}</span>}
        {shortcut && (
          <kbd className="ml-1.5 rounded border border-slate-200 bg-slate-100 px-1 py-0.5 font-mono text-[10px] text-slate-500 group-hover:border-slate-300">
            {shortcut}
          </kbd>
        )}
      </button>
    );
  }
);

Button.displayName = 'Button';
