import { PageProps } from "@/types/page";
import "./user-profile.scss";
import { Copy } from "lucide-react";
import { Id, toast } from "react-toastify";
import { Button } from "@/components/ui/button";
import useDonationSmartContract from "@/hooks/useDonationSmartContract";
import { useEffect, useState } from "react";
import { UserDataType } from "@/types/profile";
import { DonationCampaign } from "@/types/donation";
import CampaignCard from "@/components/campaign-card";
import SkeletonCard from "@/components/skeleton-card";
import { handleError, removeNotification } from "@/lib/utils";

const UserProfile = ({ provider, currentWalletAddress }: PageProps) => {
  const [userData, setUserData] = useState<UserDataType>(); // State to store user data fetched from the smart contract
  const [loading, setLoading] = useState<boolean>(true); // State to track loading state of user data fetch
  const smartContract = useDonationSmartContract(provider); // Hook to interact with the donation smart contract

  // step 22 - Fetch user data from the smart contract using the "GetUserDetails" method.
  const getUserData = async () => {};

  // step 23 - Delete a specific campaign using the campaign ID
  const deleteComapign = async () => {};

  // step 24 - Withdraw amount of raised amount of campaign
  const withdrawAmount = async () => {}

  // Use Effect to Fetch CampaginData
  useEffect(() => {
    if (smartContract && currentWalletAddress) {
      getUserData();
    }
  }, [smartContract, currentWalletAddress]);

  return (
    <div className="user-page container">
      {/* Button to copy the user's wallet address to the clipboard */}
      <Button
        className="profile-button outline"
        onClick={() => {
          navigator.clipboard.writeText(currentWalletAddress as string);
          toast.success("Wallet Address Copied");
        }}
      >
        {currentWalletAddress} <Copy className="copy-icon" />
      </Button>
      <div className="campaigns-collection">
        <h2>Your Campaigns</h2>
        {userData?.campaigns && userData?.campaigns.length > 0 ? (
          <div className="collection">
            {userData?.campaigns.map((data: DonationCampaign, index) => {
              return (
                <CampaignCard
                  data={data}
                  key={index}
                  isProfile
                  handleDelete={deleteComapign}
                  handleWithdraw={withdrawAmount}
                />
              );
            })}
          </div>
        ) : loading ? (
          <div className="loading-container">
            {[...Array(4)].map((_, index) => (
              <SkeletonCard key={index} />
            ))}
          </div>
        ) : (
          <div className="bordered-container">
            <strong>It's Look like no campaign created yet</strong>
          </div>
        )}
      </div>
    </div>
  );
};

export default UserProfile;
