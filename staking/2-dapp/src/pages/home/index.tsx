import { useEffect, useMemo, useRef, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";
import { Id, toast } from "react-toastify";
import axios from "axios";

import "./home.scss";
import { removeNotification, stakingContractAddress } from "@/lib/utils";
import useSmartContract from "@/hooks/useSmartContract";

import { Button } from "@/components/ui/button";
import useStakingContract from "@/hooks/useStakingContract";
import DepositDataCard from "@/components/deposit-data-card";
import CreateTokenodal from "@/components/create-token-modal";

interface PageProps {
  provider: IPortkeyProvider | null;
  currentWalletAddress?: string;
}

interface ITokenInfo {
  balance: string;
  tokenInfo: {
    symbol: string;
  };
}

interface IDepositData {
  depositId: string;
  address: string;
  tokenSymbol: string;
  amount: string;
  lockTime: string;
  depositTime: string;
}

const HomePage = ({ provider, currentWalletAddress }: PageProps) => {
  const { sideChainSmartContract } = useSmartContract(provider);
  const stakingContract = useStakingContract(provider);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [formLoading, setFormLoading] = useState<boolean>(false);
  const [tokenInfo, setTokenInfo] = useState<null | ITokenInfo>(null);
  const [depositData, setDepositData] = useState<[] | IDepositData[]>([]);
  const [isEdited, setIsEdited] = useState(false);
  const [isWithdrawing, setIsWithdrawing] = useState(false);
  const [amount, setAmount] = useState("");
  const [amountError, setAmountError] = useState("");
  const [totalStakedAmount, setTotalStakedAmount] = useState("0");

  const intervalId = useRef<number | NodeJS.Timeout | null>(null);

  // Filter Withdrawable Data from Deposite data
  const withdrawableData = useMemo(() => {
    if (depositData.length > 0) {
      const current = Math.floor(new Date().getTime() / 1000);
      return depositData.filter(
        (data) => Number(data.depositTime) + Number(data.lockTime) < current
      );
    }
    return [];
  }, [depositData]);

  // Filter Staked Data from Deposite data
  const stakedData = useMemo(() => {
    if (depositData.length > 0) {
      const current = Math.floor(new Date().getTime() / 1000);
      return depositData.filter((data) => Number(data.depositTime) + Number(data.lockTime) > current);
    }
    return [];
  }, [depositData]);

  // Calculating total amount of user's staked TOKEN
  const totalUserStakeAmount = useMemo(() => {
    return depositData.length > 0
      ? depositData.reduce(
          (total: number, stakeData: IDepositData) =>
            total + parseFloat(stakeData.amount),
          0
        )
      : 0;
  }, [depositData]);

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  // Step R - fetch Fungible Token data
  const fetchTokenData = async () => {};

  // useEffect hook to fetch token data periodically when the wallet address is available and the form is not loading
  useEffect(() => {
    if (!formLoading && currentWalletAddress) {
      fetchTokenData();
      clearInterval(intervalId.current as number);

      const intervalid = setInterval(() => {
        fetchTokenData();
      }, 5000);

      intervalId.current = intervalid;

      // Cleanup the interval on component unmount
      return () => {
        if (intervalId.current) {
          clearInterval(intervalId.current as number); // casting it to a number
        }
      };
    }
  }, [currentWalletAddress, formLoading]);

  // useEffect hook to fetch deposit data and total staked amount when the wallet address is available and the form is not loading
  useEffect(() => {
    // Check if the form is not loading and the current wallet address is available
    if (!formLoading && currentWalletAddress) {
      // Fetch the deposit data when the conditions are met
      fetchDepositData();
  
      // Fetch the total staked amount when the conditions are met
      fetchTotalStakedAmount();
    }
  }, [currentWalletAddress, formLoading]); // Dependencies for useEffect - re-run when these change

  const onHandleMax = () => {
    setAmount(tokenInfo?.balance || "0");
  };

  const handleAmountError = (amount: string) => {
    const error =
      amount === "0"
        ? "Amount Should be Grater than 0"
        : !amount
        ? "Amount is Require"
        : Number(amount) > (Number(tokenInfo?.balance) || 0)
        ? "You don't have enough Token Balance"
        : "";
    setAmountError(error);
    return !!error;
  };

  useEffect(() => {
    if (!isEdited) {
      return;
    }
    handleAmountError(amount);
  }, [amount]);

  // Step S - Function to transfer TOKEN to the Staking Contract
  const transferTokenToStakingContract = async (amount: string) => {};

  // Step T - Function to handle staking of the token
  const handleStaking = async () => {};

  // Step U - Function to fetch deposit data for the current wallet address
  const fetchDepositData = async () => {};

  // Step V - Function to fetch the total staked amount from the staking contract
  const fetchTotalStakedAmount = async () => {};

  // Step W - Function to withdraw staked tokens based on a deposit ID
  const withdrawStake = async (depositId: string) => {};

  // Step X - Function to perform an emergency withdrawal of staked tokens based on a deposit ID
  const emergencyWithdrawStake = async (depositId: string) => {};

  return (
    <div className="home-container">
      <div className="staking-page-container">
        <div className="heading">
          <h1>Staking dApp</h1>
          <Button
            type="submit"
            className="submit-btn"
            onClick={() => setIsModalOpen(true)}
          >
            Create Token
          </Button>
        </div>
        <p className="app-desc">
          Securely stake your tokens on the aelf blockchain and earn rewards.
          Easily manage your staking, track rewards, and withdraw anytime, all
          with a user-friendly interface designed for the seamless decentralized
          finance (DeFi) interaction.
        </p>
        <div className="staking-wrapper">
          <div className="staking-widget">
            <h2>Stake $TOKEN</h2>
            <div className="token-balance">
              <p>
                $TOKEN Balance :{" "}
                <strong>{tokenInfo ? tokenInfo.balance : 0}</strong>
              </p>
            </div>
            <div className="input-container">
              <label>Token Amount</label>
              <button onClick={onHandleMax}>MAX</button>
              <input
                placeholder="Enter Amount"
                className={amountError ? "error" : ""}
                type="text"
                value={amount}
                onChange={(e) => {
                  let value = parseInt(e.target.value);
                  if (value.toString() !== "NaN") {
                    setAmount(value.toString());
                    setIsEdited(true);
                  }
                  if (!e.target.value) {
                    setAmount(e.target.value);
                  }
                }}
              />
              {amountError && <p className="input-error">{amountError}</p>}
            </div>
            <label>Duration</label>
            <div className="duration-container">
              <div className="left-info">
                <h4>5 Minutes</h4>
                <p>1.1x multiplier</p>
              </div>
            </div>
            <Button
              type="submit"
              className="submit-btn"
              onClick={handleStaking}
            >
              Stake $TOKEN
            </Button>
          </div>
          <div className="staking-info-wrapper">
            <div className="staking-box-container">
              <div className="staking-info-box">
                <p>Total Staked $Token (Your)</p>
                <h2>{totalUserStakeAmount}</h2>
              </div>
              <div className="staking-info-box">
                <p>Total Staked $Token</p>
                <h2>{totalStakedAmount}</h2>
              </div>
            </div>
            <h2>Here How it Works</h2>
            <p>
              First, decide how many $TOKENs you're willing to stake, then
              choose the duration for your staked $TOKEN. it's as simple
              as that!
            </p>
            <p>Now, here are a few rules you'll need to keep in mind.</p>
            <ul>
            <li>The lock duration is 5 Minute.</li>
              <li>The highest multiplier you can get is 1.1x for now.</li>
              <li>You can withdraw Staked amount after 5 Minute with reward.</li>
              <li>
                Any user can lock their $TOKENs and earn rewards based on the
                multiplier.
              </li>
              <li>
                The staked amount is available for emergency withdrawal at any
                time.
              </li>
            </ul>
          </div>
        </div>
        <div className="data-table-container">
          <h2>Available to Withdraw $TOKEN</h2>
          <div className="table-head">
            <p>Amount</p>
            <p>Multiplier</p>
            <p>Total</p>
            <p className="date">Staked At</p>
            <p className="action">Action</p>
          </div>
          {withdrawableData.length > 0 &&
            withdrawableData.map((data: IDepositData) => (
              <DepositDataCard
                data={data}
                isWithdrawable
                isWithdrawing={isWithdrawing}
                withdrawStake={withdrawStake}
              />
            ))}
        </div>
        <div className="data-table-container">
          <h2>Staked $TOKEN</h2>
          <div className="table-head">
            <p>Amount</p>
            <p>Multiplier</p>
            <p>Total</p>
            <p className="date">Staked At</p>
            <p className="date">Remaining Time</p>
            <p className="action">Action</p>
          </div>
          {stakedData.length > 0 &&
            stakedData.map((data: IDepositData) => (
              <DepositDataCard
                data={data}
                isWithdrawing={isWithdrawing}
                withdrawStake={emergencyWithdrawStake}
              />
            ))}
        </div>
        <CreateTokenodal
          isModalOpen={isModalOpen}
          handleCloseModal={handleCloseModal}
          formLoading={formLoading}
          setFormLoading={setFormLoading}
          currentWalletAddress={currentWalletAddress}
          provider={provider}
        />
      </div>
    </div>
  );
};

export default HomePage;
