export interface Donator {
  donor: string;
  amount: string;
  timestamp: number;
}

export interface DonationCampaign {
  donators: Donator[];
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  type: string;
  goalAmount: string;
  currentAmount: string;
  creator: string;
  startTime: string;
  endTime: string;
  isActive: boolean;
  isWithdrawn: boolean
}
