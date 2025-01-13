import { useEffect, useMemo, useRef, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";
import { DonationCampaign, Donator } from "@/types/donation";
import Counter from "./counter";
import { Button } from "@/components/ui/button";
import userBalance from "@/hooks/userBalance";
import { Input } from "@/components/ui/input";
import { convertTokenToAmount, remainingCounter } from "@/lib/utils";

interface IProps {
  provider: IPortkeyProvider | null;
  campaignDetails: DonationCampaign;
  currentWalletAddress?: string;
  formLoading: boolean;
  donateFunds: (amount: number) => void;
}

// DonateWidget component to handle user interactions for donating to a campaign
const DonateWidget = ({
  provider, // The provider object for blockchain interactions
  campaignDetails, // Details of the campaign being viewed
  currentWalletAddress, // Current user's wallet address
  formLoading, // Indicates if a form submission is in progress
  donateFunds, // Function to handle the donation process
}: IProps) => {
  const [amount, setAmount] = useState(0); // Amount to be donated
  const [amountError, setAmountError] = useState(""); // Error message for invalid input
  const { balance } = userBalance(provider, currentWalletAddress); // Fetch user's balance
  const intervalId = useRef<NodeJS.Timeout | null>(null); // Ref for interval timer
  const [timeDiff, setTimeDiff] = useState({
    days: 0,
    hours: 0,
    minutes: 0,
    seconds: 0,
    difference: 0, // Difference in time (in milliseconds) between now and campaign end
  });

  // Calculate the percentage of the goal amount raised
  const raisedPercentage =
    campaignDetails?.currentAmount && campaignDetails?.goalAmount
      ? (Number(campaignDetails.currentAmount) /
          Number(campaignDetails.goalAmount)) *
        100
      : 0;

  // Check if the campaign's end time has passed
  const isTimeOver = timeDiff.difference === 0;

  const isGoalCompleted = Number(campaignDetails.currentAmount) === Number(campaignDetails.goalAmount);

  const yourDonation = useMemo(() => {
    if (campaignDetails && currentWalletAddress) {
      let amount = 0;
      campaignDetails.donators.length > 0 &&
        campaignDetails.donators.map((data: Donator) => {
          if (data.donor === currentWalletAddress) {
            amount += Number(data.amount);
          }
        });
      return convertTokenToAmount(amount);
    }
    return 0;
  }, [campaignDetails]);

  // Get the difference between the current time and the campaign end time
  const getDifference = (endTime: string) => {
    const data = remainingCounter(endTime);
    setTimeDiff(data);
    return data;
  };

  // Handle changes in the donation amount input field
  const handleAmountChange = (inputAmount: number) => {
    if (!campaignDetails) {
      return;
    }
    setAmountError(""); // Clear any existing error
    setAmount(inputAmount); // Set the new donation amount

    // Validate if the input amount exceeds the user's balance
    if (inputAmount > balance) {
      setAmountError("You don't have enough balance");
    }

    // Validate if the input amount exceeds the remaining goal amount
    const remainingDonation =
      convertTokenToAmount(campaignDetails.goalAmount) -
      convertTokenToAmount(campaignDetails.currentAmount);
    if (inputAmount > remainingDonation) {
      setAmountError("You cannot donate more than the remaining goal");
    }
  };

  // Set up an interval to update the time remaining until the campaign ends
  useEffect(() => {
    if (campaignDetails?.endTime) {
      intervalId.current = setInterval(() => {
        const data = getDifference(campaignDetails?.endTime);
        if (data.difference === 0) {
          clearInterval(intervalId.current!); // Clear interval when time is up
          intervalId.current = null;
        }
      }, 1000); // Update every second
    }

    // Cleanup interval on component unmount or when endTime changes
    return () => {
      if (intervalId.current) {
        clearInterval(intervalId.current);
        intervalId.current = null;
      }
    };
  }, [campaignDetails?.endTime]);

  return (
    <div className="donate-container">
      <h3>Donate ELF</h3>
      <Counter timeDiff={timeDiff} />
      <p className="primary-text raised-amount">
        Raised Amount:{" "}
        {convertTokenToAmount(campaignDetails?.currentAmount || 0)} ELF /{" "}
        {convertTokenToAmount(campaignDetails?.goalAmount || 0)} ELF{" "}
      </p>
      <div className="raised-progress-bar">
        <div
          className="active-bar"
          style={{ width: raisedPercentage + "%" }}
        ></div>
      </div>
      <p className="primary-text donation-text">
        Your Donation: {yourDonation} ELF
      </p>
      <p className="primary-text your-balance">Your Balance: {balance}</p>
      {!isTimeOver && !isGoalCompleted ? (
        <div>
          <div className={`donate-input ${amountError ? "with-error" : ""}`}>
            <Input
              value={amount}
              type="number"
              placeholder="0"
              onChange={(e) => handleAmountChange(Number(e.target.value))}
            />
            <Button
              className="max-btn"
              onClick={() => handleAmountChange(balance)}
            >
              Max
            </Button>
            {amountError && <p className="error-text">{amountError}</p>}
          </div>
          <Button
            className="donate-btn"
            disabled={amount <= 0 || formLoading || !!amountError}
            onClick={() => {
              donateFunds(amount);
              setAmount(0);
            }}
          >
            Donate
          </Button>
        </div>
      ) : (
        <p className="primary-text campaign-over">{isGoalCompleted ? "Goal Amount Over" : "Campaign Over"}</p>
      )}
    </div>
  );
};

export default DonateWidget;
