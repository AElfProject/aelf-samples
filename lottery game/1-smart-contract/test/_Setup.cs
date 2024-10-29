using AElf.Contracts.MultiToken;
using AElf.Cryptography.ECDSA;
using AElf.Testing.TestBase;

namespace AElf.Contracts.LotteryGame
{
    // The Module class load the context required for unit testing
    public class Module : ContractTestModule<LotteryGame>
    {
        
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class TestBase : ContractTestBase<Module>
    {
        // The Stub class for unit testing
        internal readonly LotteryGameContainer.LotteryGameStub LotteryGameStub;
        internal readonly TokenContractContainer.TokenContractStub TokenContractStub;
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;

        public TestBase()
        {
            LotteryGameStub = GetLotteryGameContractStub(DefaultKeyPair);
            TokenContractStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, DefaultKeyPair);
        }

        private LotteryGameContainer.LotteryGameStub GetLotteryGameContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<LotteryGameContainer.LotteryGameStub>(ContractAddress, senderKeyPair);
        }
    }
    
}