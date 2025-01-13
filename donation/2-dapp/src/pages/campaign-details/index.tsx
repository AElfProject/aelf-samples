import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { CAMPAIGN_CATEGORY_ICONS } from "@/lib/constant";
import useDonationSmartContract, {
  donationContractAddress,
} from "@/hooks/useDonationSmartContract";
import { DonationCampaign } from "@/types/donation";
import DonateWidget from "./donate-widget";
import "./campaign-details.scss";
import { Id, toast } from "react-toastify";
import {
  convertAmountToToken,
  convertTokenToAmount,
  handleError,
  removeNotification,
} from "@/lib/utils";
import useTokenContract from "@/hooks/useTokenContract";
import { Button } from "@/components/ui/button";
import { PageProps } from "@/types/page";

// Default token allowance to set when approving transactions
const setDefaultAllowance = "10000000000";

// Main component for rendering campaign details and donation functionality
const CampaignDetails = ({ provider, currentWalletAddress }: PageProps) => {
  const [campaignDetails, setCampaignDetails] = useState<DonationCampaign>(); // State to hold campaign details
  const [formLoading, setFormLoading] = useState<boolean>(false); // State to indicate form submission/loading status
  const [loading, setLoading] = useState<boolean>(true); // State to indicate overall loading status

  // Initialize smart contract and token contract hooks
  const smartContract = useDonationSmartContract(provider);
  const tokenContract = useTokenContract(provider);

  // Extract `id` from the route parameters
  const { id } = useParams();
  const navigate = useNavigate(); // Navigation hook to redirect users

  // step 19 - Fetch campaign details from the smart contract
  const getCampaignDetails = async () => {};

  // Fetch campaign data when the component mounts or the smart contract instance changes
  useEffect(() => {
    if (smartContract) {
      getCampaignDetails();
    }
  }, [smartContract]);

  // Show loading spinner while data is being fetched
  if (loading) {
    return (
      <div className="container">
        <h3>Loading Data...</h3>
      </div>
    );
  }

  // step 20 - Check if the user's wallet has sufficient allowance to spend tokens
  const checkAllowance = async () => {};

  // step 21 - Function to donate funds
  const donateFunds = async (amount: number) => {};

  return (
    <div className="container">
      {campaignDetails && (
        <div className="campaign-detail-container">
          <div className="top-view">
            <div className="image-container">
              {campaignDetails.creator === currentWalletAddress && (
                <div className="btn-wrapper">
                  <Button
                    className="edit-btn"
                    onClick={() =>
                      navigate("/edit-campaign/" + campaignDetails.id)
                    }
                  >
                    Edit
                  </Button>
                </div>
              )}
              <img
                src={campaignDetails.imageUrl}
                alt="campaign-image"
                loading="lazy"
              />
            </div>
            {campaignDetails && (
              <DonateWidget
                provider={provider}
                campaignDetails={campaignDetails}
                currentWalletAddress={currentWalletAddress}
                formLoading={formLoading}
                donateFunds={donateFunds}
              />
            )}
          </div>
          <div className="info">
            <div className="category">
              {CAMPAIGN_CATEGORY_ICONS[campaignDetails.type || ""]}
              <p>{campaignDetails.type.replace("_"," ")}</p>
            </div>
            <p className="title">{campaignDetails.title}</p>
            <p className="desc">{campaignDetails.description}</p>
            <h4 className="section-title">CREATOR</h4>
            <div className="creator">
              <img
                src="/user-profile.svg"
                className="user-profile"
                alt="User Profile"
                title="Profile"
              />
              <p>{campaignDetails.creator}</p>
            </div>
            <h4 className="section-title">DONATORS</h4>

            {campaignDetails.donators.length > 0 ? (
              <ul className="donator-list">
                {campaignDetails.donators.map((data, index) => (
                  <li key={index}>
                    <p>{data.donor}</p>
                    <p>{convertTokenToAmount(data.amount)} ELF</p>
                  </li>
                ))}
              </ul>
            ) : (
              <></>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default CampaignDetails;
