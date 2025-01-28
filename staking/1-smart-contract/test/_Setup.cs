using AElf.Cryptography.ECDSA;
using AElf.ContractTestBase;
using AElf.Kernel.SmartContract.Application;
using AElf.Types;
using AElf.ContractTestKit;
using Volo.Abp.Modularity;
using AElf.Kernel.SmartContract;
using System.Threading.Tasks;
using Volo.Abp.Threading;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using System.IO;
using AElf.Kernel;

namespace AElf.Contracts.SinglePoolStaking
{
    // The Module class load the context required for unit testing
    [DependsOn(typeof(ContractTestModule))]
    public class SinglePoolStakingTestModule : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            // disable authority for deployment
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
        }
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class SinglePoolStakingTestBase : ContractTestBase<SinglePoolStakingTestModule>
    {
        // The Stub class for unit testing
        internal SinglePoolStakingContainer.SinglePoolStakingStub SinglePoolStakingStub { get; private set; }
        // A key pair that can be used to interact with the contract instance
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address DefaultSender => Address.FromPublicKey(DefaultKeyPair.PublicKey);
        protected Address ContractAddress { get; set; }

        public SinglePoolStakingTestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy the contract using the base class method
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(SinglePoolStaking).Assembly.Location),
                HashHelper.ComputeFrom("SinglePoolStaking"),
                DefaultKeyPair
            );
            
            SinglePoolStakingStub = GetSinglePoolStakingContractStub(DefaultKeyPair);
        }

        private SinglePoolStakingContainer.SinglePoolStakingStub GetSinglePoolStakingContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<SinglePoolStakingContainer.SinglePoolStakingStub>(ContractAddress, senderKeyPair);
        }
    }
}