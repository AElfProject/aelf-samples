using AElf.Cryptography.ECDSA;
using AElf.ContractTestBase;
using AElf.ContractTestKit;
using AElf.Types;
using Volo.Abp.Modularity;
using AElf.Kernel.SmartContract;
using System.Threading.Tasks;
using Volo.Abp.Threading;
using System.IO;
using AElf.Kernel;
using System.Collections.Generic;
using AElf.Kernel.SmartContract.Application;
using AElf.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Google.Protobuf.WellKnownTypes;
using System;

namespace AElf.Contracts.DonationApp
{
    [DependsOn(typeof(ContractTestModule))]
    public class DonationDAppTestModule : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
            context.Services.AddSingleton<IContractCodeProvider, ContractCodeProvider>();
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            var contractCodeProvider = context.ServiceProvider.GetRequiredService<IContractCodeProvider>();
            var contractDllLocation = typeof(DonationApp).Assembly.Location;
            var codes = new Dictionary<string, byte[]>
            {
                {
                    nameof(DonationApp),
                    File.ReadAllBytes(contractDllLocation)
                }
            };
            ((ContractCodeProvider)contractCodeProvider).Codes = codes;
        }
    }

    public class ContractCodeProvider : IContractCodeProvider
    {
        private IReadOnlyDictionary<string, byte[]> _codes;

        public IReadOnlyDictionary<string, byte[]> Codes
        {
            get => _codes;
            set => _codes = value;
        }
    }

    public class DonationDAppTestBase : ContractTestBase<DonationDAppTestModule>
    {
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address DefaultAddress => Accounts[0].Address;
        protected ECKeyPair User1KeyPair => Accounts[1].KeyPair;
        protected Address User1Address => Accounts[1].Address;
        protected ECKeyPair User2KeyPair => Accounts[2].KeyPair;
        protected Address User2Address => Accounts[2].Address;

        internal DonationDAppContainer.DonationDAppStub DonationContract { get; private set; }
        protected Address DonationContractAddress { get; set; }

        public DonationDAppTestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy the donation contract
            DonationContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(DonationApp).Assembly.Location),
                HashHelper.ComputeFrom("DonationApp"),
                DefaultKeyPair
            );
            DonationContract = GetTester<DonationDAppContainer.DonationDAppStub>(DonationContractAddress, DefaultKeyPair);
        }

        protected Timestamp GetTimestamp(long offsetDays = 0)
        {
            return new Timestamp
            {
                Seconds = DateTimeOffset.UtcNow.AddDays(offsetDays).ToUnixTimeSeconds()
            };
        }
    }
} 