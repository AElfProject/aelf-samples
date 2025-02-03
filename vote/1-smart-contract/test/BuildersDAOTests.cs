using System;
using System.Threading.Tasks;
using AElf.Types;
using AElf.ContractTestKit;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;
using AElf.Cryptography.ECDSA;
using AElf.ContractTestBase.ContractTestKit;

namespace AElf.Contracts.BuildersDAO
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class BuildersDAOTests : BuildersDAOTestBase
    {
        [Fact]
        public async Task Initialize_Success()
        {
            // Act
            var memberCount = await BuildersDAOStub.GetMemberCount.CallAsync(new Empty());
            var proposals = await BuildersDAOStub.GetAllProposals.CallAsync(new Empty());

            // Assert
            memberCount.Value.ShouldBe(0);
            proposals.Proposals.Count.ShouldBe(1);
            proposals.Proposals[0].Title.ShouldBe("Proposal #1");
            proposals.Proposals[0].Description.ShouldBe("This is the first proposal of the DAO");
            proposals.Proposals[0].Status.ShouldBe("IN PROGRESS");
        }

        [Fact]
        public async Task JoinDAO_Success()
        {
            // Arrange
            var address = Address.FromPublicKey(DefaultKeyPair.PublicKey);

            // Act
            await BuildersDAOStub.JoinDAO.SendAsync(address);
            var memberCount = await BuildersDAOStub.GetMemberCount.CallAsync(new Empty());
            var isMember = await BuildersDAOStub.GetMemberExist.CallAsync(address);

            // Assert
            memberCount.Value.ShouldBe(1);
            isMember.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task CreateProposal_Success()
        {
            // Arrange
            var address = Address.FromPublicKey(DefaultKeyPair.PublicKey);
            await BuildersDAOStub.JoinDAO.SendAsync(address);

            var proposalInput = new CreateProposalInput
            {
                Creator = address,
                Title = "Test Proposal",
                Description = "This is a test proposal",
                VoteThreshold = 3
            };

            // Act
            var proposal = await BuildersDAOStub.CreateProposal.SendAsync(proposalInput);
            var createdProposal = await BuildersDAOStub.GetProposal.CallAsync(new StringValue { Value = proposal.Output.Id });

            // Assert
            createdProposal.Title.ShouldBe(proposalInput.Title);
            createdProposal.Description.ShouldBe(proposalInput.Description);
            createdProposal.VoteThreshold.ShouldBe(proposalInput.VoteThreshold);
            createdProposal.Status.ShouldBe("IN PROGRESS");
        }

        [Fact]
        public async Task VoteOnProposal_Success()
        {
            // Arrange
            var address = Address.FromPublicKey(DefaultKeyPair.PublicKey);
            await BuildersDAOStub.JoinDAO.SendAsync(address);

            var proposalInput = new CreateProposalInput
            {
                Creator = address,
                Title = "Test Proposal",
                Description = "This is a test proposal",
                VoteThreshold = 1
            };
            var proposal = await BuildersDAOStub.CreateProposal.SendAsync(proposalInput);

            // Act
            var voteInput = new VoteInput
            {
                Voter = address,
                ProposalId = proposal.Output.Id,
                Vote = true
            };
            await BuildersDAOStub.VoteOnProposal.SendAsync(voteInput);
            var votedProposal = await BuildersDAOStub.GetProposal.CallAsync(new StringValue { Value = proposal.Output.Id });

            // Assert
            votedProposal.YesVotes.Count.ShouldBe(1);
            votedProposal.NoVotes.Count.ShouldBe(0);
            votedProposal.Status.ShouldBe("PASSED");
        }

        [Fact]
        public async Task GetAllProposals_Success()
        {
            // Arrange
            var address = Address.FromPublicKey(DefaultKeyPair.PublicKey);
            await BuildersDAOStub.JoinDAO.SendAsync(address);

            var proposalInput1 = new CreateProposalInput
            {
                Creator = address,
                Title = "Test Proposal 1",
                Description = "This is test proposal 1",
                VoteThreshold = 3
            };
            var proposalInput2 = new CreateProposalInput
            {
                Creator = address,
                Title = "Test Proposal 2",
                Description = "This is test proposal 2",
                VoteThreshold = 3
            };
            await BuildersDAOStub.CreateProposal.SendAsync(proposalInput1);
            await BuildersDAOStub.CreateProposal.SendAsync(proposalInput2);

            // Act
            var proposals = await BuildersDAOStub.GetAllProposals.CallAsync(new Empty());

            // Assert
            proposals.Proposals.Count.ShouldBe(3); // Including the initial proposal
            proposals.Proposals[0].Title.ShouldBe("Proposal #1"); // Initial proposal
            proposals.Proposals[1].Title.ShouldBe(proposalInput1.Title);
            proposals.Proposals[2].Title.ShouldBe(proposalInput2.Title);
        }
    }
}