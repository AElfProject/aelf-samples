import { IPortkeyProvider } from "@portkey/provider-types";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { toast } from "react-toastify";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import "./transfer-nft.scss";
import { Button } from "@/components/ui/button";
import { NFT_IMAGES } from "@/lib/constant";
import useNFTSmartContract from "@/hooks/useNFTSmartContract";
import { delay, handleError, removeNotification } from "@/lib/utils";
import { zodResolver } from "@hookform/resolvers/zod";

const formSchema = z.object({
  address: z.string(),
  amount: z.string(),
  memo: z.string(),
});

const TransferNftPage = ({
  provider,
  currentWalletAddress,
}: {
  provider: IPortkeyProvider | null;
  currentWalletAddress: string;
}) => {
  const { sideChainSmartContract } = useNFTSmartContract(provider);
  const navigate = useNavigate();

  const location = useLocation();
  const [searchParams] = useSearchParams(location.search);
  const nftSymbol = searchParams.get("nft-symbol");
  const nftBalance = searchParams.get("nft-balance") || 0;
  const nftIndex = searchParams.get("nft-index");
  const nftDetails = NFT_IMAGES[nftIndex ? Number(nftIndex) : 0];

  const handleReturnClick = () => {
    navigate("/");
  };

  // Configure NFT Transfer Form
  const form = useForm<z.infer<typeof formSchema>>();

  // Transfer NFT to Other Wallet
  const transferNftToOtherAccount = async () => {};

  // Handle Transfer Submit Form
  function onSubmit(values: z.infer<typeof formSchema>) {}

  return (
    <div className="form-wrapper">
      <div className="form-container">
        <div className="form-content">
          <h2 className="form-title">Transfer NFT</h2>
          <div className="nft-card">
            <img src={nftDetails} alt={"nft- image"} />
            <div className="nft-details">
              <p>
                Symbol: <strong>{nftSymbol}</strong>
              </p>
              <p>
                Supply Balance: <strong>{nftBalance}</strong>
              </p>
            </div>
          </div>
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="space-y-8 proposal-form"
            >
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="address"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Wallet Address ( Receiver )</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Enter the wallet address of receiver"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="amount"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Amount</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Enter the Amount"
                          type="number"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="memo"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>memo</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter the memo" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="button-container">
                <button
                  type="button"
                  className="return-btn"
                  onClick={handleReturnClick}
                >
                  Cancel
                </button>
                <Button type="submit" className="submit-btn">
                  Transfer NFT
                </Button>
              </div>
            </form>
          </Form>
        </div>
      </div>
    </div>
  );
};

export default TransferNftPage;
