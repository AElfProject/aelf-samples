import { IPortkeyProvider, IChain } from "@portkey/provider-types";
import { useEffect, useState } from "react";

const useSmartContract = (provider: IPortkeyProvider | null) => {
  const [roleContract, setRoleContract] = useState<ReturnType<IChain["getContract"]>>();
  const [allowanceContract, setAllowanceContract] = useState<ReturnType<IChain["getContract"]>>();

  //Step A - Function to fetch a smart contract based on deployed wallet address
  const fetchContract = async (address: string) => {};

  // Step B - fetch role-contract
  const getRoleContract = async () => {
    //Replace with Address of Deployed Allowance Smart Contract
    const contract = await fetchContract("your_deployed_role_contract_address");
    contract && setRoleContract(contract);
  };


  // Step C - fetch allowance-contract
  const getAllowanceContract = async () => {
    //Replace with Address of Deployed Allowance Smart Contract
    const contract = await fetchContract("your_deployed_allowance_contract_address");
    contract && setAllowanceContract(contract);
  };

  // Step D - Effect hook to initialize and fetch the smart contract when the provider changes
  useEffect(() => {}, [provider]); // Dependency array ensures this runs when the provider changes

  return { roleContract, allowanceContract };
};

export default useSmartContract;
