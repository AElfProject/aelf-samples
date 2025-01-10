import { useEffect } from "react";
import { Button } from "@/components/ui/button";
import { IPortkeyProvider, MethodsBase } from "@portkey/provider-types";
import detectProvider from "@portkey/detect-provider";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "./header.scss";

// This is the profile button SVG icon
const ProfileButton = ({ onClick }: { onClick: () => void }) => (
  <svg
    onClick={onClick}
    stroke="currentColor"
    fill="currentColor"
    strokeWidth="0"
    viewBox="0 0 24 24"
    height="1em"
    width="1em"
    xmlns="http://www.w3.org/2000/svg"
  >
    <g id="User">
      <g>
        <path d="M17.438,21.937H6.562a2.5,2.5,0,0,1-2.5-2.5V18.61c0-3.969,3.561-7.2,7.938-7.2s7.938,3.229,7.938,7.2v.827A2.5,2.5,0,0,1,17.438,21.937ZM12,12.412c-3.826,0-6.938,2.78-6.938,6.2v.827a1.5,1.5,0,0,0,1.5,1.5H17.438a1.5,1.5,0,0,0,1.5-1.5V18.61C18.938,15.192,15.826,12.412,12,12.412Z"></path>
        <path d="M12,9.911a3.924,3.924,0,1,1,3.923-3.924A3.927,3.927,0,0,1,12,9.911Zm0-6.847a2.924,2.924,0,1,0,2.923,2.923A2.926,2.926,0,0,0,12,3.064Z"></path>
      </g>
    </g>
  </svg>
);

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
  // Initialize the wallet provider and connect the wallet
  const init = async () => {
    try {
      // Detect the Portkey provider
      const walletProvider = await detectProvider({ providerName: "Portkey" });
      setProvider(walletProvider);

      // Check if the provider is connected
      if (walletProvider) {
        setIsConnected(walletProvider.isConnected());
      }

      try {
        // Fetch the accounts from the wallet provider
        const accounts: { AELF: string[] } | undefined = await walletProvider?.request({
          method: MethodsBase.ACCOUNTS,
        });
        
        if (!accounts) throw new Error("No accounts found");
        
        console.log("accounts", accounts);

        // Get the first account from the list of accounts
        const account = accounts?.AELF[0];
        if (!account) throw new Error("No account found");
        
        console.log("account", account);

        // Connect the wallet provider
        connect(walletProvider as IPortkeyProvider);
      } catch (error) {
        console.error("Error fetching accounts", error);
      }
    } catch (error) {
      console.error("Error initializing provider", error);
    }
  };
  
  // Step C - Connect Portkey Wallet
  const connect = async (walletProvider?: IPortkeyProvider) => {};

  // useEffect hook to initialize the provider when the component mounts
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
          {isConnected && (
            <Button className="profile-button">
              <ProfileButton onClick={() => navigate("/profile")} />
            </Button>
          )}
        </div>
      </div>
    </header>
  );
};

export default Header;
