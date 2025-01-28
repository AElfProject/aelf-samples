import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import * as z from "zod";
import Select from "react-select";
import { zodResolver } from "@hookform/resolvers/zod";
import { Id, toast } from "react-toastify";

import {
  convertAmountToToken,
  convertTokenToAmount,
  getCurrentDate,
  handleError,
  removeNotification,
} from "@/lib/utils";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import "./create-campaign.scss";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { CATEGORY_OPTIONS } from "@/lib/constant";
import useDonationSmartContract from "@/hooks/useDonationSmartContract";
import { useNavigate, useParams } from "react-router-dom";
import { PageProps } from "@/types/page";

// Option type for category dropdown
interface Option {
  value: string; // Category value
  label: string; // Category label
}

// Define form validation schema using Zod
const formSchema = z.object({
  title: z.string().min(1, "Title is required"), // Title is mandatory
  description: z.string().min(1, "Description is required"), // Description is mandatory
  selectedCategory: z
    .object({
      value: z.string(),
      label: z.string(),
    })
    .nullable()
    .refine((val) => val !== null, "Category is required"), // Category must be selected
  goal: z.number().min(1, "Goal is required"), // Goal amount is mandatory
  endDate: z.string().min(1, "End Date is required"), // End date is mandatory
  imageUrl: z
    .string()
    .url("Please enter a valid URL")
    .min(1, "Campaign image is required"), // Image URL is mandatory and must be valid
});

// Main component for creating/editing a campaign
const CreateCampaign = ({ provider, currentWalletAddress }: PageProps) => {
  const smartContract = useDonationSmartContract(provider); // Hook to interact with the smart contract
  const [formLoading, setFormLoading] = useState<boolean>(false); // State to show form loading spinner
  const [isContractInitialized, IsContractInitialized] = useState<boolean>(false); // State to track contract initialization
  const navigate = useNavigate(); // For navigation
  const { id: editCampaignId } = useParams(); // Get campaign ID from the route params

  // Step 10 - Configure campaign form
  const form = useForm<z.infer<typeof formSchema>>();

  // step 11 - Check if the smart contract is initialized
  const checkIsContractInitialized = async () => {};

  // step 12 - Initialize the smart contract if not already initialized
  const initializeContract = async () => {};

  // step 13 - Function to create a new campaign
  const createCampaign = async () => {};

  // step 14 - Function to edit an existing campaign
  const editCampaign = async () => {};

  // step 15 - Handle form submission for creating/editing a campaign
  const onSubmit = (values: z.infer<typeof formSchema>) => {};

  // step 16 - Set form data for editing an existing campaign
  const setEditFormData = async () => {};

  // Run once on component mount: Check contract initialization
  useEffect(() => {
    smartContract && checkIsContractInitialized();
  }, [smartContract]);

  // If editing, set form data once the campaign ID is available
  useEffect(() => {
    if (editCampaignId && smartContract) {
      setEditFormData();
    }
  }, [editCampaignId, smartContract]);

  return (
    <div className="create-campaign container">
      <h2>{editCampaignId ? "Edit Campaign" : "Start a Campaign"}</h2>
      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="space-y-8 modal-form"
        >
          <div className="input-group row">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Title</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter Title" {...field} />
                  </FormControl>
                  <FormMessage className="error-message" />
                </FormItem>
              )}
            />
            <div className="select-container">
              <FormField
                control={form.control}
                name="selectedCategory"
                render={() => (
                  <FormItem>
                    <FormLabel>Select Category</FormLabel>
                    <FormControl>
                      <Controller
                        name="selectedCategory"
                        control={form.control}
                        render={({ field }) => (
                          <Select
                            value={field.value}
                            options={CATEGORY_OPTIONS}
                            //@ts-ignore
                            onChange={(opt: Option) => {
                              field.onChange(opt);
                            }}
                            placeholder="Select Category"
                          />
                        )}
                      />
                    </FormControl>
                    <FormMessage className="error-message" />
                  </FormItem>
                )}
              />
            </div>
          </div>
          <div className="input-group">
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description</FormLabel>
                  <FormControl>
                    <textarea
                      rows={5}
                      placeholder="Enter Description"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage className="error-message" />
                </FormItem>
              )}
            />
          </div>

          <div className="input-group row">
            <FormField
              control={form.control}
              name="goal"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Goal</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Enter "
                      {...field}
                      onChange={({ target: { value } }) => {
                        const numericValue = value ? parseFloat(value) : 0;
                        form.setValue("goal", numericValue); // Convert to number
                        if (!value) {
                          form.setError("goal", {
                            message: "Goal is required",
                          });
                        } else {
                          form.clearErrors("goal");
                        }
                      }}
                    />
                  </FormControl>
                  <FormMessage className="error-message" />
                </FormItem>
              )}
            />
            {!editCampaignId && (
              <FormField
                control={form.control}
                name="endDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>End Date</FormLabel>
                    <FormControl>
                      <Input
                        type="date"
                        min={getCurrentDate()}
                        placeholder="Select End Date"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage className="error-message" />
                  </FormItem>
                )}
              />
            )}
          </div>

          <div className="input-group">
            <FormField
              control={form.control}
              name="imageUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Campaign Image</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter Campaign Image" {...field} />
                  </FormControl>
                  <FormMessage className="error-message" />
                </FormItem>
              )}
            />
          </div>

          <div className="button-container">
            <Button className="outline" onClick={() => navigate("/")}>
              Back to Home
            </Button>
            <Button type="submit" className="submit-btn" disabled={formLoading}>
              {!!editCampaignId ? "Update" : "Submit New"} Campagin
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default CreateCampaign;
