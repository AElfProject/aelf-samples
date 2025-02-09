import { useEffect, useMemo, useState } from "react";
import { IPortkeyProvider } from "@portkey/provider-types";
import { Controller, useForm } from "react-hook-form";
import * as z from "zod";
import Select from "react-select";
import { Id, toast } from "react-toastify";

import "./home.scss";
import { Button } from "@/components/ui/button";
import { CATEGORY_OPTIONS, FILTER_TYPE, TASK_STATUS } from "@/lib/constant";
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
import { PlusIcon } from "@/components/ui/icons";
import { removeNotification } from "@/lib/utils";
import useTodoSmartContract from "@/hooks/useTodoSmartContract";
import PageFilter from "@/components/page-filter";
import TodoCard from "@/components/todo-card";

interface ITodoObject {
  name: string;
  description: string;
  category: string;
  createdAt: string;
  status: string;
  taskId: string;
  updatedAt: string;
}

interface Option {
  value: string;
  label: string;
};

interface PageProps {
  provider: IPortkeyProvider | null;
  currentWalletAddress?: string;
}

const formSchema = z.object({
  name: z.string().min(1, "Name is required"),
  description: z.string().min(1, "Description is required"),
  selectedCategory: z
    .object({
      value: z.string(),
      label: z.string(),
    })
    .nullable()
    .refine((val) => val !== null, "Category is required"),
});

const HomePage = ({ provider, currentWalletAddress }: PageProps) => {
  const smartContract = useTodoSmartContract(provider);

  const [todoData, setTodoData] = useState<ITodoObject[] | []>([]);
  const [updateId, setUpdateId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<Option | null>();
  const [selectedFilter, setSelectedFilter] = useState<string>(FILTER_TYPE.all);

  const [isContractInitialized, setIsContractInitialized] =
    useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(true);
  const [formLoading, setFormLoading] = useState<boolean>(false);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);


  // Step D - Configure Todo Form
  const form = useForm<z.infer<typeof formSchema>>();

  // ============ Start Filter Data ============ //
  const pendingTask = useMemo(() => {
    if (todoData.length === 0) {
      return todoData;
    }
    return todoData.filter(
      (data: ITodoObject) => data.status.toLowerCase() === TASK_STATUS.pending
    );
  }, [todoData, selectedFilter, updateId]);

  const completedTask = useMemo(() => {
    if (todoData.length === 0) {
      return todoData;
    }
    return todoData.filter(
      (data: ITodoObject) => data.status.toLowerCase() === TASK_STATUS.completed
    );
  }, [todoData, selectedFilter, updateId]);

  const removedTask = useMemo(() => {
    if (todoData.length === 0) {
      return todoData;
    }
    return todoData.filter(
      (data: ITodoObject) => data.status.toLowerCase() === TASK_STATUS.removed
    );
  }, [todoData, selectedFilter, updateId]);

  const filteredTask = useMemo(() => {
    switch (selectedFilter) {
      case TASK_STATUS.pending:
        return pendingTask
      case TASK_STATUS.completed:
        return completedTask
      case TASK_STATUS.removed:
        return removedTask
      default:
        return todoData
    }
  }, [selectedFilter, completedTask, pendingTask]);

  // ============ End Filter Data ============ //


  const handleCloseModal = () => {
    form.reset();
    setIsModalOpen(false);
    setUpdateId(null);
    setSelectedCategory(null);
  };

  // ============ Start ToDo dApp Functions ============ //

  // step 8 - Get Todo Data from User's wallet using contract
  const getTodoData = async () => {};

  // step 1 - Check If Contract is Initialized or not 
  const checkIsContractInitialized = async () => {};

  // step 2 - Initialize the smart contract
  const initializeContract = async () => {};

  // Check whether contract initialized or not
  useEffect(() => {
    smartContract && checkIsContractInitialized();
  }, [smartContract]);

  // Use Effect to Fetch NFTs
  useEffect(() => {
    if (currentWalletAddress) {
      getTodoData();
    }
  }, [currentWalletAddress]);

  // step 3 - Create a New Task using Smart Contract
  const createNewTask = async () => {};

  // step 4 - Update the Task
  const updateTask = async () => {};

  // step 5- Update Status from Pending to Completed of the Task
  const completeTask = async () => {};

  // step 6 - Delete the Task
  const deleteTask = async () => {};

  // ============ End ToDo dApp Functions ============ //

  
  // step 7 - Handle Submit Form
  const onSubmit = async () => {};

  const onEditHandle = (data: ITodoObject) => {
    // set the UpdateId state
    setUpdateId(data.taskId);

    // set the default form value for edit task
    form.setValue("name", data.name);
    form.setValue("description", data.description);
    const categoryObj = {
      label: data.category.charAt(0).toUpperCase() + data.category.slice(1),
      value: data.category,
    };
    form.setValue("selectedCategory", categoryObj);
    setSelectedCategory(categoryObj);

    // open the form modal
    setIsModalOpen(true);
  };

  return (
    <div className="home-container">
      <div className="todo-collection-container">
        <div className="todo-collection-head">
          <h2>All Tasks</h2>
          <div className="button-wrapper">
            <Button
              className="header-button"
              disabled={!currentWalletAddress}
              onClick={() => setIsModalOpen(true)}
            >
              <PlusIcon />
              Add New
            </Button>
          </div>
        </div>
        <PageFilter
          selectedFilter={selectedFilter}
          setSelectedFilter={setSelectedFilter}
          allLength={todoData.length}
          pendingLength={pendingTask.length}
          completedLength={completedTask.length}
          removedLength={removedTask.length}
        />
        {currentWalletAddress ? (
          <div className="todo-collection">
            {filteredTask.length > 0 ? (
              filteredTask.slice(0, 5).map((data: ITodoObject, index) => {
                return (
                  <TodoCard
                    data={data}
                    index={index}
                    onEditTaskHandle={onEditHandle}
                    onCompleteTaskHandle={completeTask}
                    onDeleteTaskHandle={deleteTask}
                    updateId={updateId}
                    deletingId={deletingId}
                  />
                );
              })
            ) : loading ? (
              <div className="bordered-container">
                <strong>Loading...</strong>
              </div>
            ) : (
              <div className="bordered-container">
                <strong>
                  It's Look like you haven't created any Todo Item yet
                </strong>
              </div>
            )}
          </div>
        ) : (
          <div className="bordered-container no-wallet">
            <strong>
              Please connect your Portkey Wallet and Create a new Todo List.
            </strong>
          </div>
        )}
        <Modal
          isVisible={isModalOpen}
          title={(updateId ? "Update" : "Create New") + " Task"}
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
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Name</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Token Name" {...field} />
                      </FormControl>
                      <FormMessage className="error-message" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="input-group">
                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Description</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter Symbol" {...field} />
                      </FormControl>
                      <FormMessage className="error-message" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="select-container">
                <FormField
                  control={form.control}
                  name="selectedCategory"
                  render={() => (
                    <FormItem>
                      <FormLabel>Select Types</FormLabel>
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
                                setSelectedCategory(opt);
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
              <div className="button-container">
                <Button
                  type="submit"
                  className="submit-btn"
                  disabled={formLoading}
                >
                  {!!updateId ? "Update" : "Create New"} Task
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
