using System;
using System.Threading.Tasks;
using AElf.Types;
using AElf.ContractTestKit;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;
using AElf.Cryptography.ECDSA;
using System.Linq;

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
        public async Task GetPlayAmountLimit_Success()
        {
            // Act
            var result = await LotteryGameStub.GetPlayAmountLimit.CallAsync(new Empty());
            
            // Assert
            result.MinimumAmount.ShouldBe(1_000_000); // 0.01 ELF
            result.MaximumAmount.ShouldBe(1_000_000_000); // 10 ELF
        }

        [Fact]
        public async Task Play_BelowMinimumAmount_Fail()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            const long playAmount = 500_000; // 0.005 ELF, below minimum

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(async () =>
                await LotteryGameStub.Play.SendAsync(new Int64Value { Value = playAmount }));
            exception.Message.ShouldContain("Invalid play amount");
        }

        [Fact]
        public async Task Play_AboveMaximumAmount_Fail()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            const long playAmount = 1_500_000_000; // 15 ELF, above maximum

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(async () =>
                await LotteryGameStub.Play.SendAsync(new Int64Value { Value = playAmount }));
            exception.Message.ShouldContain("Invalid play amount");
        }

        [Fact]
        public async Task Deposit_NotOwner_Fail()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            const long depositAmount = 10_000_000;
            var nonOwnerStub = GetLotteryGameContractStub(Accounts[1].KeyPair);

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(async () =>
                await nonOwnerStub.Deposit.SendAsync(new Int64Value { Value = depositAmount }));
            exception.Message.ShouldContain("Unauthorized");
        }

        [Fact]
        public async Task Withdraw_NotOwner_Fail()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            const long withdrawAmount = 5_000_000;
            var nonOwnerStub = GetLotteryGameContractStub(Accounts[1].KeyPair);

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(async () =>
                await nonOwnerStub.Withdraw.SendAsync(new Int64Value { Value = withdrawAmount }));
            exception.Message.ShouldContain("Unauthorized");
        }

        [Fact]
        public async Task TransferOwnership_Success()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            var newOwner = Accounts[1].Address;

            // Act
            var result = await LotteryGameStub.TransferOwnership.SendAsync(newOwner);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            var owner = await LotteryGameStub.GetOwner.CallAsync(new Empty());
            owner.Value.ShouldBe(newOwner.ToBase58());
        }

        [Fact]
        public async Task TransferOwnership_Fail_NotOwner()
        {
            // Arrange
            await LotteryGameStub.Initialize.SendAsync(new Empty());
            var newOwner = Accounts[1].Address;
            var notOwner = GetLotteryGameContractStub(Accounts[2].KeyPair);

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () => await notOwner.TransferOwnership.SendAsync(newOwner));
        }

        private LotteryGameContainer.LotteryGameStub GetLotteryGameContractStub(ECKeyPair keyPair)
        {
            return GetTester<LotteryGameContainer.LotteryGameStub>(ContractAddress, keyPair);
        }
    }
}