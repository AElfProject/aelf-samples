import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { IPortkeyProvider } from "@portkey/provider-types";
import { toast } from "react-toastify";

import "./home.scss";
import { NFT_IMAGES } from "@/lib/constant";
import { Button } from "@/components/ui/button";
import useNFTSmartContract from "@/hooks/useNFTSmartContract";
import { fetchUserNftData } from "@/lib/commonFunctions";
import { IContract } from "@/lib/types";

const HomePage = ({
  provider,
  currentWalletAddress,
}: {
  provider: IPortkeyProvider | null;
  currentWalletAddress?: string;
}) => {
  const navigate = useNavigate();
  const [userNfts, setUserNfts] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const { sideChainSmartContract } = useNFTSmartContract(provider);

  // get NFT Data from User's wallet
  const getNFTData = async () => {};

  // Use Effect to Fetch NFTs
  useEffect(() => {
    if (currentWalletAddress && sideChainSmartContract) {
      getNFTData();
    }
  }, [currentWalletAddress, sideChainSmartContract]);

  return (
    <div className="home-container">
    <div className="marketplace-info">
      <h1>NFTs</h1>
      <h3>Create and Transfer Non-Fungible Tokens with AELF</h3>
    </div>
    <div className="nft-collection-container">
      <div className="nft-collection-head">
        <h2>Your NFT Tokens</h2>
        <div className="button-wrapper">
          <Button
            className="header-button"
            disabled={!!(userNfts.length === 0)}
            onClick={() => navigate(`/create-nft?nft-create=true`)}
          >
            Create NFT
          </Button>
          <Button
            className="header-button"
            onClick={() =>
              currentWalletAddress
                ? navigate("/create-nft")
                : toast.warning("Please Connect Wallet First")
            }
          >
            Create NFT Collection
          </Button>
        </div>
      </div>

      {currentWalletAddress ? (
        <div className="nft-collection">
          {userNfts.length > 0 ? (
            userNfts.slice(0, 5).map((data, index) => (
              <div
                className={
                  userNfts.length > 3 ? "nft-card around" : "nft-card"
                }
                key={index}
              >
                <img src={NFT_IMAGES[index + 1]} alt={"nft- image" + index} />
                <div className="nft-info">
                  <p>{data.nftSymbol}</p>
                </div>

                <div className="nft-info-row">
                  <span>Name:</span>
                  <span>{data.tokenName}</span>
                </div>

                <div className="nft-info-row">
                  <span>Collection Symbol:</span>
                  <span>{data.collectionSymbol}</span>
                </div>

                <div className="nft-info-row">
                  <span>Balance:</span>
                  <span>{data.balance}</span>
                </div>

                <div className="nft-info-row">
                  <span>Owner:</span>
                  <span>{data.realOwner.address}</span>
                </div>

                <div className="buy-container">
                  <Button
                    onClick={() =>
                      navigate(
                        `/transfer-nft?nft-index=${index + 1}&nft-symbol=${data.nftSymbol}&nft-balance=${data.balance}`
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
    </div>
  );
};

export default HomePage;
