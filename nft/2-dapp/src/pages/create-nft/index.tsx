import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
// @ts-ignore
import AElf from "aelf-sdk";
import { Buffer } from "buffer";
import { Id, toast } from "react-toastify";

import { IPortkeyProvider } from "@portkey/provider-types";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import detectProvider from "@portkey/detect-provider";
import { Button } from "@/components/ui/button";
import useNFTSmartContract from "@/hooks/useNFTSmartContract";
import "./create-nft.scss";

import {
  CustomToast,
  delay,
  handleError,
  removeNotification,
} from "@/lib/utils";
import { InfoIcon } from "@/components/ui/icons";
import { IWalletInfo } from "aelf-sdk/types/wallet";

const formSchema = z.object({
  tokenName: z.string(),
  symbol: z.string(),
  totalSupply: z.string(),
  decimals: z.string(),
});

interface INftInput {
  symbol: string;
  tokenName: string;
  totalSupply: string;
  decimals: string;
  issuer: string;
  isBurnable: boolean;
  issueChainId: number;
  owner: string;
}

interface INftParams {
  tokenName: string;
  symbol: string;
  totalSupply: string;
}

interface INftValidateResult {
  parentChainHeight: string | number;
  signedTx: string;
  merklePath: { merklePathNodes: { hash: string; isLeftChildNode: boolean }[] };
}

const wallet = AElf.wallet.getWalletByPrivateKey(
  "4e83df2aa7c8552a75961f9ab9f2f06e01e0dca0203e383da5468bbbe2915c97"
);

