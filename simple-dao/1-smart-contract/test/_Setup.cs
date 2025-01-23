using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AElf.ContractTestBase;
using AElf.ContractTestKit;
using AElf.Kernel.SmartContract;
using AElf.Kernel.SmartContract.Application;
using AElf.Types;
using Google.Protobuf;
using Volo.Abp.Modularity;
using AElf.Kernel;
using Microsoft.Extensions.DependencyInjection;
using AElf.Cryptography.ECDSA;
using AElf.Kernel.Token;
using AElf.Standards.ACS0;
using Volo.Abp.Threading;
using System.IO;
using Google.Protobuf.WellKnownTypes;
using AElf.CSharp.Core;

namespace AElf.Contracts.SimpleDAO
{
    [DependsOn(
    typeof(ContractTestModule))]
    public class Module : ContractTestModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            Configure<ContractOptions>(o => o.ContractDeploymentAuthorityRequired = false);
            
            context.Services.AddSingleton<IBlockTimeProvider, BlockTimeProvider>();
            context.Services.AddSingleton<TokenContractContainer.TokenContractStub>();
            
            // Add Token contract initialization
            context.Services.AddSingleton<IContractInitializationProvider, TokenContractInitializationProvider>();
        }
    }

    public class TestBase : ContractTestBase<Module>
    {
        internal SimpleDAOContainer.SimpleDAOStub SimpleDAOStub { get; private set; }
        internal TokenContractContainer.TokenContractStub TokenContractStub { get; private set; }
        internal IBlockTimeProvider BlockTimeProvider { get; private set; }
        
        protected ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        protected Address ContractAddress { get; private set; }
        protected Address TokenContractAddress { get; private set; }
        protected Address SimpleDAOAddress => ContractAddress;

        public TestBase()
        {
            BlockTimeProvider = GetRequiredService<IBlockTimeProvider>();
            AsyncHelper.RunSync(InitializeContracts);
        }

        private async Task InitializeContracts()
        {
            // Deploy Token Contract as system contract
            TokenContractAddress = await DeploySystemSmartContract(
                KernelConstants.CodeCoverageRunnerCategory,
                File.ReadAllBytes(typeof(TokenContract).Assembly.Location),
                HashHelper.ComputeFrom("AElf.ContractNames.Token"),
                DefaultKeyPair
            );

            // Deploy SimpleDAO Contract
            ContractAddress = await DeployContractAsync(
                KernelConstants.CodeCoverageRunnerCategory,
                File.ReadAllBytes(typeof(SimpleDAO).Assembly.Location),
                HashHelper.ComputeFrom("SimpleDAO"),
                DefaultKeyPair
            );

            // Initialize stubs
            SimpleDAOStub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, DefaultKeyPair);
            TokenContractStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);

            // Create the seed NFT collection first
            await TokenContractStub.Create.SendAsync(new CreateInput
            {
                Symbol = "SEED-0",
                TokenName = "Seed Collection",
                TotalSupply = 1,
                Decimals = 0,
                Issuer = Accounts[0].Address,
                IsBurnable = false,
                Owner = Accounts[0].Address,
                IssueChainId = 9992731,
                ExternalInfo = new ExternalInfo
                {
                    Value = { { "TokenType", "NFT" } }
                }
            });

            // Issue the seed NFT to the default account
            await TokenContractStub.Issue.SendAsync(new IssueInput
            {
                Symbol = "SEED-0",
                Amount = 1,
                To = Accounts[0].Address,
                Memo = "Issue seed NFT"
            });

            // Create the native token (ELF)
            await TokenContractStub.Create.SendAsync(new CreateInput
            {
                Symbol = "ELF",
                TokenName = "ELF Token",
                TotalSupply = 100_000_000_000_000_000L,
                Decimals = 8,
                Issuer = Accounts[0].Address,
                IsBurnable = true,
                Owner = Accounts[0].Address,
                IssueChainId = 9992731,
                ExternalInfo = new ExternalInfo
                {
                    Value = { { "TokenType", "FT" } }
                }
            });

            // Issue the native token to the default account
            await TokenContractStub.Issue.SendAsync(new IssueInput
            {
                Symbol = "ELF",
                Amount = 100_000_000_000_000_000L,
                To = Accounts[0].Address,
                Memo = "Issue native token"
            });
        }
    }
}