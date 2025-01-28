import { useEffect, useMemo, useState } from "react";

import "./home.scss";
import { Button } from "@/components/ui/button";
import { PlusIcon } from "@/components/ui/icons";
import PageFilter from "@/components/page-filter";
import CampaignCard from "@/components/campaign-card";
import SkeletonCard from "@/components/skeleton-card";
import { useLocation, useNavigate } from "react-router-dom";
import useDonationSmartContract from "@/hooks/useDonationSmartContract";
import { DonationCampaign } from "@/types/donation";
import { PageProps } from "@/types/page";

const HomePage = ({ provider, currentWalletAddress }: PageProps) => {
  // Initialize the smart contract hook with the provided provider
  const smartContract = useDonationSmartContract(provider);

  // React Router hooks for navigation and accessing the current URL
  const navigate = useNavigate();
  const location = useLocation();

  // Extract and decode the search value from the URL query string
  const searchValue = decodeURIComponent(
    location.search.replace("?search=", "")
  );

  // State variables for storing campaign data, filter, and loading status
  const [campaignData, setCampaignData] = useState<DonationCampaign[] | []>([]);
  const [selectedFilter, setSelectedFilter] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(true);

  // step 17 - Fetch campaign data from the smart contract
  const getCampaignData = async () => {};

  // Compute the total number of campaigns
  const totalCampaign = useMemo(
    () => (campaignData && campaignData.length > 0 ? campaignData.length : 0),
    [campaignData]
  );

  // step 18 - Filter campaigns based on selected filter and search value
  const filterCampaignsData = useMemo(() => {}, []);

  // Fetch campaign data when the smart contract is initialized
  useEffect(() => {
    if (smartContract) {
      getCampaignData();
    }
  }, [smartContract]);
  
  return (
    <div className="home-container">
      <div className="home-collection-container">
        <div className="home-collection-head">
          <h3>All Campaigns ({totalCampaign})</h3>
          <div className="button-wrapper">
            <Button
              className="header-button"
              disabled={!currentWalletAddress}
              onClick={() => navigate("/create-campaign")}
            >
              <PlusIcon />
              Create Campaign
            </Button>
          </div>
        </div>
        <PageFilter
          selectedFilter={selectedFilter}
          setSelectedFilter={setSelectedFilter}
        />
        <div className="home-collection">
          {filterCampaignsData && filterCampaignsData.length > 0 ? (
            <div className="campaign-collection">
              {filterCampaignsData.map((data: DonationCampaign, index) => {
                return <CampaignCard data={data} key={index} />;
              })}
            </div>
          ) : loading ? (
            <div className="loading-container">
              {[...Array(8)].map((_, index) => (
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
    </div>
  );
};

export default HomePage;
