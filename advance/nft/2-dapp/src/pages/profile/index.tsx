import { Button } from "@/components/ui/button";
import { useEffect, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";
import "./profile.scss";
import useNFTSmartContract from "@/hooks/useNFTSmartContract";
import { useNavigate } from "react-router-dom";
import { NFT_IMAGES } from "@/lib/constant";
import { toast } from "react-toastify";
import { CopyIcon } from "@/components/ui/icons";
import { fetchUserNftData } from "@/lib/commonFunctions";

type INft = {
  symbol: string;
  tokenName: string;
  amount: number | string;
  address: string;
}

const ProfilePage = ({
  provider,
  currentWalletAddress,
}: {
  provider: IPortkeyProvider | null;
  currentWalletAddress: string;
}) => {
  const [userNfts, setUserNfts] = useState<INft[]>([]);
  const [loading, setLoading] = useState(true);
  const { sideChainSmartContract } = useNFTSmartContract(provider);
  const navigate = useNavigate();

  // get NFT Data from User's wallet
  const getNFTData = async () => {};

  // Use Effect to Fetch NFTs
  useEffect(() => {
    if (currentWalletAddress && sideChainSmartContract) {
      getNFTData();
    }
  }, [currentWalletAddress, sideChainSmartContract]);

  return (
    <section className="profile-page">
      <div className="container profile-details">
        <svg width="28" height="29" viewBox="0 0 28 29" fill="none">
          <g id="portkey V2 1">
            <rect
              y="0.539062"
              width="28"
              height="28"
              rx="6"
              fill="#5D42FF"
            ></rect>
            <path
              id="Vector"
              fill-rule="evenodd"
              clip-rule="evenodd"
              d="M13.0006 8.96873C10.665 9.99582 9.19602 12.5078 10.0512 15.0041C10.5683 16.5137 11.6104 17.5894 13.1558 18.1392C15.8583 19.1008 18.6794 16.9865 18.363 14.2381C18.2286 13.07 17.6369 12.1805 16.4351 11.8125C15.5411 11.5387 14.6856 11.6666 13.9875 12.3178C13.395 12.8699 13.1704 13.5499 13.4077 14.3402C13.5387 14.7748 13.8532 15.0452 14.2812 15.1743C14.6569 15.2872 14.9475 15.1238 15.1714 14.8111C15.1262 14.7355 15.0867 14.6529 15.0318 14.5818C14.7397 14.2033 14.9766 13.6527 15.2292 13.4234C15.5725 13.1119 16.1284 13.0835 16.4937 13.3415C17.0862 13.7593 17.3556 14.8706 17.0336 15.5678C16.4948 16.734 15.1094 17.4129 13.8383 17.0826C11.8313 16.5612 10.8418 14.5747 11.4895 12.6544C12.3503 10.1028 15.2434 8.98668 17.7088 10.2105C19.7021 11.1995 20.442 12.9264 20.3625 14.982C20.2323 18.3667 16.8254 20.8776 13.4092 20.2248C10.096 19.592 7.63567 16.4116 7.81215 13.404C7.87708 12.2935 8.12371 11.2537 8.69046 10.2995C10.165 7.81821 12.4619 6.65048 15.3348 6.46159C16.0911 6.41185 16.8537 6.45412 17.5743 6.45448C17.5347 6.43691 17.4444 6.39239 17.3504 6.35724C15.9474 5.83061 14.4976 5.66266 13.0118 5.87511C12.0906 5.98883 11.2403 6.23831 10.4672 6.59513C10.453 6.59737 10.4362 6.60224 10.4157 6.61083C10.068 6.75932 10.0098 6.41821 9.88627 6.31497C9.76613 6.21473 9.7818 6.33928 9.7818 6.33928C9.88666 6.83936 9.62398 7.04096 9.58853 7.06565C9.36951 7.20144 9.15946 7.34805 8.95648 7.50216C8.95051 7.50476 8.94603 7.50476 8.9397 7.50776C8.46025 7.72694 8.26101 7.35664 8.04049 7.24407C7.80731 7.12512 7.89537 7.32037 7.89537 7.32037C8.18415 7.89563 7.9349 8.38785 7.88826 8.47089C7.68381 8.69195 7.49239 8.92422 7.3133 9.16546C7.29464 9.17743 7.27637 9.18865 7.25509 9.20772C6.78424 9.6345 6.41784 9.29675 6.12682 9.23317C5.80594 9.16359 5.98764 9.37418 5.98764 9.37418C6.58126 9.98348 6.39359 10.7162 6.39359 10.7162C5.63095 12.3683 5.35858 14.2811 5.67535 16.2006C6.71671 22.5101 14.4584 25.49 19.533 21.4255C21.181 20.1055 22.3243 17.5628 22.4449 15.4593C22.5306 13.9654 22.0698 12.6095 21.2202 11.382C19.4177 8.77722 15.815 7.73106 13.0006 8.96873Z"
              fill="white"
            ></path>
          </g>
        </svg>
        <Button
          className="header-button profile-button outline"
          onClick={() => {
            navigator.clipboard.writeText(currentWalletAddress);
            toast.success("Wallet Address Copied");
          }}
        >
          {currentWalletAddress} <CopyIcon className="copy-icon" />
        </Button>
      </div>
      <div className="container profile-nft-collection">
        <h2>Your NFT Tokens</h2>
        {currentWalletAddress ? (
          <div className="nft-collection">
            {userNfts.length > 0 ? (
              userNfts.slice(0, 5).map((data:INft, index: number) => (
                <div
                  className={
                    userNfts.length > 3 ? "nft-card around" : "nft-card"
                  }
                  key={index}
                >
                  <img src={NFT_IMAGES[index + 1]} alt={"nft- image" + index} />
                  <div className="nft-info">
                    <p>{data.symbol}</p>
                  </div>

                  <div className="nft-info-row">
                    <span>Name:</span>
                    <span>{data.tokenName}</span>
                  </div>

                  <div className="nft-info-row">
                    <span>Balance:</span>
                    <span>{data.amount}</span>
                  </div>

                  <div className="nft-info-row">
                    <span>Owner:</span>
                    <span>{data.address}</span>
                  </div>

                  <div className="buy-container">
                    <Button
                      onClick={() =>
                        navigate(
                          `/transfer-nft?nft-index=${index + 1}&nft-symbol=${data.symbol}&nft-balance=${data.amount}`
                        )
                      }
                    >
                      Transfer NFT
                    </Button>
                  </div>
                </div>
              ))
            ) : loading ? (
              <div className="bordered-container">
                <strong>Loading...</strong>
              </div>
            ) : (
              <div className="bordered-container">
                <strong>
                  It's Look like you don't have any NFT on your wallet
                </strong>
              </div>
            )}
          </div>
        ) : (
          <div className="bordered-container">
            <strong>
              Please connect your Portkey Wallet and Create a new NFT Collection
              and NFT Tokens
            </strong>
          </div>
        )}
      </div>
    </section>
  );
};

export default ProfilePage;
