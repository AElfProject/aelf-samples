using AElf.Contracts.MultiToken;
using AElf.Cryptography.ECDSA;
using AElf.Testing.TestBase;

namespace AElf.Contracts.NftSale
{
    // The Module class load the context required for unit testing
    public class Module : ContractTestModule<NftSale>
    {
        
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class TestBase : ContractTestBase<Module>
    {
        // The Stub class for unit testing
        internal readonly NftSaleContainer.NftSaleStub NftSaleStub;
        internal readonly TokenContractContainer.TokenContractStub TokenContractStub;
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;
        
        public TestBase()
        {
            NftSaleStub = GetNftSaleContractStub(DefaultKeyPair);
            TokenContractStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);
        }

        private NftSaleContainer.NftSaleStub GetNftSaleContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<NftSaleContainer.NftSaleStub>(ContractAddress, senderKeyPair);
        }
    }
    
}