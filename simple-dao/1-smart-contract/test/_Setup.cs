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
using Microsoft.Extensions.DependencyInjection;
using AElf.Contracts.MultiToken;

namespace AElf.Contracts.SimpleDAO
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
        internal Address ContractAddress { get; set; }
        internal SimpleDAOContainer.SimpleDAOStub SimpleDAOStub { get; set; }
        internal TokenContractContainer.TokenContractStub TokenContractStub { get; set; }
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address TokenContractAddress { get; set; }
        protected IBlockTimeProvider BlockTimeProvider { get; set; }

        public TestBase()
        {
            BlockTimeProvider = GetRequiredService<IBlockTimeProvider>();
            AsyncHelper.RunSync(InitializeContracts);
        }

        protected async Task InitializeContracts()
        {
            // Deploy SimpleDAO contract
            ContractAddress = await DeployContractAsync(
                KernelConstants.DefaultRunnerCategory,
                File.ReadAllBytes(typeof(SimpleDAO).Assembly.Location),
                HashHelper.ComputeFrom("SimpleDAO"),
                DefaultKeyPair
            );
            
            // Get Token contract address
            TokenContractAddress = Address.FromBase58("ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx");
            
            // Initialize contract stubs
            SimpleDAOStub = GetSimpleDAOContractStub(DefaultKeyPair);
            TokenContractStub = GetTokenContractStub(DefaultKeyPair);
        }

        private SimpleDAOContainer.SimpleDAOStub GetSimpleDAOContractStub(ECKeyPair keyPair)
        {
            return GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, keyPair);
        }

        private TokenContractContainer.TokenContractStub GetTokenContractStub(ECKeyPair keyPair)
        {
            return GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, keyPair);
        }
    }
}