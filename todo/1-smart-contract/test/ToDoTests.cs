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

            ToDoContractStub.CreateTask.SendAsync(taskInput1).Wait();
            ToDoContractStub.CreateTask.SendAsync(taskInput2).Wait();

            var taskList = ToDoContractStub.ListTasks.CallAsync(new StringValue { Value = DefaultSender.ToString().Trim('"') }).Result;

            taskList.Tasks.Count.ShouldBe(2);
        }
    }
}
