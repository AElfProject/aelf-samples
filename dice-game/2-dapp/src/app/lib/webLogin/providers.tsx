'use client';
import { WebLoginProvider, init } from '@aelf-web-login/wallet-adapter-react';
import { useEffect, useState } from 'react';
import { IBridgeAPI } from '@aelf-web-login/wallet-adapter-bridge';
export default function Provider({ children }: { children: React.ReactNode }) {
  const [bridgeAPI, setBridgeAPI] = useState<IBridgeAPI>();
  useEffect(() => {
    const initBridgeAPI = async () => {
      const { walletConnectConfig } = await import(
        '@/app/lib/walletConnectConfig'
      );
      const bridgeAPI = init(walletConnectConfig); // upper config
      setBridgeAPI(bridgeAPI);
    };
    initBridgeAPI();
  }, []);

  return (
    <>
      {bridgeAPI ? (
        <WebLoginProvider bridgeAPI={bridgeAPI}>{children}</WebLoginProvider>
      ) : (
        <></>
      )}
    </>
  );
}
