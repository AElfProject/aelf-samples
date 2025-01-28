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

namespace AElf.Contracts.TicTacToe
{
    [DependsOn(typeof(ContractTestModule))]
    public class TicTacToeTestModule : ContractTestModule
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
            var contractDllLocation = typeof(TicTacToe).Assembly.Location;
            var codes = new Dictionary<string, byte[]>
            {
                {
                    nameof(TicTacToe),
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

    public class TicTacToeTestBase : ContractTestBase<TicTacToeTestModule>
    {
        internal TicTacToeContainer.TicTacToeStub TicTacToeStub { get; private set; }
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; set; }

        public TicTacToeTestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(TicTacToe).Assembly.Location),
                HashHelper.ComputeFrom("TicTacToe"),
                DefaultKeyPair
            );
            
            TicTacToeStub = GetTicTacToeContractStub(DefaultKeyPair);
        }

        internal TicTacToeContainer.TicTacToeStub GetTicTacToeContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<TicTacToeContainer.TicTacToeStub>(ContractAddress, senderKeyPair);
        }
    }
}