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
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; set; }

        public TestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy lottery game contract
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(LotteryGame).Assembly.Location),
                HashHelper.ComputeFrom("LotteryGame"),
                DefaultKeyPair
            );
            
            // Initialize contract stub
            LotteryGameStub = GetLotteryGameContractStub(DefaultKeyPair);
        }

        private LotteryGameContainer.LotteryGameStub GetLotteryGameContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<LotteryGameContainer.LotteryGameStub>(ContractAddress, senderKeyPair);
        }
    }
}