import { fetchContract } from "@/lib/contract";
import { IPortkeyProvider, IChain } from "@portkey/provider-types";
import { useEffect, useState } from "react";

const useTokenContract = (provider: IPortkeyProvider | null) => {
  const [tokenContract, setTokenContract] = useState<ReturnType<IChain["getContract"]>>();

  // Step 5 - Function to fetch the ELF token contract on dApp chain
  const fetchTokenContract = async () => {};

  // Step 6 -  Effect hook to initialize and fetch the smart contract when the provider changes
  useEffect(() => {}, []); // Dependency array ensures this runs when the provider changes
  
  return tokenContract;
};

export default useTokenContract;
