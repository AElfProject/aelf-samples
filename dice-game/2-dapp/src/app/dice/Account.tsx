'use client';
import { useAppDispatch, useAppSelector } from '@/app/lib/reduxToolkit/hooks';
import {
  getWalletInfo,
  setWalletInfo,
} from '@/app/lib/reduxToolkit/features/walletConnect/walletConnectSlice';
import { TWalletInfo } from '@aelf-web-login/wallet-adapter-base';
import { useConnectWallet } from '@aelf-web-login/wallet-adapter-react';
import { useEffect } from 'react';

// Step B - Connect wallet handler
export const onConnectBtnClickHandler = async () => {};

// Step C - Disconnect wallet handler
export const onDisConnectBtnClickHandler = async () => {};

// Step D - Wallet info component
export const WalletConnectWithRTK = () => {};

/**
 * @description
 * If we do not format the data, we will get the error follow.
 * RTK: A non-serializable value was detected in an action;
 * @param walletInfo
 */
function formatWalletInfo(walletInfo: TWalletInfo) {
  console.log('formatWalletInfo: , ', walletInfo);
  return {
    name: walletInfo?.name || walletInfo?.extraInfo?.nickName || '-',
    address: walletInfo?.address || '-',
  };
}
