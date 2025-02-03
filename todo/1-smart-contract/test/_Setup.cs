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

namespace AElf.Contracts.ToDo.Tests
{
    // The Module class load the context required for unit testing
    [DependsOn(typeof(ContractTestModule))]
    public class ToDoContractTestModule : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            // disable authority for deployment
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
        }
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class ToDoContractTestBase : ContractTestBase<ToDoContractTestModule>
    {
        // The Stub class for unit testing
        internal ToDoContainer.ToDoStub ToDoContractStub { get; set; }
        // A key pair that can be used to interact with the contract instance
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address DefaultSender => Address.FromPublicKey(DefaultKeyPair.PublicKey);
        protected Address ContractAddress { get; set; }

        public ToDoContractTestBase()
        {
            InitializeContracts();
        }

        private void InitializeContracts()
        {
            // Deploy the contract using the base class method
            ContractAddress = DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(ToDo).Assembly.Location),
                HashHelper.ComputeFrom("ToDo"),
                DefaultKeyPair
            ).Result;
            
            ToDoContractStub = GetToDoContractStub(DefaultKeyPair);
        }

        private ToDoContainer.ToDoStub GetToDoContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<ToDoContainer.ToDoStub>(ContractAddress, senderKeyPair);
        }
    }
}