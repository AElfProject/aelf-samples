import { DonationCampaign } from "./donation";

export type UserDataType = {
  campaigns: DonationCampaign[];
  donatedCampaigns: DonationCampaign[];
  walletAddress: string;
  totalRaisedAmount: string;
};
