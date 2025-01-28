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
using System;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.BuildersDAO
{
    [DependsOn(typeof(ContractTestModule))]
    public class BuildersDAOTestModule : ContractTestModule
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
            var contractDllLocation = typeof(BuildersDAO).Assembly.Location;
            var codes = new Dictionary<string, byte[]>
            {
                {
                    nameof(BuildersDAO),
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

    public class BuildersDAOTestBase : ContractTestBase<BuildersDAOTestModule>
    {
        internal BuildersDAOContainer.BuildersDAOStub BuildersDAOStub { get; private set; }
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; set; }
        private static int _contractIndex;

        public BuildersDAOTestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            var uniqueId = System.Threading.Interlocked.Increment(ref _contractIndex);
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(BuildersDAO).Assembly.Location),
                HashHelper.ComputeFrom($"BuildersDAO{uniqueId}"),
                DefaultKeyPair
            );
            
            BuildersDAOStub = GetBuildersDAOContractStub(DefaultKeyPair);
            await BuildersDAOStub.Initialize.SendAsync(new Empty());
        }

        internal BuildersDAOContainer.BuildersDAOStub GetBuildersDAOContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<BuildersDAOContainer.BuildersDAOStub>(ContractAddress, senderKeyPair);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}