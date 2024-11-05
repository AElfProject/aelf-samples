import { useState, Fragment, useEffect } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { IPortkeyProvider } from "@portkey/provider-types";
import { zodResolver } from "@hookform/resolvers/zod";

import { Button } from "../ui/button";
import { ROLE } from "@/lib/constant";
import Modal from "../modal";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "../ui/form";
import { Input } from "../ui/input";
import { Id, toast } from "react-toastify";
import { removeNotification } from "@/lib/utils";
import useSmartContract from "@/hooks/useSmartContract";

interface IProps {
  currentWalletAddress: string | undefined;
  provider: IPortkeyProvider | null;
  role: {
    Admin: string;
    Parent: string;
    Child: string;
  };
  handleOpenModal: (type: string, isEdit?: boolean, address?: string) => void;
}

const formSchema = z.object({
  amount: z.string().min(1, "Amount is required"),
});

const ParentComponent = ({
  currentWalletAddress,
  provider,
  role,
  handleOpenModal,
}: IProps) => {
  const [isModalOpen, setModalOpen] = useState<boolean>(false);
  const [formLoading, setFormLoading] = useState<boolean>(false);
  const [allowanceValue, setAllowanceValue] = useState<number | null>(null);
  const [isContractInitialize, setIsContractInitialize] =
    useState<boolean>(false);
  const { allowanceContract } = useSmartContract(provider);

  // Step K - Configure Set Allowance Form
  const form = useForm<z.infer<typeof formSchema>>();

  const handleCloseModal = () => {
    setModalOpen(false);
    form.reset();
  };

  // step L - Check if Allowance Contract is initialized or not
  const checkIsContractInitialized = async () => {};

  // Step M - Initialize Allowance Contract
  const initializeContract = async () => {};

  // step N - Set AllowanceValue 
  const setAllowance = async (value: { amount: string }) => {};

  // Step O - Handle Set Allowance Submit Form
  const onSubmit = async (value: { amount: string }) => {};

  // step P - Get AllowanceValue 
  const getAllowance = async () => {};
  
  // Use Effect to Fetch Allowance Data & Check contract intialized or not
  useEffect(() => {
    if (allowanceContract) {
      role.Child && getAllowance();
      checkIsContractInitialized();
    }
  }, [formLoading, role.Child, allowanceContract]);

  return (
    <Fragment>
      <h1>Parent</h1>
      <div className="allowance-head">
        <h2>Perform Operations</h2>
        <div className="button-wrapper">
          <Button
            disabled={!!role.Child || !currentWalletAddress}
            onClick={() => handleOpenModal(ROLE.child)}
          >
            Set Child
          </Button>
        </div>
      </div>
      <div className="data-table-container">
        <h2>Child Data</h2>
        <div className="table-head">
          <p className="address">Address</p>
          <p>Allowance</p>
        </div>
        <div className="table-body">
          {!!role.Child ? (
            <div className="table-data-row">
              <p className="address">{role.Child}</p>
              <p className="allowance">{allowanceValue || "---"}</p>
              <Button
                onClick={() => handleOpenModal(ROLE.child, true, role.Child)}
                className="edit"
              >
                Edit
              </Button>
              <Button
                disabled={!currentWalletAddress}
                onClick={() => {
                  setModalOpen(true);
                }}
              >
                Set Allowance
              </Button>
            </div>
          ) : (
            <div className="no-child-data">
              <strong>No Child data found</strong>
            </div>
          )}
        </div>
      </div>
      <Modal
        isVisible={isModalOpen}
        title={"Set Allowance"}
        onClose={handleCloseModal}
      >
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="space-y-8 modal-form"
          >
            <div className="input-group">
              <FormField
                control={form.control}
                name="amount"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Amout</FormLabel>
                    <FormControl>
                      <Input placeholder="Enter Amout" {...field} />
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
                Set Allowance
              </Button>
            </div>
          </form>
        </Form>
      </Modal>
    </Fragment>
  );
};

export default ParentComponent;