const CreateNftPage = ({
  currentWalletAddress,
}: {
  currentWalletAddress: string;
}) => {
  const [provider, setProvider] = useState<IPortkeyProvider | null>(null);
  const { mainChainSmartContract, sideChainSmartContract } =
    useNFTSmartContract(provider);
  const [transactionStatus, setTransactionStatus] = useState<boolean>(false);
  const [isNftCollectionCreated, setIsNftCollectionCreated] =
    useState<boolean>(false);

  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams] = useSearchParams(location.search);
  const isNftCreate = searchParams.get("nft-create");

  const mainchain_from_chain_id = 9992731;
  const sidechain_from_chain_id = 1931928;

  const tdvw = new AElf(
    new AElf.providers.HttpProvider("https://tdvw-test-node.aelf.io")
  );

  const aelf = new AElf(
    new AElf.providers.HttpProvider("https://aelf-test-node.aelf.io")
  );

  const handleReturnClick = () => {
    navigate("/");
  };

  const init = async () => {
    try {
      setProvider(await detectProvider());
    } catch (error) {
      console.log(error, "=====error");
    }
  };

  useEffect(() => {
    if (!provider) init();
  }, [provider]);

  useEffect(() => {
    if (isNftCreate) {
      setIsNftCollectionCreated(true);
    }
  }, [isNftCreate]);

  // Step D - Configure NFT Form
  const form = useForm<z.infer<typeof formSchema>>();

  // Get Token Contract
  const getTokenContract = async (aelf: AElf, wallet: IWalletInfo) => {
    const tokenContractName = "AElf.ContractNames.Token";
    // get chain status
    const chainStatus = await aelf.chain.getChainStatus();
    // get genesis contract address
    const GenesisContractAddress = chainStatus.GenesisContractAddress;
    // get genesis contract instance
    const zeroContract = await aelf.chain.contractAt(
      GenesisContractAddress,
      wallet
    );
    // Get contract address by the read only method `GetContractAddressByName` of genesis contract
    const tokenContractAddress =
      await zeroContract.GetContractAddressByName.call(
        AElf.utils.sha256(tokenContractName)
      );

    return await aelf.chain.contractAt(tokenContractAddress, wallet);
  };

  // Get CrossChain Contract
  const getCrossChainContract = async (aelf: AElf, wallet: IWalletInfo) => {
    const crossChainContractName = "AElf.ContractNames.CrossChain";

    // get chain status
    const chainStatus = await aelf.chain.getChainStatus();
    // get genesis contract address
    const GenesisContractAddress = chainStatus.GenesisContractAddress;
    // get genesis contract instance
    const zeroContract = await aelf.chain.contractAt(
      GenesisContractAddress,
      wallet
    );
    // Get contract address by the read only method `GetContractAddressByName` of genesis contract
    const crossChainContractAddress =
      await zeroContract.GetContractAddressByName.call(
        AElf.utils.sha256(crossChainContractName)
      );

    return await aelf.chain.contractAt(crossChainContractAddress, wallet);
  };

  //============== Create NFT Collection Steps =================//

  // step 1 - Create New NFT Collection on MainChain Function
  const createNftCollectionOnMainChain = async (values: {
    tokenName: string;
    symbol: string;
    totalSupply: string;
    decimals: string;
  }) => {
    let createLoadingId: Id;
    createLoadingId = toast.loading("Creating NFT Collection..");
    try {
      // Create an object with the necessary information for the new NFT collection.
      const createNtfInput: INftInput = {
        tokenName: values.tokenName, // Name of the nft Collection
        symbol: values.symbol, // Symbol of the token (You have to get it from your PortKey wallet on NFT seed from NFT section)
        totalSupply: values.totalSupply, // Total supply of the token
        decimals: values.decimals, // Decimals of the token
        issuer: currentWalletAddress, // Address of the token issuer
        isBurnable: true, // Indicates if the token can be burned
        issueChainId: sidechain_from_chain_id, // ID of the issuing chain
        owner: currentWalletAddress, // Owner's wallet address
      };

      // Call the smart contract method to create the new NFT collection on the main chain.
      const result = await mainChainSmartContract?.callSendMethod(
        "Create",
        currentWalletAddress,
        createNtfInput
      );

      // Log the result of the creation for debugging purposes.
      console.log("========= result of createNewNft =========", result);

      toast.update(createLoadingId, {
        render: "NFT Collection Created Successfully On MainChain",
        type: "success",
        isLoading: false,
      });
      removeNotification(createLoadingId);

      // Return the input data for further use.
      return createNtfInput;
    } catch (error) {
      handleError(createLoadingId, error);
      return "error";
    }
  };

  // step 2 - Validate Collection information existence
  // This function validates if the token collection information already exists on the main blockchain.
  const validateNftCollectionInfo = async (values: INftInput) => {
    let validateLoadingId: Id;
    // Start Loading before initiate the transaction
    validateLoadingId = toast.loading(
      <CustomToast
        title="Transaction is getting validated on aelf blockchain. Please wait!"
        message="Validation means transaction runs through a consensus algorithm to be selected or rejected. Once the status changes process will complete. It usually takes some time in distributed systems."
      />
    );
    try {
      // Create an object with the necessary information for token validation.
      const validateInput = {
        symbol: values.symbol, // Symbol of the token
        tokenName: values.tokenName, // Name of the token
        totalSupply: values.totalSupply, // Total supply of the token
        decimals: values.decimals, // Decimals of the token
        issuer: currentWalletAddress, // Address of the token issuer
        isBurnable: true, // Indicates if the token can be burned
        issueChainId: sidechain_from_chain_id, // ID of the issuing chain
        owner: currentWalletAddress, // Owner's wallet address
      };

      // get mainnet contract
      const aelfTokenContract = await getTokenContract(aelf, wallet);

      // prepare Sign the transaction using contract method (ValidateTokenInfoExists Function)
      const signedTx =
        aelfTokenContract.ValidateTokenInfoExists.getSignedTx(validateInput);

      // send the transaction using signed Transaction
      const { TransactionId: VALIDATE_TXID } = await aelf.chain.sendTransaction(
        signedTx
      );

      // get Validate Result
      let VALIDATE_TXRESULT = await aelf.chain.getTxResult(VALIDATE_TXID);

      // we need to wait till our latest index Hight grater than or equal to our Transaction block number
      let heightDone = false;

      while (!heightDone) {
        // get latest index Hight
        const sideIndexMainHeight = await GetParentChainHeight();
        if (
          // check the latest index Hight is grater than or equal
          Number(sideIndexMainHeight) >=
          VALIDATE_TXRESULT.Transaction.RefBlockNumber
        ) {
          VALIDATE_TXRESULT = await aelf.chain.getTxResult(VALIDATE_TXID);
          heightDone = true;
        }
      }

      console.log("VALIDATE_TXRESULT", VALIDATE_TXRESULT);

      // Update the Loading Message
      toast.update(validateLoadingId, {
        render: "Validating Token Successfully Executed",
        type: "success",
        isLoading: false,
      });

      // Remove the Loading Message
      removeNotification(validateLoadingId);

      // Return necessary details.
      return {
        transactionId: VALIDATE_TXID,
        signedTx: signedTx,
        BlockNumber: VALIDATE_TXRESULT.BlockNumber,
      };
    } catch (error) {
      // If there's an error, log it and alert the user.
      console.error(error, "=====error in validateTokenInfoExist");
      handleError(validateLoadingId, error);
      return "error";
    }
  };

  // Step 3: Get the parent chain height
  // This function fetches the current height of the parent blockchain.
  const GetParentChainHeight = async () => {
    try {
      const tdvwCrossChainContract = await getCrossChainContract(tdvw, wallet);
      // Call the smart contract method to get the parent chain height.
      const result = await tdvwCrossChainContract.GetParentChainHeight.call();
      // Return the parent chain height if it exists, otherwise return an empty string.
      return result ? (result.value as string) : "";
    } catch (error) {
      // If there's an error, log it and return an error status.
      console.error(error, "=====error in GetParentChainHeight");
      return "error";
    }
  };

  // step 4 - Fetch the merkle path by transaction Id
  const getMerklePathByTxId = async (aelf: AElf, txId: string) => {
    try {
      const { MerklePathNodes } = await aelf.chain.getMerklePathByTxId(txId);

      const formattedMerklePathNodes = MerklePathNodes.map(
        ({
          Hash,
          IsLeftChildNode,
        }: {
          Hash: string;
          IsLeftChildNode: boolean;
        }) => ({
          hash: Hash,
          isLeftChildNode: IsLeftChildNode,
        })
      );

      return { merklePathNodes: formattedMerklePathNodes };
    } catch (error) {
      console.error("Error fetching Merkle path:", error);
      throw new Error("Failed to get Merkle path by transaction ID.");
    }
  };

  // step 5 - Create a collection on the dAppChain
  const createCollectionOnSideChain = async () => {};

  //============== Create NFT Token Steps =================//

  // step 6 - Create an NFT on the mainchain
  const createNFTOnMainChain = async () => {};

  // step 7 - Validate a NFT Token on MainChain
  const validateNftToken = async () => {};

  // step 8 - Create a NFT on dAppChain.
  const createNftTokenOnSideChain = async () => {};

  // step 9 - Issue a NFT Function which has been Created on dAppChain
  const issueNftOnSideChain = async () => {};

  // step 10 - Call Necessary Function for Create NFT
  const createNftToken = async () => {};

  //============== Handle Submit Form =================//

  // Step 11 - Handle Submit Form
  const onSubmit = async () => {};

  return (
    <div className="form-wrapper">
      <div className="form-container">
        <div className="form-content">
          <h2 className="form-title">Create a New NFT</h2>
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="space-y-8 proposal-form"
            >
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="tokenName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Token Name</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Token Name" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="symbol"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="symbol-label">
                        Symbol <InfoIcon className="info-icon" />
                      </FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Symbol" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <div className="input-group">
                <FormField
                  control={form.control}
                  name="totalSupply"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Total Supply</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Total Supply" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              {!isNftCollectionCreated && (
                <div className="input-group">
                  <FormField
                    control={form.control}
                    name="decimals"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Decimals</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter Decimals" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              )}
              <div className="button-container">
                <Button
                  type="button"
                  className="return-btn"
                  disabled={!!transactionStatus}
                  onClick={handleReturnClick}
                >
                  Return
                </Button>
                <Button
                  type="submit"
                  className="submit-btn"
                  disabled={!!transactionStatus}
                >
                  Create {isNftCollectionCreated ? "NFT" : "Collection"}
                </Button>
              </div>
            </form>
          </Form>
        </div>
      </div>
    </div>
  );
};

export default CreateNftPage;
