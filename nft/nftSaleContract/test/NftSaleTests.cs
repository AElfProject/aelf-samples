using System;
using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AElf.Contracts.Vote;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.NftSale
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class NftSaleTests : TestBase
    {
        [Fact]
        public async Task InitializeContract_Success()
        {
            var price = new Price
            {
                Amount = 4,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Act
            var result = await NftSaleStub.Initialize.SendAsync(nftPrice);
            
            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var owner = await NftSaleStub.GetOwner.CallAsync(new Empty());
            owner.Value.ShouldBe(DefaultAccount.Address.ToBase58());
        }

        [Fact]
        public async Task InitializeContract_Fail_AlreadyInitialized()
        {
            var price = new Price
            {
                Amount = 4,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Arrange
            await NftSaleStub.Initialize.SendAsync(nftPrice);

            // Act & Assert
            Should.Throw<Exception>(async () => await NftSaleStub.Initialize.SendAsync(nftPrice));
        }
        
        [Fact]
        public async Task SetPrice_and_GetPrice()
        {
            var price = new Price
            {
                Amount = 2,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Arrange
            await NftSaleStub.Initialize.SendAsync(nftPrice);
            var priceNew = new Price
            {
                Amount = 4,
                Symbol = "ELF"
            };

            await NftSaleStub.SetPriceAndSymbol.SendAsync(new NftPrice
            {
                Symbol = "ELF",
                Price = priceNew
            });
            
            var symbolPrice = await NftSaleStub.GetPrice.CallAsync(new Empty{});

            symbolPrice.Amount.ShouldBe(4);
        }

        [Fact]
        public async Task Purchase_Success()
        {
            var price = new Price
            {
                Amount = 2,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            await NftSaleStub.Initialize.SendAsync(nftPrice);
            await ApproveSpendingAsync(10000_0000000);
            await TokenContractStub.Approve.SendAsync(new ApproveInput
            {
                Spender = Accounts[1].Address,
                Symbol = "ELF",
                Amount = 100_00000000
            });
            await TokenContractStub.Approve.SendAsync(new ApproveInput
            {
                Spender = Accounts[0].Address,
                Symbol = "ELF",
                Amount = 100_00000000
            });
            //await SendTokenTo(Accounts[2].Address);
            await SendTokenTo(ContractAddress);
            //await SendTokenTo(Accounts[0].Address);
            await SendTokenTo(Accounts[1].Address);
            var initialContractBalance = await GetContractBalanceAsync(Accounts[0].Address);
            
            var initialContractBalance1 = await GetContractBalanceAsync(Accounts[1].Address);
            var priceNew = new Price
            {
                Amount = 3,
                Symbol = "ELF"
            };
            await NftSaleStub.Purchase.SendAsync(new PurchaseInput
            {
                Amount = 1,
                Symbol = "ELF",
                Memo = "Test get resource",
                Price = priceNew
            });
            
            var finalContractBalance = await GetContractBalanceAsync(Accounts[0].Address);
            var finalContractBalance2 = await GetContractBalanceAsync(Accounts[1].Address);
            finalContractBalance.ShouldBe(initialContractBalance - 1);
        }
        
        private async Task ApproveSpendingAsync(long amount)
        {
            await TokenContractStub.Approve.SendAsync(new ApproveInput
            {
                Spender = ContractAddress,
                Symbol = "ELF",
                Amount = amount
            });
        }
        
        [Fact]
        public async Task Deposit_Success()
        {
            var price = new Price
            {
                Amount = 2,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Arrange
            await NftSaleStub.Initialize.SendAsync(nftPrice);
            
            // Approve spending on the lottery contract
            await ApproveSpendingAsync(100_00000000);

            const long depositAmount = 10_000_000; // 0.1 ELF
            // var depositInput = new Int64Value() { Value = depositAmount };
            var depositInput = new DepositeInput
            {
                Symbol = "ELF",
                Amount = 10_000_000
            };

            var initialContractBalance = await GetContractBalanceAsync(ContractAddress);
            
            // Act
            var result = await NftSaleStub.Deposit.SendAsync(depositInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check balance update
            var finalContractBalance = await GetContractBalanceAsync(ContractAddress);
            finalContractBalance.ShouldBe(initialContractBalance + depositAmount);

            // Check if the event is emitted
            var events = result.TransactionResult.Logs;
            events.ShouldContain(log => log.Name == nameof(DepositEvent));
        }

        [Fact]
        public async Task Withdraw_Success()
        {
            var price = new Price
            {
                Amount = 2,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Arrange
            await NftSaleStub.Initialize.SendAsync(nftPrice);
            
            // Approve spending on the lottery contract
            await ApproveSpendingAsync(100_00000000);

            var depositInput = new DepositeInput
            {
                Symbol = "ELF",
                Amount = 10_000_000
            };
            await NftSaleStub.Deposit.SendAsync(depositInput);

            const long withdrawAmount = 5_000_000; // 0.05 ELF
            var withdrawInput = new Int64Value() { Value = withdrawAmount };

            var initialSenderBalance = await GetTokenBalanceAsync(DefaultAccount.Address);
            var initialContractBalance = await GetContractBalanceAsync(ContractAddress);
            
            // Act
            var result = await NftSaleStub.Withdraw.SendAsync(withdrawInput);

            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            // Check balance update
            var finalSenderBalance = await GetTokenBalanceAsync(DefaultAccount.Address);
            var finalContractBalance = await GetContractBalanceAsync(ContractAddress);

            finalSenderBalance.ShouldBe(initialSenderBalance + withdrawAmount);
            finalContractBalance.ShouldBe(initialContractBalance - withdrawAmount);

            // Check if the event is emitted
            var events = result.TransactionResult.Logs;
            events.ShouldContain(log => log.Name == nameof(WithdrawEvent));
        }

        [Fact]
        public async Task Withdraw_InsufficientBalance_Fail()
        {
            var price = new Price
            {
                Amount = 2,
                Symbol = "ELF"
            };
            var nftPrice = new NftPrice
            {
                Symbol = "ELF",
                Price = price
            };
            // Arrange
            await NftSaleStub.Initialize.SendAsync(nftPrice);

            long withdrawAmount = 5_000_000; // 0.05 ELF
            var withdrawInput = new Int64Value() { Value = withdrawAmount };

            // Act & Assert
            Should.Throw<Exception>(async () => await NftSaleStub.Withdraw.SendAsync(withdrawInput));
        }

        private async Task<long> GetTokenBalanceAsync(Address owner)
        {
            return (await TokenContractStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = owner,
                Symbol = "ELF"
            })).Balance;
        }

        private async Task<long> GetContractBalanceAsync(Address address)
        {
            var input = new GetContractBalanceInput {Address = address};
            return (await NftSaleStub.GetContractBalance.CallAsync(input)).Value;
        }
        
        private async Task SendTokenTo(Address address)
        {
            await TokenContractStub.Transfer.SendAsync(new TransferInput
            {
                To = address,
                Symbol = "ELF",
                Amount = 100
            });
        }
    }
}