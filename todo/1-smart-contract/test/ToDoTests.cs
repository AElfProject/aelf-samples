using AElf.ContractTestKit;
using AElf.Kernel;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;
using System.Threading.Tasks;

namespace AElf.Contracts.ToDo.Tests
{
    public class ToDoTests : ToDoContractTestBase
    {
        private void InitializeContract()
        {
            var result = ToDoContractStub.Initialize.SendAsync(new Empty()).Result;
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        }

        [Fact]
        public void Initialize_Contract_Test()
        {
            InitializeContract();

            var status = ToDoContractStub.GetInitialStatus.CallAsync(new Empty()).Result;
            status.Value.ShouldBeTrue();
        }

        [Fact]
        public void Initialize_AlreadyInitialized_Test()
        {
            InitializeContract();
            
            // Try to initialize again
            var result = ToDoContractStub.Initialize.SendAsync(new Empty()).Result;
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var status = ToDoContractStub.GetInitialStatus.CallAsync(new Empty()).Result;
            status.Value.ShouldBeTrue();
        }

        [Fact]
        public void CreateTask_NotInitialized_Test()
        {
            var taskInput = new TaskInput
            {
                Name = "Test Task",
                Description = "This is a test task.",
                Category = "General"
            };

            var result = ToDoContractStub.CreateTask.SendAsync(taskInput).Result;
            result.Output.Value.ShouldBe("Contract not initialized.");
        }

        [Fact]
        public void CreateTask_Test()
        {
            InitializeContract();

            var taskInput = new TaskInput
            {
                Name = "Test Task",
                Description = "This is a test task.",
                Category = "General"
            };

            var result = ToDoContractStub.CreateTask.SendAsync(taskInput).Result;
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var taskId = result.Output.Value;
            var task = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = taskId }).Result;

