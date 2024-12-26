import { IPortkeyProvider, IChain } from "@portkey/provider-types";
import { useEffect, useState } from "react";
import { IContract } from "@/lib/types";

// Custom Hook for interacting with NFT Smart Contracts
const useNFTSmartContract = (provider: IPortkeyProvider | null) => {
  // State variables to store the smart contract instances
  const [mainChainSmartContract, setMainChainSmartContract] = useState<ReturnType<IChain["getContract"]>>();
  const [sideChainSmartContract, setSideChainSmartContract] = useState<ReturnType<IChain["getContract"]>>();

//Step A - Function to fetch a smart contract based on the chain symbol and the contract address
const fetchContract = async () => {};

// Step B -  Effect hook to initialize and fetch the smart contracts when the provider changes
useEffect(() => {}, []); // Dependency array ensures this runs when the provider changes

  // Return the smart contract instances
  return {
    mainChainSmartContract,
    sideChainSmartContract,
  };
};

export default useNFTSmartContract;
