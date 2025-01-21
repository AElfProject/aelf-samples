using System;
using System.Threading.Tasks;
using AElf.Types;
using AElf.ContractTestKit;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.LotteryGame
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class LotteryGameTests : TestBase
    {
        [Fact]
        public async Task InitializeContract_Success()
        {
            // Act
            var result = await LotteryGameStub.Initialize.SendAsync(new Empty());
            
            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var owner = await LotteryGameStub.GetOwner.CallAsync(new Empty());
            owner.Value.ShouldBe(Accounts[0].Address.ToBase58());
        }

        [Fact]
        public async Task InitializeContract_Fail_AlreadyInitialized()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());

            // Act & Assert
            Should.Throw<Exception>(async () => await LotteryGameStub.Initialize.SendAsync(new Empty()));
        }

        [Fact]
        public async Task PlayLottery_WithinLimits_Success()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            
            const long playAmount = 5_000_000; // 0.05 ELF, within limits
            var playInput = new Int64Value() { Value = playAmount };

            // Setup contract balance
            await LotteryGameStub.Deposit.SendAsync(new Int64Value { Value = 50_00000000 });
            var initialContractBalance = await GetContractBalanceAsync();

            // Act
            var result = await LotteryGameStub.Play.SendAsync(playInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check balance update
            var finalContractBalance = await GetContractBalanceAsync();
            var contractDiffBalance = finalContractBalance - initialContractBalance;
            Math.Abs(contractDiffBalance).ShouldBe(playAmount);

            // Check if the event is emitted
            var events = result.TransactionResult.Logs;
            events.ShouldContain(log => log.Name == nameof(PlayOutcomeEvent));
        }

        [Fact]
        public void PlayLottery_ExceedsMaximumAmount_Fail()
        {
            // Arrange
            LotteryGameStub.Initialize.SendAsync(new Empty());

            const long playAmount = 1_500_000_000; // 15 ELF, exceeds limit
            var playInput = new Int64Value() { Value = playAmount };

            // Act & Assert
            Should.Throw<Exception>(async () => await LotteryGameStub.Play.SendAsync(playInput));
        }

        [Fact]
        public void PlayLottery_BelowMinimumAmount_Fail()
        {
            // Arrange
            LotteryGameStub.Initialize.SendAsync(new Empty());

            const long playAmount = 500_000; // 0.005 ELF, below limit
            var playInput = new Int64Value() { Value = playAmount };

            // Act & Assert
            Should.Throw<Exception>(async () => await LotteryGameStub.Play.SendAsync(playInput));
        }
        
        [Fact]
        public async Task Deposit_Success()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());

            const long depositAmount = 10_000_000; // 0.1 ELF
            var depositInput = new Int64Value() { Value = depositAmount };

            var initialContractBalance = await GetContractBalanceAsync();
            
            // Act
            var result = await LotteryGameStub.Deposit.SendAsync(depositInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check balance update
            var finalContractBalance = await GetContractBalanceAsync();
            finalContractBalance.ShouldBe(initialContractBalance + depositAmount);

            // Check if the event is emitted
            var events = result.TransactionResult.Logs;
            events.ShouldContain(log => log.Name == nameof(DepositEvent));
        }

        [Fact]
        public async Task Withdraw_Success()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());

            const long depositAmount = 10_000_000; // 0.1 ELF
            var depositInput = new Int64Value() { Value = depositAmount };
            await LotteryGameStub.Deposit.SendAsync(depositInput);

            const long withdrawAmount = 5_000_000; // 0.05 ELF
            var withdrawInput = new Int64Value() { Value = withdrawAmount };

            var initialContractBalance = await GetContractBalanceAsync();
            
            // Act
            var result = await LotteryGameStub.Withdraw.SendAsync(withdrawInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check balance update
            var finalContractBalance = await GetContractBalanceAsync();
            finalContractBalance.ShouldBe(initialContractBalance - withdrawAmount);

            // Check if the event is emitted
            var events = result.TransactionResult.Logs;
            events.ShouldContain(log => log.Name == nameof(WithdrawEvent));
        }

        [Fact]
        public async Task Withdraw_InsufficientBalance_Fail()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());

            long withdrawAmount = 5_000_000; // 0.05 ELF
            var withdrawInput = new Int64Value() { Value = withdrawAmount };

            // Act & Assert
            Should.Throw<Exception>(async () => await LotteryGameStub.Withdraw.SendAsync(withdrawInput));
        }

        private async Task<long> GetContractBalanceAsync()
        {
            return (await LotteryGameStub.GetContractBalance.CallAsync(new Empty())).Value;
        }
    }
}