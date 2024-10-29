using AElf.Contracts.MultiToken;
using AElf.ContractTestBase.ContractTestKit;
using AElf.Cryptography.ECDSA;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace AElf.Contracts.SimpleDAO
{
    // The Module class load the context required for unit testing
    public class Module : Testing.TestBase.ContractTestModule<SimpleDAO>
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            context.Services.AddSingleton<IBlockTimeProvider, BlockTimeProvider>();
        }
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class TestBase : Testing.TestBase.ContractTestBase<Module>
    {
        internal IBlockTimeProvider BlockTimeProvider;
        
        // The Stub class for unit testing
        internal readonly SimpleDAOContainer.SimpleDAOStub SimpleDAOStub;
        internal readonly TokenContractContainer.TokenContractStub TokenContractStub;
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;

        public TestBase()
        {
            SimpleDAOStub = GetSimpleDAOContractStub(DefaultKeyPair);
            TokenContractStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);
            BlockTimeProvider = Application.ServiceProvider.GetService<IBlockTimeProvider>();
        }

        private SimpleDAOContainer.SimpleDAOStub GetSimpleDAOContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, senderKeyPair);
        }
    }
    
}