            task.TaskId.ShouldBe(taskId);
            task.Name.ShouldBe(taskInput.Name);
            task.Description.ShouldBe(taskInput.Description);
            task.Category.ShouldBe(taskInput.Category);
            task.Status.ShouldBe("pending");
            task.CreatedAt.ShouldBe(task.UpdatedAt); // Timestamps should be equal for new task
            task.Owner.ShouldBe(DefaultSender.ToString().Trim('"'));
        }

        [Fact]
        public void CreateTask_SequentialIds_Test()
        {
            InitializeContract();

            var taskInput1 = new TaskInput
            {
                Name = "Task 1",
                Description = "First task",
                Category = "General"
            };

            var taskInput2 = new TaskInput
            {
                Name = "Task 2",
                Description = "Second task",
                Category = "General"
            };

            var result1 = ToDoContractStub.CreateTask.SendAsync(taskInput1).Result;
            var result2 = ToDoContractStub.CreateTask.SendAsync(taskInput2).Result;

            var taskId1 = int.Parse(result1.Output.Value);
            var taskId2 = int.Parse(result2.Output.Value);

            taskId2.ShouldBe(taskId1 + 1);
        }

        [Fact]
        public void UpdateTask_NonExistent_Test()
        {
            InitializeContract();

            var updateInput = new TaskUpdateInput
            {
                TaskId = "999",
                Name = "Updated Task",
                Description = "Task has been updated.",
                Category = "Work",
                Status = "completed"
            };

            var updateResult = ToDoContractStub.UpdateTask.SendAsync(updateInput).Result;
            updateResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var task = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = "999" }).Result;
            task.Name.ShouldBe("Task not found.");
        }

        [Fact]
        public void UpdateTask_PartialUpdate_Test()
        {
            InitializeContract();

            // Create a task first
            var taskInput = new TaskInput
            {
                Name = "Original Task",
                Description = "Original description",
                Category = "General"
            };

            var createResult = ToDoContractStub.CreateTask.SendAsync(taskInput).Result;
            var taskId = createResult.Output.Value;
            var originalTask = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = taskId }).Result;

            // Update only the name and description
            var updateInput = new TaskUpdateInput
            {
                TaskId = taskId,
                Name = "Updated Task",
                Description = "Updated description",
                Category = originalTask.Category,
                Status = originalTask.Status
            };

            var updateResult = ToDoContractStub.UpdateTask.SendAsync(updateInput).Result;
            updateResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var updatedTask = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = taskId }).Result;

            updatedTask.Name.ShouldBe("Updated Task");
            updatedTask.Description.ShouldBe("Updated description");
            updatedTask.Category.ShouldBe(originalTask.Category);
            updatedTask.Status.ShouldBe(originalTask.Status);
        }

        [Fact]
        public void UpdateTask_Test()
        {
            InitializeContract();

            var taskInput = new TaskInput
            {
                Name = "Task to Update",
                Description = "Update this task.",
                Category = "General"
            };

            var createResult = ToDoContractStub.CreateTask.SendAsync(taskInput).Result;
            var taskId = createResult.Output.Value;

            var updateInput = new TaskUpdateInput
            {
                TaskId = taskId,
                Name = "Updated Task",
                Description = "Task has been updated.",
                Category = "Work",
                Status = "completed"
            };

            var updateResult = ToDoContractStub.UpdateTask.SendAsync(updateInput).Result;
            updateResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var updatedTask = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = taskId }).Result;

            updatedTask.Name.ShouldBe(updateInput.Name);
            updatedTask.Description.ShouldBe(updateInput.Description);
            updatedTask.Category.ShouldBe(updateInput.Category);
            updatedTask.Status.ShouldBe(updateInput.Status);
        }

        [Fact]
        public void DeleteTask_NonExistent_Test()
        {
            InitializeContract();

            var deleteResult = ToDoContractStub.DeleteTask.SendAsync(new StringValue { Value = "999" }).Result;
            deleteResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var taskList = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;
            taskList.Tasks.Count.ShouldBe(0);
        }

        [Fact]
        public void DeleteTask_Test()
        {
            InitializeContract();

            var taskInput = new TaskInput
            {
                Name = "Task to Delete",
                Description = "This task will be deleted.",
                Category = "General"
            };

            var createResult = ToDoContractStub.CreateTask.SendAsync(taskInput).Result;
            var taskId = createResult.Output.Value;

            var deleteResult = ToDoContractStub.DeleteTask.SendAsync(new StringValue { Value = taskId }).Result;
            deleteResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var deletedTask = ToDoContractStub.GetTask.CallAsync(new StringValue { Value = taskId }).Result;
            deletedTask.Name.ShouldBe("Task not found.");
        }

        [Fact]
        public void DeleteTask_MultipleTasks_Test()
        {
            InitializeContract();

            // Create three tasks
            var taskInput1 = new TaskInput { Name = "Task 1", Description = "First task", Category = "General" };
            var taskInput2 = new TaskInput { Name = "Task 2", Description = "Second task", Category = "Work" };
            var taskInput3 = new TaskInput { Name = "Task 3", Description = "Third task", Category = "Personal" };

            var taskId1 = ToDoContractStub.CreateTask.SendAsync(taskInput1).Result.Output.Value;
            var taskId2 = ToDoContractStub.CreateTask.SendAsync(taskInput2).Result.Output.Value;
            var taskId3 = ToDoContractStub.CreateTask.SendAsync(taskInput3).Result.Output.Value;

            // Delete the second task
            var deleteResult = ToDoContractStub.DeleteTask.SendAsync(new StringValue { Value = taskId2 }).Result;
            deleteResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check task list
            var taskList = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;
            taskList.Tasks.Count.ShouldBe(2);
            taskList.Tasks[0].Name.ShouldBe("Task 1");
            taskList.Tasks[1].Name.ShouldBe("Task 3");
        }

        [Fact]
        public void ListTasks_EmptyList_Test()
        {
            InitializeContract();

            var taskList = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;
            taskList.Tasks.Count.ShouldBe(0);
        }

        [Fact]
        public void ListTasks_DifferentOwners_Test()
        {
            InitializeContract();

            // Create tasks
            var taskInput1 = new TaskInput { Name = "Task 1", Description = "First task", Category = "General" };
            var taskInput2 = new TaskInput { Name = "Task 2", Description = "Second task", Category = "Work" };

            var result1 = ToDoContractStub.CreateTask.SendAsync(taskInput1).Result;
            result1.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var result2 = ToDoContractStub.CreateTask.SendAsync(taskInput2).Result;
            result2.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // List tasks for current owner
            var ownerTasks = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;
            ownerTasks.Tasks.Count.ShouldBe(2);

            // List tasks for different owner
            var differentOwner = "different_owner";
            var otherOwnerTasks = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = differentOwner }).Result;
            otherOwnerTasks.Tasks.Count.ShouldBe(0);
        }

        [Fact]
        public void ListTasks_Test()
        {
            InitializeContract();

            var taskInput1 = new TaskInput
            {
                Name = "Task 1",
                Description = "First task.",
                Category = "General"
            };

            var taskInput2 = new TaskInput
            {
                Name = "Task 2",
                Description = "Second task.",
                Category = "Work"
            };

            var result1 = ToDoContractStub.CreateTask.SendAsync(taskInput1).Result;
            result1.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var result2 = ToDoContractStub.CreateTask.SendAsync(taskInput2).Result;
            result2.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var taskList = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;

            taskList.Tasks.Count.ShouldBe(2);
        }
    }
}
