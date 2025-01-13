import { useEffect } from "react";
import { Button } from "@/components/ui/button";
import { IPortkeyProvider, MethodsBase } from "@portkey/provider-types";
import detectProvider from "@portkey/detect-provider";
import { useNavigate } from "react-router-dom";
import "./header.scss";
import { toast } from "react-toastify";
import SearchInput from "./search-input";
import { Loader } from "lucide-react";
import userBalance from "@/hooks/userBalance";

interface IProps {
  isConnected: boolean;
  currentWalletAddress?: string;
  setIsConnected: (val: boolean) => void;
  setCurrentWalletAddress: (val: string) => void;
  provider: IPortkeyProvider | null;
  setProvider: (provider: IPortkeyProvider | null) => void;
}

const Header = ({
  isConnected,
  currentWalletAddress,
  setIsConnected,
  setCurrentWalletAddress,
  provider,
  setProvider,
}: IProps) => {
  const { balance, isLoading } = userBalance(provider, currentWalletAddress);

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

  // Step 9 - Connect Portkey Wallet
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
          title="aelf"
          onClick={() => navigate("/")}
        />
        <div className="right-wrapper">
          <SearchInput />
          {currentWalletAddress && (
            <Button className="balance-btn outline">
              {isLoading ? <Loader className="loading-animation" /> : balance}{" "}
              ELF
            </Button>
          )}
          <Button
            className="connect-btn"
            onClick={() => !isConnected && connect()}
          >
            {isConnected
              ? currentWalletAddress?.slice(0, 5) +
                "....." +
                currentWalletAddress?.slice(-5)
              : "Connect Wallet"}
          </Button>
          <Button
            className="profile-button"
            onClick={() => isConnected && navigate("/user-profile")}
          >
            <img
              src="/user-profile.svg"
              className="user-profile"
              alt="User Profile"
              title="Profile"
            />
          </Button>
        </div>
      </div>
    </header>
  );
};

export default Header;
