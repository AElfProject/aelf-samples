using AElf.Contracts.MultiToken;
using AElf.Cryptography.ECDSA;
using AElf.ContractTestBase;
using AElf.ContractTestKit;
using AElf.Kernel.SmartContract.Application;
using AElf.Types;
using Volo.Abp.Modularity;
using AElf.Kernel.SmartContract;
using System.Threading.Tasks;
using Volo.Abp.Threading;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using AElf.Kernel;

namespace AElf.Contracts.LotteryGame
{
    // The Module class load the context required for unit testing
    [DependsOn(typeof(ContractTestModule))]
    public class Module : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            // disable authority for deployment
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
        }
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class TestBase : ContractTestBase<Module>
    {
        // The Stub class for unit testing
        internal LotteryGameContainer.LotteryGameStub LotteryGameStub { get; private set; }
        internal TokenContractContainer.TokenContractStub TokenContractStub { get; private set; }
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; set; }

        public TestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy the contract using the base class method
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(LotteryGame).Assembly.Location),
                HashHelper.ComputeFrom("LotteryGame"),
                DefaultKeyPair
            );
            
            LotteryGameStub = GetLotteryGameContractStub(DefaultKeyPair);
            // TokenContractStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);
        }

        private LotteryGameContainer.LotteryGameStub GetLotteryGameContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<LotteryGameContainer.LotteryGameStub>(ContractAddress, senderKeyPair);
        }
    }
}