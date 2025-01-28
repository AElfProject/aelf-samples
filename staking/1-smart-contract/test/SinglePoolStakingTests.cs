using System.Threading.Tasks;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.SinglePoolStaking
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class SinglePoolStakingTests : SinglePoolStakingTestBase
    {
        [Fact]
        public async Task Initialize_Test()
        {
            // Arrange
            var tokenContractAddress = Address.FromPublicKey(DefaultKeyPair.PublicKey);
            var input = new InitializeInput
            {
                TokenContractAddress = tokenContractAddress
            };

            // Act
            await SinglePoolStakingStub.Initialize.SendAsync(input);
            
            // Assert
            var initialized = await SinglePoolStakingStub.IfInitialized.CallAsync(new Empty());
            initialized.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task GetTotalStakedAmount_ShouldBeZero_WhenNoDeposits()
        {
            // Arrange
            var tokenContractAddress = Address.FromPublicKey(DefaultKeyPair.PublicKey);
            await SinglePoolStakingStub.Initialize.SendAsync(new InitializeInput
            {
                TokenContractAddress = tokenContractAddress
            });

            // Act
            var totalStaked = await SinglePoolStakingStub.GetTotalStakedAmount.CallAsync(new Empty());
            
            // Assert
            totalStaked.Value.ShouldBe(0);
        }
    }
    
}