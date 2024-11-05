import { useEffect } from "react";
import { Button } from "@/components/ui/button";
import { IPortkeyProvider, MethodsBase } from "@portkey/provider-types";
import detectProvider from "@portkey/detect-provider";
import { useNavigate } from "react-router-dom";
import "./header.scss";
import { toast } from "react-toastify";

const Header = ({
  isConnected,
  currentWalletAddress,
  setIsConnected,
  setCurrentWalletAddress,
  provider,
  setProvider,
}: {
  isConnected: boolean;
  currentWalletAddress: string | undefined;
  setIsConnected: (val: boolean) => void;
  setCurrentWalletAddress: (val: string) => void;
  provider: IPortkeyProvider | null;
  setProvider: (p: IPortkeyProvider | null) => void;
}) => {
  const init = async () => {
    try {
      const walletProvider = await detectProvider({ providerName: "Portkey" });
      setProvider(walletProvider);
      if (walletProvider) {
        setIsConnected(walletProvider.isConnected());
      }
      try {
        //Fetch Accounts
        const accounts: { AELF: string[] } | undefined =
          await walletProvider?.request({
            method: MethodsBase.ACCOUNTS,
          });
        if (!accounts) throw new Error("No accounts");

        const account = accounts?.AELF[0];

        if (!account) throw new Error("No account");

        connect(walletProvider as IPortkeyProvider);
      } catch (error) {
        console.error(error, "===error");
      }
    } catch (error) {
      console.log(error, "=====error");
    }
  };

  // Step E - Connect Portkey Wallet
  const connect = async (walletProvider?: IPortkeyProvider) => {};

  useEffect(() => {
    if (!provider) init();
  }, [provider]);

  const navigate = useNavigate();

  return (
    <header className="app-navbar">
      <div className="container">
        <img
          src="/aelf_logo.svg"
          className="logo-image"
          alt="Aelf Logo"
          onClick={() => navigate("/")}
        />
        <div className="right-wrapper">
          <Button onClick={() => connect()}>
            {isConnected
              ? currentWalletAddress?.slice(0, 5) +
                "....." +
                currentWalletAddress?.slice(-5)
              : "Connect Wallet"}
          </Button>
        </div>
      </div>
    </header>
  );
};

export default Header;
