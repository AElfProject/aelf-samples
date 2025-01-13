import { IPortkeyProvider } from "@portkey/provider-types";
import useTokenContract from "./useTokenContract";
import { useEffect, useState } from "react";
import { convertTokenToAmount } from "@/lib/utils";

const userBalance = (
  provider: IPortkeyProvider | null,
  currentWalletAddress?: string
) => {
  const tokencontract = useTokenContract(provider);
  const [balance, setBalance] = useState(0);
  const [isLoading, setLoading] = useState(false);

  // Step 7 - Function to get ELF token balance
  const getTokenBalance = async () => {};

  // Step 8 - Effect hook to fetch the token balance when tokencontract and currentWalletAddress change
  useEffect(() => {}, []);

  return {
    balance,
    isLoading,
    getBalance: getTokenBalance,
  };
};

export default userBalance;
