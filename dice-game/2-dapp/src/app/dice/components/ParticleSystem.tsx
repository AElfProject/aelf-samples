import React from 'react';

interface ParticleSystemProps {
  active: boolean;
  position?: { x: number; y: number };
}

const ParticleSystem: React.FC<ParticleSystemProps> = ({
  active,
  position,
}) => {
  return (
    <div
      className={`particle-container ${active ? 'active' : ''}`}
      style={
        position
          ? ({
              '--spawn-x': `${position.x}px`,
              '--spawn-y': `${position.y}px`,
            } as React.CSSProperties)
          : undefined
      }
    />
  );
};

export default ParticleSystem;
