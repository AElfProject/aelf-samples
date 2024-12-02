'use client';
import { useConnectWallet } from '@aelf-web-login/wallet-adapter-react';
import { getTxResultRetry } from '@aelf-web-login/utils';
import { message } from 'antd';
import { useEffect, useState } from 'react';
import Bignumber from 'bignumber.js';
import { DICE_GAME_CONFIG } from '../../config/config';

const { DICE_CONTRACT_ADDRESS, TOKEN_CONTRACT_ADDRESS, CHAIN_ID, EXPLORE_URL } =
  DICE_GAME_CONFIG;

export const BASE_NUMBER = 0.1;

export function useTokenMethod() {
  const { callSendMethod, callViewMethod, walletInfo } = useConnectWallet();

  // Step E - Check Get Token Alowance
  const getAllowance = async () => {};

  // Step F - Set Token Alowance
  const setAllowance = async () => {};

  return {
    getAllowance,
    setAllowance,
  };
}

export function useDiceMethods() {
  // Extract wallet and method utility functions from the wallet connection hook
  const { callSendMethod, callViewMethod, walletInfo } = useConnectWallet();
  // Extract token-related methods (e.g., setting allowance)
  const { setAllowance } = useTokenMethod();
  // State to hold player information (score and highscore)
  const [playerInfo, setPlayerInfo] = useState({
    score: 0,
    highscore: 0,
  });

  // Step G - Get Player Info From Dice Contract
  const getPlayerInfo = async () => {};

  // Step H - Fetch player information when the wallet is connected.
  useEffect(() => {}, []); // Dependency array ensures this effect runs when walletInfo changes

  // Step I - Play Game
  const play = async () => {};

  // Return the play function, getPlayerInfo function, and playerInfo state for external use
  return {
    play,
    getPlayerInfo,
    playerInfo,
  };
}
