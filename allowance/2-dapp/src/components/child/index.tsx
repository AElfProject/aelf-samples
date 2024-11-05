import { useState, Fragment, useEffect } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { IPortkeyProvider } from "@portkey/provider-types";
import { Id, toast } from "react-toastify";

import { Button } from "../ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "../ui/form";
import { Input } from "../ui/input";
import { removeNotification } from "@/lib/utils";
import useSmartContract from "@/hooks/useSmartContract";

interface IProps {
  provider: IPortkeyProvider | null;
  currentWalletAddress: string | undefined;
}

const formSchema = z.object({
  amount: z.string().min(1, "Number is required"),
});

const ChildComponent = ({ provider, currentWalletAddress }: IProps) => {
  const { allowanceContract } = useSmartContract(provider);
  const [formLoading, setFormLoading] = useState<boolean>(false);
  const [allowanceValue, setAllowanceValue] = useState("");

  // Step Q - Configure Spend Allowance Form
  const form = useForm<z.infer<typeof formSchema>>();

  // step R - Get AllowanceValue 
  const getAllowance = async () => {};

  useEffect(() => {
    allowanceContract && getAllowance();
  }, [allowanceContract]);

  // step S - Spend Allowance 
  const spendAllowance = async (value: { amount: string }) => {};

  // Step T - Handle Spend Allowance Submit Form
  const onSubmit = (values: { amount: string }) => {};

  return (
    <Fragment>
      <h1>Child</h1>
      <div className="data-table-container child">
        <div className="spend-funds-form">
          <h2>Spend Allowance</h2>
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="space-y-8 modal-form"
            >
              {allowanceValue && (
                <p className="allowance-text">
                  Your Allowance = {allowanceValue}
                </p>
              )}
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="amount"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Amount</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Amount" {...field} />
                      </FormControl>
                      <FormMessage className="error-message" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="button-container">
                <Button
                  type="submit"
                  className="submit-btn"
                  disabled={formLoading}
                >
                  Spend
                </Button>
              </div>
            </form>
          </Form>
        </div>
      </div>
    </Fragment>
  );
};

export default ChildComponent;
