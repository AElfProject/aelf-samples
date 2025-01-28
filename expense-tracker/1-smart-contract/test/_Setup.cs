using AElf.Cryptography.ECDSA;
using AElf.ContractTestBase;
using AElf.ContractTestKit;
using AElf.Kernel.SmartContract.Application;
using AElf.Types;
using Volo.Abp.Modularity;
using AElf.Kernel.SmartContract;
using System.Threading.Tasks;
using Volo.Abp.Threading;
using System.IO;
using AElf.Kernel;

namespace AElf.Contracts.ExpenseTracker
{
    [DependsOn(typeof(ContractTestModule))]
    public class Module : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
        }
    }
    
    public class TestBase : ContractTestBase<Module>
    {
        internal ExpenseTrackerContainer.ExpenseTrackerStub ExpenseTrackerStub { get; private set; }
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; set; }

        public TestBase()
        {
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy expense tracker contract
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(ExpenseTracker).Assembly.Location),
                HashHelper.ComputeFrom("ExpenseTracker"),
                DefaultKeyPair
            );
            
            // Initialize contract stub
            ExpenseTrackerStub = GetExpenseTrackerContractStub(DefaultKeyPair);
        }

        private ExpenseTrackerContainer.ExpenseTrackerStub GetExpenseTrackerContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<ExpenseTrackerContainer.ExpenseTrackerStub>(ContractAddress, senderKeyPair);
        }
    }
}