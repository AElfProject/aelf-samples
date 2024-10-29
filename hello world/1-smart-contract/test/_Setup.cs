using AElf.Cryptography.ECDSA;
using AElf.Testing.TestBase;

namespace AElf.Contracts.HelloWorld
{
    // The Module class load the context required for unit testing
    public class Module : ContractTestModule<HelloWorld>
    {
        
    }
    
    // The TestBase class inherit ContractTestBase class, it defines Stub classes and gets instances required for unit testing
    public class TestBase : ContractTestBase<Module>
    {
        // The Stub class for unit testing
        internal readonly HelloWorldContainer.HelloWorldStub HelloWorldStub;
        // A key pair that can be used to interact with the contract instance
        private ECKeyPair DefaultKeyPair => Accounts[0].KeyPair;

        public TestBase()
        {
            HelloWorldStub = GetHelloWorldContractStub(DefaultKeyPair);
        }

        private HelloWorldContainer.HelloWorldStub GetHelloWorldContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<HelloWorldContainer.HelloWorldStub>(ContractAddress, senderKeyPair);
        }
    }
    
}