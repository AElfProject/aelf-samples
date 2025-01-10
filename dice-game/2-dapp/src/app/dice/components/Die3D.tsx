import React from 'react';
import DiceFace from './DiceFace';

interface Die3DProps {
  value: number;
  isRolling: boolean;
  delay?: string;
  sequence: number;
}

const Die3D: React.FC<Die3DProps> = ({
  value,
  isRolling,
  delay = '0s',
  sequence,
}) => {
  // Generate random values for other faces
  const faces = {
    front: value,
    back: 7 - value, // Opposite face
    right: ((value + 1) % 6) + 1,
    left: ((value + 4) % 6) + 1,
    top: ((value + 2) % 6) + 1,
    bottom: ((value + 3) % 6) + 1,
  };

  return (
    <div className="dice-container" style={{ perspective: '1000px' }}>
      <div
        className={`dice relative size-16 cursor-pointer transition-transform duration-1000
          ${isRolling ? `roll-sequence-${sequence}` : 'hover:scale-110'}`}
        style={{
          transformStyle: 'preserve-3d',
          animationDelay: delay,
        }}
      >
        {(Object.entries(faces) as [keyof typeof faces, number][]).map(
          ([position, val]) => (
            <DiceFace key={position} position={position} value={val} />
          ),
        )}
      </div>
    </div>
  );
};

export default Die3D;
