'use client';
import React, { useCallback, useEffect, useState } from 'react';
import { RotateCcw, Zap } from 'lucide-react';
import Die3D from './components/Die3D';
import ParticleSystem from './components/ParticleSystem';
import './index.css';
import { useConnectWallet } from '@aelf-web-login/wallet-adapter-react';
import { BASE_NUMBER, useDiceMethods } from '@/app/dice/useDiceMethods';
import {
  onConnectBtnClickHandler,
  WalletConnectWithRTK,
} from '@/app/dice/Account';
import Bignumber from 'bignumber.js';
import PlayButton from '@/app/dice/components/PlayButton';

// Global variable to store player information
let playerInfoGlobal: any = {};

/**
 * Main component for the Dice Game.
 * Handles dice rolling, wallet connection, game state management, and animations.
 */
export default function DiceGame() {
  const { connectWallet, walletInfo } = useConnectWallet();
  const { play, getPlayerInfo } = useDiceMethods();
  const [isPlaying, setIsPlaying] = useState(false);

  const [dice1, setDice1] = useState(1);
  const [dice2, setDice2] = useState(1);
  const [isRolling, setIsRolling] = useState(false);
  const [message, setMessage] = useState('');
  const [sequence, setSequence] = useState(1);
  const [isShaking, setIsShaking] = useState(false);
  const [particlePosition, setParticlePosition] = useState<{
    x: number;
    y: number;
  }>();

  const [playerInfo, setPlayerInfo] = useState({
    dice1: '1',
    dice2: '1',
  });

  // Step J - Fetch player information when wallet is connected
  useEffect(() => {}, []);

   /**
   * Creates a single particle effect.
   * @param x - X position of the particle.
   * @param y - Y position of the particle.
   * @param container - Container element for particles.
   * @param type - Type of particle effect (e.g., basic, star, spark).
   */
  // Step K - Creates a single particle effect.
  const createParticle = useCallback(()=>{},[]);

  /**
   * Spawns multiple particle effects at a specified position.
   * @param x - X position for spawning particles.
   * @param y - Y position for spawning particles.
   */
  // Step L - Spawns multiple particle effects at a specified position.
  const spawnParticles = useCallback(()=>{},[]);


  /**
   * Displays the result message based on the game outcome.
   * @param isWin - Boolean indicating if the player won.
   * @param amount - Amount involved in the game.
   */
  // Step M - Displays the result message based on the game outcome.
  const handleGameResult = () => {};


  /**
   * Rolls the dice, interacts with the game contract, and handles game animations.
   * @param multiplier - Betting multiplier for the game.
   */
  // Step N - Rolls the dice
  const rollDice = async () => {};

  return (
    <div className="tron-bg min-h-screen bg-gray-900 text-teal-300">
      <div
        className={`container relative mx-auto px-4 py-8 ${isShaking ? 'screen-shake' : ''}`}
      >
        <ParticleSystem active={isRolling} position={particlePosition} />
        <div className="flex h-[100px] w-full justify-end">
          <WalletConnectWithRTK />
        </div>
        <div className="mx-auto max-w-2xl">
          <div className="mb-8 text-center">
            <h1 className="tron-glow mb-2 bg-gradient-to-r from-teal-300 to-teal-500 bg-clip-text text-4xl font-bold text-transparent">
              AELF DICE
            </h1>
            <div className="flex items-center justify-center gap-2">
              <Zap className="size-5" />
              <p className="text-teal-400">
                Initialize sequence for critical hits
              </p>
              <Zap className="size-5" />
            </div>
          </div>

          <div className="mb-8 rounded-xl border border-teal-500/30 bg-gray-900/80 p-8 shadow-[0_0_15px_rgba(79,209,197,0.3)] backdrop-blur-lg">
            <div className="mb-8 flex justify-center gap-16">
              <Die3D value={dice1} isRolling={isRolling} sequence={sequence} />
              <Die3D
                value={dice2}
                isRolling={isRolling}
                delay="0.1s"
                sequence={sequence}
              />
            </div>

            <div className="mb-6 flex justify-center gap-4">
              {walletInfo ? (
                isRolling || isPlaying ? (
                  <PlayButton disabled={true}>PROCESSING...</PlayButton>
                ) : (
                  <>
                    <PlayButton onClick={() => rollDice(1)}>
                      {BASE_NUMBER} ELF
                    </PlayButton>
                    <PlayButton onClick={() => rollDice(2)}>
                      {BASE_NUMBER * 2} ELF
                    </PlayButton>
                    <PlayButton onClick={() => rollDice(4)}>
                      {BASE_NUMBER * 4} ELF
                    </PlayButton>
                  </>
                )
              ) : (
                <>
                  <button
                    onClick={() => onConnectBtnClickHandler(connectWallet)}
                    className="flex items-center gap-2 rounded-lg border-2 border-teal-400/50 bg-gray-800/50 px-6 py-3 font-bold text-teal-300 transition-all hover:scale-105 hover:shadow-[0_0_15px_rgba(79,209,197,0.3)]"
                  >
                    <RotateCcw size={20} />
                    Login
                  </button>
                </>
              )}
            </div>

            {message && (
              <div className="tron-glow mb-4 animate-pulse text-center text-lg font-bold text-teal-300">
                {message}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
