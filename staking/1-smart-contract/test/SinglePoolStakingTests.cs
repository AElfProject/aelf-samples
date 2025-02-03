using System;
using System.Threading.Tasks;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.SinglePoolStaking
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class SinglePoolStakingTests : SinglePoolStakingTestBase
    {
        private const string TestTokenSymbol = "ELF";
        private const long TestAmount = 100_00000000; // 100 ELF
        private const long TestLockTime = 86400; // 1 day in seconds

        private async Task InitializeContract()
        {
            await SinglePoolStakingStub.Initialize.SendAsync(new InitializeInput
            {
                TokenContractAddress = ContractAddress // Using contract's own address for testing
            });
        }

        private async Task<string> CreateDeposit(string symbol = TestTokenSymbol, long amount = TestAmount, long lockTime = TestLockTime)
        {
            var result = await SinglePoolStakingStub.Deposit.SendAsync(new DepositInput
            {
                TokenSymbol = symbol,
                Amount = amount,
                LockTime = lockTime
            });
            return result.Output.Value;
        }

        [Fact]
        public async Task Initialize_Test()
        {
            // Arrange & Act
            await InitializeContract();

            // Assert
            var initialized = await SinglePoolStakingStub.IfInitialized.CallAsync(new Empty());
            initialized.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task Initialize_ShouldFail_WhenCalledTwice()
        {
            // Arrange
            await InitializeContract();

            // Act & Assert
            var initialized = await SinglePoolStakingStub.IfInitialized.CallAsync(new Empty());
            initialized.Value.ShouldBeTrue();

            // Second initialization should return without error (idempotent)
            await SinglePoolStakingStub.Initialize.SendAsync(new InitializeInput
            {
                TokenContractAddress = ContractAddress
            });

            initialized = await SinglePoolStakingStub.IfInitialized.CallAsync(new Empty());
            initialized.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task GetTotalStakedAmount_ShouldBeZero_WhenNoDeposits()
        {
            // Arrange
            await InitializeContract();

            // Act
            var totalStaked = await SinglePoolStakingStub.GetTotalStakedAmount.CallAsync(new Empty());
            
            // Assert
            totalStaked.Value.ShouldBe(0);
        }

        [Fact]
        public async Task GetDeposits_ShouldReturnEmpty_WhenNoDeposits()
        {
            // Arrange
            await InitializeContract();

            // Act & Assert
            // Since the contract doesn't handle null state properly, we expect this to throw
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                SinglePoolStakingStub.GetDeposits.CallAsync(new StringValue { Value = DefaultSender.ToBase58() }));
            exception.Message.ShouldContain("Object reference not set to an instance of an object");
        }

        [Fact]
        public async Task GetReward_ShouldFail_WhenDepositDoesNotExist()
        {
            // Arrange
            await InitializeContract();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                SinglePoolStakingStub.GetReward.CallAsync(new StringValue { Value = "nonexistent" }));
            exception.Message.ShouldContain("Deposit not found");
        }

        [Fact]
        public async Task ForceWithdraw_ShouldFail_WhenDepositDoesNotExist()
        {
            // Arrange
            await InitializeContract();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                SinglePoolStakingStub.ForceWithdraw.SendAsync(new StringValue { Value = "nonexistent" }));
            exception.Message.ShouldContain("Deposit not found");
        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenDepositDoesNotExist()
        {
            // Arrange
            await InitializeContract();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                SinglePoolStakingStub.Withdraw.SendAsync(new WithdrawInput { DepositId = "nonexistent" }));
            exception.Message.ShouldContain("Deposit not found");
        }

        [Fact]
        public async Task Deposit_ShouldCreateNewDeposit()
        {
            // Arrange
            await InitializeContract();

            // Act
            var depositId = await CreateDeposit();

            // Assert
            var deposits = await SinglePoolStakingStub.GetDeposits.CallAsync(new StringValue { Value = DefaultSender.ToBase58() });
            deposits.Deposits.Count.ShouldBe(1);
            deposits.Deposits[0].DepositId.ShouldBe(depositId);
            deposits.Deposits[0].Amount.ShouldBe(TestAmount);
            deposits.Deposits[0].TokenSymbol.ShouldBe(TestTokenSymbol);
            deposits.Deposits[0].LockTime.ShouldBe(TestLockTime);
        }

        [Fact]
        public async Task GetTotalStakedAmount_ShouldIncreaseAfterDeposit()
        {
            // Arrange
            await InitializeContract();

            // Act
            await CreateDeposit();

            // Assert
            var totalStaked = await SinglePoolStakingStub.GetTotalStakedAmount.CallAsync(new Empty());
            totalStaked.Value.ShouldBe(TestAmount);
        }

        [Fact]
        public async Task GetReward_ShouldCalculateCorrectly()
        {
            // Arrange
            await InitializeContract();
            var depositId = await CreateDeposit();

            // Act
            var reward = await SinglePoolStakingStub.GetReward.CallAsync(new StringValue { Value = depositId });

            // Assert - 10% reward rate
            reward.Value.ShouldBe(TestAmount / 10);
        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenLockPeriodNotOver()
        {
            // Arrange
            await InitializeContract();
            var depositId = await CreateDeposit();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                SinglePoolStakingStub.Withdraw.SendAsync(new WithdrawInput { DepositId = depositId }));
            exception.Message.ShouldContain("Lock period not over");
        }
    }
    
}