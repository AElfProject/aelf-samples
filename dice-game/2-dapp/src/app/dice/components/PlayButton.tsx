import React from 'react';

export default function PlayButton({
  onClick = () => {},
  disabled = false,
  children,
}: {
  onClick?: any;
  disabled?: boolean;
  children: React.ReactNode;
}) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className="rounded-lg border-2 border-teal-400 bg-teal-500/20 px-6 py-3 font-bold text-teal-300 transition-all hover:scale-105 hover:shadow-[0_0_15px_rgba(79,209,197,0.5)] disabled:cursor-not-allowed disabled:opacity-50"
    >
      {children}
    </button>
  );
}
