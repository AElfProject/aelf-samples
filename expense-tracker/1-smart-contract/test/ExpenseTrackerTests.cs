using System;
using System.Threading.Tasks;
using AElf.Types;
using AElf.ContractTestKit;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;
using AElf.Cryptography.ECDSA;
using System.Linq;

namespace AElf.Contracts.ExpenseTracker
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class ExpenseTrackerTests : TestBase
    {
        [Fact]
        public async Task GetInitialStatus_ShouldReturnFalseByDefault()
        {
            // Act
            var result = await ExpenseTrackerStub.GetInitialStatus.CallAsync(new Empty());
            
            // Assert
            result.Value.ShouldBeFalse();
        }

        [Fact]
        public async Task Initialize_Success()
        {
            // Act
            var result = await ExpenseTrackerStub.Initialize.SendAsync(new Empty());
            
            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        }

        [Fact]
        public async Task AddExpense_Success()
        {
            // Arrange
            await ExpenseTrackerStub.Initialize.SendAsync(new Empty());
            var input = new ExpenseInput
            {
                Description = "Test Expense",
                Category = "Food",
                Amount = 100,
                Currency = "USD"
            };

            // Act
            var result = await ExpenseTrackerStub.AddExpense.SendAsync(input);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var expenseId = result.Output.Value;
            var expense = await ExpenseTrackerStub.GetExpense.CallAsync(new StringValue { Value = expenseId });
            expense.Description.ShouldBe("Test Expense");
            expense.Category.ShouldBe("Food");
            expense.Amount.ShouldBe(100);
            expense.Currency.ShouldBe("USD");
        }

        [Fact]
        public async Task UpdateExpense_Success()
        {
            // Arrange
            await ExpenseTrackerStub.Initialize.SendAsync(new Empty());
            var addResult = await ExpenseTrackerStub.AddExpense.SendAsync(new ExpenseInput
            {
                Description = "Original Description",
                Category = "Food",
                Amount = 100,
                Currency = "USD"
            });
            var expenseId = addResult.Output.Value;

            // Act
            var updateInput = new ExpenseUpdateInput
            {
                ExpenseId = expenseId,
                Description = "Updated Description",
                Amount = 200,
                Category = "Food",
                Currency = "USD"
            };
            var result = await ExpenseTrackerStub.UpdateExpense.SendAsync(updateInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var updatedExpense = await ExpenseTrackerStub.GetExpense.CallAsync(new StringValue { Value = expenseId });
            updatedExpense.Description.ShouldBe("Updated Description");
            updatedExpense.Amount.ShouldBe(200);
            updatedExpense.Category.ShouldBe("Food");
            updatedExpense.Currency.ShouldBe("USD");
        }

        [Fact]
        public async Task DeleteExpense_Success()
        {
            // Arrange
            await ExpenseTrackerStub.Initialize.SendAsync(new Empty());
            var addResult = await ExpenseTrackerStub.AddExpense.SendAsync(new ExpenseInput
            {
                Description = "To be deleted",
                Category = "Test",
                Amount = 100,
                Currency = "USD"
            });
            var expenseId = addResult.Output.Value;

            // Act
            var result = await ExpenseTrackerStub.DeleteExpense.SendAsync(new StringValue { Value = expenseId });

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var deletedExpense = await ExpenseTrackerStub.GetExpense.CallAsync(new StringValue { Value = expenseId });
            deletedExpense.Description.ShouldBe("Expense not found.");
        }

        [Fact]
        public async Task ListExpenses_Success()
        {
            // Arrange
            await ExpenseTrackerStub.Initialize.SendAsync(new Empty());
            await ExpenseTrackerStub.AddExpense.SendAsync(new ExpenseInput
            {
                Description = "Expense 1",
                Category = "Food",
                Amount = 100,
                Currency = "USD"
            });
            await ExpenseTrackerStub.AddExpense.SendAsync(new ExpenseInput
            {
                Description = "Expense 2",
                Category = "Transport",
                Amount = 200,
                Currency = "USD"
            });

            // Act
            var defaultKeyPair = Accounts[0].KeyPair;
            var defaultAddress = Address.FromPublicKey(defaultKeyPair.PublicKey);
            var result = await ExpenseTrackerStub.ListExpenses.CallAsync(
                new StringValue { Value = defaultAddress.ToBase58() });

            // Assert
            result.Expenses.Count.ShouldBe(2);
            result.Expenses[0].Description.ShouldBe("Expense 1");
            result.Expenses[1].Description.ShouldBe("Expense 2");
        }

        private ExpenseTrackerContainer.ExpenseTrackerStub GetExpenseTrackerContractStub(ECKeyPair keyPair)
        {
            return GetTester<ExpenseTrackerContainer.ExpenseTrackerStub>(ContractAddress, keyPair);
        }
    }
}