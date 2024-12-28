import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
// @ts-ignore
import AElf from "aelf-sdk";
import { Buffer } from "buffer";
import { toast } from "react-toastify";

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

interface ValidationResult {
  transactionId: string;
  signedTx: string;
  BlockNumber: number;
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

  // step - 1 Create New NFT Collection on the mainchain
  const createNftCollectionOnMainChain = async ()=> {};

  // step 2 - Validate Collection information exist
  // This function validates if the token collection information already exists on the main blockchain.
  const validateNftCollectionInfo = async () => {};

  // Step 3: Get the parent chain height
  // This function fetches the current height of the parent blockchain.
  const GetParentChainHeight = async () => {};

  // step 5 - Create a collection on the dAppChain
  const createCollectionOnSideChain = async () => {};

  //============== Create NFT Token Steps =================//

  // step 6 - Create an NFT on the mainchain
  const createNFTOnMainChain = async () => {};

  // step 7 - Validate an NFT token on the maincgit stashhain
  const validateNftToken = async () => {};

  // step 8 - Create a NFT on dAppChain.
  const createNftTokenOnSideChain = async () => {};

  // step 9 - Issue a NFT Function which has been Created on dAppChain
  const issueNftOnSideChain = async () => {};

  // step 10 - Call Necessary Function for Create NFT
  const createNftToken = async () => {};

  //============== Handle Submit Form =================//

  //Step 11 - Handle Submit Form
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
