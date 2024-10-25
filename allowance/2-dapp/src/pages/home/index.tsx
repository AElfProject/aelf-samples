import { useEffect, useMemo, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { Id, toast } from "react-toastify";

import "./home.scss";
import { Button } from "@/components/ui/button";
import Modal from "@/components/modal";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { zodResolver } from "@hookform/resolvers/zod";
import AdminComponent from "@/components/admin";
import ParentComponent from "@/components/parent";
import { ROLE } from "@/lib/constant";
import ChildComponent from "@/components/child";
import useSmartContract from "@/hooks/useSmartContract";
import { removeNotification } from "@/lib/utils";

interface PageProps {
  provider: IPortkeyProvider | null;
  currentWalletAddress?: string;
}

const formSchema = z.object({
  address: z.string().min(1, "Address is required"),
});

const HomePage = ({ provider, currentWalletAddress }: PageProps) => {
  const { roleContract } = useSmartContract(provider);
  const [isModalOpen, setModalOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [formLoading, setFormLoading] = useState<boolean>(false);
  const [isContractInitialize, setIsContractInitialize] =
    useState<boolean>(false);
  const [roleType, setRoleType] = useState("");
  const [role, setRole] = useState({
    Admin: "",
    Parent: "",
    Child: "",
  });

  const currentRole = useMemo(() => {
    if (!currentWalletAddress) {
      return null;
    }
    switch (currentWalletAddress) {
      case role.Admin:
        return ROLE.admin;
      case role.Parent:
        return ROLE.parent;
      case role.Child:
        return ROLE.child;
      default:
        null;
    }
  }, [currentWalletAddress, role]);

  // Step F - Configure Role Form
  const form = useForm<z.infer<typeof formSchema>>();

  // Function to handle opening the modal
  const handleOpenModal = (
    type: string,      // Type of role (Admin, Parent, or Child)
    isEdit?: boolean,  // Optional flag to indicate if it's an edit action
    address?: string   // Optional address to pre-fill the form if editing
  ) => {
    // Open the modal
    setModalOpen(true);
    // Set the role type (Admin, Parent, or Child) based on the action
    setRoleType(type);
    // If it's an edit and an address is provided, pre-fill the form with the address
    if (isEdit && address) {
      form.setValue("address", address);  // Set the address value in the form
    }
  };

  // Function to handle closing the modal
  const handleCloseModal = () => {
    // Clear the role type when the modal is closed
    setRoleType("");
    // Close the modal
    setModalOpen(false);
    // Reset the form fields to their default values
    form.reset();
    // Stop any form loading indicators
    setFormLoading(false);
  };

  // Step G - Initialize Role Contract
  const initializeContract = async () => {};

  // Step H - Set User Role
  const setAuthority = async (values: { address: string }, type: string) => {};

  // Step I - Handle Submit Form
  const onSubmit = async (values: { address: string }) => {};

  // Step H - Get Role Data
  const getAuthorityData = async () => {};

  // Use Effect to Fetch Authority Data
  useEffect(() => {
    if (currentWalletAddress) {
      getAuthorityData();
    }
  }, [currentWalletAddress]);

  return (
    <div className="home-container">
      <div className="allowance-container">
        {currentWalletAddress && currentRole ? (
          currentRole === ROLE.admin ? (
            <AdminComponent
              currentWalletAddress={currentWalletAddress}
              role={role}
              handleOpenModal={handleOpenModal}
            />
          ) : currentRole === ROLE.parent ? (
            <ParentComponent
              currentWalletAddress={currentWalletAddress}
              provider={provider}
              role={role}
              handleOpenModal={handleOpenModal}
            />
          ) : (
            currentRole === ROLE.child && (
              <ChildComponent
                currentWalletAddress={currentWalletAddress}
                provider={provider}
              />
            )
          )
        ) : loading ? (
          <div className="bordered-container no-wallet">
            <strong>Loading Data...</strong>
          </div>
        ) : !currentWalletAddress ? (
          <div className="bordered-container no-wallet">
            <strong>
              Please connect your Portkey Wallet and access the Allowance
              smart contract
            </strong>
          </div>
        ) : (
          !currentRole && (
            <div className="bordered-container no-wallet">
              <strong>
                You are not authorise with contract. Please connect wallet with
                authorized portkey wallet.
              </strong>
            </div>
          )
        )}
        <Modal
          isVisible={isModalOpen}
          title={"Set " + roleType}
          onClose={() => handleCloseModal()}
        >
          <Form {...form}>
            <form
              onSubmit={form.handleSubmit(onSubmit)}
              className="space-y-8 modal-form"
            >
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="address"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Address</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter address" {...field} />
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
                  {"Set " + roleType}
                </Button>
              </div>
            </form>
          </Form>
        </Modal>
      </div>
    </div>
  );
};

export default HomePage;
