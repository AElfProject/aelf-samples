using AElf.Contracts.MultiToken;
using AElf.ContractTestBase;
using AElf.ContractTestKit;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AElf.Contracts.DonationApp
{
    public class DonationDAppTestBase : ContractTestBase<DonationDAppTestModule>
    {
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address DefaultAddress => Accounts[0].Address;
        protected ECKeyPair User1KeyPair => Accounts[1].KeyPair;
        protected Address User1Address => Accounts[1].Address;
        protected ECKeyPair User2KeyPair => Accounts[2].KeyPair;
        protected Address User2Address => Accounts[2].Address;

        internal readonly DonationDAppContainer.DonationDAppStub DonationContract;
        internal readonly TokenContractContainer.TokenContractStub TokenContract;

        protected const string TokenSymbol = "ELF";
        protected const long InitialBalance = 100_00000000; // 100 ELF
        protected const long DefaultDonationAmount = 10_00000000; // 10 ELF

        protected Address TokenContractAddress { get; set; }
        protected Address DonationContractAddress { get; set; }

        public DonationDAppTestBase()
        {
            // Mock contract addresses
            TokenContractAddress = SampleAddress.AddressList[0];
            DonationContractAddress = SampleAddress.AddressList[1];

            // Initialize contract stubs
            DonationContract = GetDonationContractStub(DefaultKeyPair);
            TokenContract = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);
        }

        private DonationDAppContainer.DonationDAppStub GetDonationContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<DonationDAppContainer.DonationDAppStub>(DonationContractAddress, senderKeyPair);
        }

        protected async Task ApproveTokenAsync(Address spender, long amount)
        {
            await TokenContract.Approve.SendAsync(new ApproveInput
            {
                Spender = spender,
                Symbol = TokenSymbol,
                Amount = amount
            });
        }

        protected async Task<long> GetTokenBalanceAsync(Address owner)
        {
            var balance = await TokenContract.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = owner,
                Symbol = TokenSymbol
            });
            return balance.Amount;
        }

        protected Timestamp GetTimestamp(long offsetDays = 0)
        {
            return new Timestamp
            {
                Seconds = DateTimeOffset.UtcNow.AddDays(offsetDays).ToUnixTimeSeconds()
            };
        }
    }

    public static class SampleAddress
    {
        public static readonly List<Address> AddressList = new List<Address>
        {
            Address.FromBase58("21tWvZwYtkd4nW2PG5VktZ5x9UMi69EqR89QUjhcpEDYj"),  // Token Contract
            Address.FromBase58("2UhWBW5nRYHh8qR5YuR3gQYvXS3bM5zWPJM4vQkYiGBmf"),  // Donation Contract
        };
    }
} 