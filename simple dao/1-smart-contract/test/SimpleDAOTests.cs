using System;
using System.Linq;
using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.SimpleDAO
{
    // This class is unit test class, and it inherit TestBase. Write your unit test code inside it
    public class SimpleDAOTests : TestBase
    {
        private const string TokenSymbol = "ELF";
        private const long BallotAmount = 5;
        private const int DefaultProposalEndTimeOffset = 100;
        
        [Fact]
        public async Task InitializeContract_Success()
        {
            // Act
            var result = await InitializeSimpleDaoContract();
            
            // Assert
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var symbol = await SimpleDAOStub.GetTokenSymbol.CallAsync(new Empty());
            symbol.Value.ShouldBe(TokenSymbol);
        }
        
        [Fact]
        public Task InitializeContract_Fail_TokenSymbolDoesNotExist()
        {
            // Act
            var invalidInput = new InitializeInput
            {
                TokenSymbol = "MOCK_TOKEN_SYMBOL"
            };
            
            // Act & Assert
            Should.Throw<Exception>(async () => await SimpleDAOStub.Initialize.SendAsync(invalidInput));
            return Task.CompletedTask;
        }

        [Fact]
        public async Task InitializeContract_Fail_AlreadyInitialized()
        {
            // Arrange
            await InitializeSimpleDaoContract();

            // Act & Assert
            Should.Throw<Exception>(async () => await SimpleDAOStub.Initialize.SendAsync(new InitializeInput
            {
                TokenSymbol = TokenSymbol
            }));
        }
        
        [Fact]
        public async Task CreateProposal_Success()
        {
            await InitializeSimpleDaoContract();
            
            var input = new CreateProposalInput
            {
                Title = "Test Proposal",
                Description = "This is a test proposal.",
                EndTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(DefaultProposalEndTimeOffset)),
                StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var result = await SimpleDAOStub.CreateProposal.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            const string proposalId = "1";
            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue{ Value = proposalId });
            proposal.ShouldNotBeNull();
            proposal.Title.ShouldBe(input.Title);
            proposal.Description.ShouldBe(input.Description);
        }

        [Fact]
        public async Task CreateProposal_EmptyTitle_ShouldThrow()
        {
            await InitializeSimpleDaoContract();
            
            var input = new CreateProposalInput
            {
                Title = "",
                Description = "This is a test proposal.",
                EndTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(DefaultProposalEndTimeOffset)),
                StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.CreateProposal.SendAsync(input));
            exception.Message.ShouldContain("Title should not be empty.");
        }

        [Fact]
        public async Task CreateProposal_EmptyDescription_ShouldThrow()
        {
            await InitializeSimpleDaoContract();
            
            var input = new CreateProposalInput
            {
                Title = "Mock Proposal",
                Description = "",
                EndTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(DefaultProposalEndTimeOffset)),
                StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.CreateProposal.SendAsync(input));
            exception.Message.ShouldContain("Description should not be empty.");
        }
        
        [Fact]
        public async Task CreateProposal_InvalidStartTime_ShouldThrow()
        {
            await InitializeSimpleDaoContract();
            
            var input = new CreateProposalInput
            {
                Title = "Mock Proposal",
                Description = "This is a test proposal.",
                EndTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(DefaultProposalEndTimeOffset)),
                StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(-1))
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.CreateProposal.SendAsync(input));
            exception.Message.ShouldContain("Start time should be greater or equal to current block time.");
        }
        
        [Fact]
        public async Task CreateProposal_InvalidExpireTime_ShouldThrow()
        {
            await InitializeSimpleDaoContract();
            
            var input = new CreateProposalInput
            {
                Title = "Mock Proposal",
                Description = "This is a test proposal.",
                EndTimestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(-100)),
                StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.CreateProposal.SendAsync(input));
            exception.Message.ShouldContain("Expire time should be greater than current block time.");
        }
        
        [Fact]
        public async Task Vote_Success_Approve()
        {
            await InitializeAndApproveSimpleDaoContract();

            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };

            var result = await SimpleDAOStub.Vote.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            proposal.Result.ApproveCounts.ShouldBe(BallotAmount);
            proposal.Result.RejectCounts.ShouldBe(0);
            proposal.Result.AbstainCounts.ShouldBe(0);
        }

        [Fact]
        public async Task Vote_Success_Reject()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Rejected,
                Amount = BallotAmount
            };

            var result = await SimpleDAOStub.Vote.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            proposal.Result.ApproveCounts.ShouldBe(0);
            proposal.Result.RejectCounts.ShouldBe(BallotAmount);
            proposal.Result.AbstainCounts.ShouldBe(0);
        }
        
        [Fact]
        public async Task Vote_Success_Abstain()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Abstained,
                Amount = BallotAmount
            };

            var result = await SimpleDAOStub.Vote.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            proposal.Result.ApproveCounts.ShouldBe(0);
            proposal.Result.RejectCounts.ShouldBe(0);
            proposal.Result.AbstainCounts.ShouldBe(BallotAmount);
        }

        [Fact]
        public async Task Vote_ProposalNotFound_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var input = new VoteInput
            {
                ProposalId = "1",
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Vote.SendAsync(input));
            exception.Message.ShouldContain("Proposal not found.");
        }

        [Fact]
        public async Task Vote_ProposalNotStarted_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();

            var now = BlockTimeProvider.GetBlockTime();
            var proposalId = await CreateTestProposalAsync(now.AddSeconds(DefaultProposalEndTimeOffset), now.AddSeconds(200));
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            
            BlockTimeProvider.SetBlockTime(3600 * 24 * 8 * 1000);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Vote.SendAsync(input));
            exception.Message.ShouldContain($"Proposal {proposalId} has ended. Voting is not allowed.");
        }
        
        [Fact]
        public async Task Vote_ExpiredProposal_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            
            BlockTimeProvider.SetBlockTime(3600 * 24 * 8 * 1000);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Vote.SendAsync(input));
            exception.Message.ShouldContain($"Proposal {proposalId} has ended. Voting is not allowed.");
        }

        [Fact]
        public async Task Vote_AlreadyVoted_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };

            await SimpleDAOStub.Vote.SendAsync(input);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Vote.SendAsync(input));
            exception.Message.ShouldContain("You have already voted.");
        }

        [Fact]
        public async Task Vote_MultipleUsers_ShouldSucceed()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };

            var user1Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[1].KeyPair);
            var user1TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[1].KeyPair);
            await TokenContractApprove(user1TokenStub);
            await SendTokenTo(Accounts[1].Address);
            
            var user2Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[2].KeyPair);
            var user2TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[2].KeyPair);
            await TokenContractApprove(user2TokenStub);
            await SendTokenTo(Accounts[2].Address);
            
            await user1Stub.Vote.SendAsync(input);
            await user2Stub.Vote.SendAsync(input);

            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            proposal.Result.ApproveCounts.ShouldBe(BallotAmount * 2);
            proposal.Result.RejectCounts.ShouldBe(0);
            proposal.Result.AbstainCounts.ShouldBe(0);
        }

        [Fact]
        public async Task Vote_MultipleVotes_ShouldAccumulate()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var inputAgree = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            var inputDisagree = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Rejected,
                Amount = BallotAmount
            };

            var user1Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[1].KeyPair);
            var user1TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[1].KeyPair);
            await TokenContractApprove(user1TokenStub);
            await SendTokenTo(Accounts[1].Address);
            
            var user2Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[2].KeyPair);
            var user2TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[2].KeyPair);
            await TokenContractApprove(user2TokenStub);
            await SendTokenTo(Accounts[2].Address);
            
            await user1Stub.Vote.SendAsync(inputAgree);
            await user2Stub.Vote.SendAsync(inputDisagree);

            var proposal = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            proposal.Result.ApproveCounts.ShouldBe(BallotAmount);
            proposal.Result.RejectCounts.ShouldBe(BallotAmount);
            proposal.Result.AbstainCounts.ShouldBe(0);
        }
        
        [Fact]
        public async Task Withdraw_Success()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var voteInput = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await SimpleDAOStub.Vote.SendAsync(voteInput);
            
            var initialBalance = TokenContractStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Accounts[0].Address,
                Symbol = TokenSymbol
            }).Result.Balance;
            
            // fast forward to proposal end time
            BlockTimeProvider.SetBlockTime(DefaultProposalEndTimeOffset * 1000);
            
            var withdrawInput = new WithdrawInput
            {
                ProposalId = proposalId
            };

            var result = await SimpleDAOStub.Withdraw.SendAsync(withdrawInput);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var newBalance = TokenContractStub.GetBalance.CallAsync(new GetBalanceInput
            {
                Owner = Accounts[0].Address,
                Symbol = TokenSymbol
            }).Result.Balance;
            
            newBalance.ShouldBe(initialBalance + BallotAmount);
        }

        [Fact]
        public async Task Withdraw_ProposalNotFound_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var input = new WithdrawInput
            {
                ProposalId = "1"
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Withdraw.SendAsync(input));
            exception.Message.ShouldContain("Proposal not found.");
        }

        [Fact]
        public async Task Withdraw_NotVoter_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new WithdrawInput
            {
                ProposalId = proposalId
            };
            
            // fast forward to proposal end time
            BlockTimeProvider.SetBlockTime(DefaultProposalEndTimeOffset * 1000);

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Withdraw.SendAsync(input));
            exception.Message.ShouldContain("Invalid amount.");
        }

        [Fact]
        public async Task Withdraw_BeforeExpiry_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new WithdrawInput
            {
                ProposalId = proposalId
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.Withdraw.SendAsync(input));
            exception.Message.ShouldContain($"Proposal {proposalId} has not ended. Withdrawal is not allowed.");
        }
        
        [Fact]
        public async Task Withdraw_MultipleUsersAttempting_Success()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new WithdrawInput
            {
                ProposalId = proposalId
            };
            var inputAgree = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };

            var user1Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[1].KeyPair);
            var user1TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[1].KeyPair);
            await TokenContractApprove(user1TokenStub);
            await SendTokenTo(Accounts[1].Address);
            
            var user2Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[2].KeyPair);
            var user2TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[2].KeyPair);
            await TokenContractApprove(user2TokenStub);
            await SendTokenTo(Accounts[2].Address);
            
            await user1Stub.Vote.SendAsync(inputAgree);
            await user2Stub.Vote.SendAsync(inputAgree);
            
            // fast forward to proposal end time
            BlockTimeProvider.SetBlockTime(DefaultProposalEndTimeOffset * 1000);

            // Ensure the multiple voters can still withdraw
            var result = await user1Stub.Withdraw.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            
            result = await user2Stub.Withdraw.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
        }
        
        [Fact]
        public async Task GetAllProposals_Empty()
        {
            var result = await SimpleDAOStub.GetAllProposals.CallAsync(new Empty());
            result.Proposals.ShouldNotBeNull();
            result.Proposals.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetAllProposals_OneProposal()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            
            var result = await SimpleDAOStub.GetAllProposals.CallAsync(new Empty());
            result.Proposals.ShouldNotBeNull();
            result.Proposals.Count.ShouldBe(1);

            var proposal = result.Proposals.First();
            proposal.Id.ShouldBe(proposalId);
            proposal.Title.ShouldBe("Test Proposal");
            proposal.Description.ShouldBe("This is a test proposal.");
        }

        [Fact]
        public async Task GetAllProposals_MultipleProposals()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId1 = await CreateTestProposalAsync();
            var proposalId2 = await CreateTestProposalAsync();

            var result = await SimpleDAOStub.GetAllProposals.CallAsync(new Empty());
            result.Proposals.ShouldNotBeNull();
            result.Proposals.Count.ShouldBe(2);
            
            //catch exception if predicate is not met
            Proposal proposal1 = null;
            Proposal proposal2 = null;
            try
            {
                proposal1 = result.Proposals.First(p => p.Id == proposalId1);
                proposal2 = result.Proposals.First(p => p.Id == proposalId2);
            }
            catch (InvalidOperationException)
            {
                Assert.False(true, "Proposal not found.");
            }
            
            proposal1.ShouldNotBeNull();
            proposal2.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAllProposals_ProposalsWithDifferentStates()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId1 = await CreateTestProposalAsync();
            var proposalId2 = await CreateTestProposalAsync();

            // Simulate voting to change state of proposals
            var voteInput = new VoteInput
            {
                ProposalId = proposalId1,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await SimpleDAOStub.Vote.SendAsync(voteInput);

            var result = await SimpleDAOStub.GetAllProposals.CallAsync(new Empty());
            result.Proposals.ShouldNotBeNull();
            result.Proposals.Count.ShouldBe(2);

            var proposal1 = result.Proposals.First(p => p.Id == proposalId1);
            var proposal2 = result.Proposals.First(p => p.Id == proposalId2);

            proposal1.Result.ApproveCounts.ShouldBe(BallotAmount);
            proposal2.Result.ApproveCounts.ShouldBe(0);
        }
        
        [Fact]
        public async Task GetProposal_Success()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            
            var result = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            result.ShouldNotBeNull();
            result.Id.ShouldBe(proposalId);
            result.Title.ShouldBe("Test Proposal");
            result.Description.ShouldBe("This is a test proposal.");
            result.StartTimestamp.ShouldBeLessThanOrEqualTo(BlockTimeProvider.GetBlockTime());
            result.EndTimestamp.ShouldBeGreaterThan(BlockTimeProvider.GetBlockTime());
            result.Proposer.ShouldBe(Accounts[0].Address);
            result.Result.ApproveCounts.ShouldBe(0);
            result.Result.RejectCounts.ShouldBe(0);
            result.Result.AbstainCounts.ShouldBe(0);
        }

        [Fact]
        public async Task GetProposal_ProposalNotFound_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = "1";

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId }));
            exception.Message.ShouldContain("Proposal not found.");
        }

        [Fact]
        public async Task GetProposal_MultipleProposals_Success()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId1 = await CreateTestProposalAsync();
            var proposalId2 = await CreateTestProposalAsync();

            var result1 = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId1 });
            result1.ShouldNotBeNull();
            result1.Id.ShouldBe(proposalId1);

            var result2 = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId2 });
            result2.ShouldNotBeNull();
            result2.Id.ShouldBe(proposalId2);
        }

        [Fact]
        public async Task GetProposal_ExpiredProposal_ShouldSucceed()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();

            // fast forward to proposal end time
            BlockTimeProvider.SetBlockTime((DefaultProposalEndTimeOffset + 1) * 1000);
            
            var result = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            result.ShouldNotBeNull();
            result.EndTimestamp.ShouldBeLessThanOrEqualTo(BlockTimeProvider.GetBlockTime());
        }

        [Fact]
        public async Task GetProposal_ProposalWithVotes_ShouldSucceed()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var voteInput = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await SimpleDAOStub.Vote.SendAsync(voteInput);

            var result = await SimpleDAOStub.GetProposal.CallAsync(new StringValue { Value = proposalId });
            result.ShouldNotBeNull();
            result.Result.ApproveCounts.ShouldBe(BallotAmount);
            result.Result.RejectCounts.ShouldBe(0);
            result.Result.AbstainCounts.ShouldBe(0);
        }
        
        [Fact]
        public async Task GetTokenSymbol_Success()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var result = await SimpleDAOStub.GetTokenSymbol.CallAsync(new Empty());
            result.ShouldNotBeNull();
            result.Value.ShouldBe("ELF");
        }
        
        [Fact]
        public async Task HasVoted_Success_NotVoted()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var input = new HasVotedInput
            {
                ProposalId = proposalId,
                Address = Accounts[0].Address
            };

            var result = await SimpleDAOStub.HasVoted.CallAsync(input);
            result.ShouldNotBeNull();
            result.Value.ShouldBeFalse();
        }

        [Fact]
        public async Task HasVoted_Success_Voted()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var proposalId = await CreateTestProposalAsync();
            var voteInput = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await SimpleDAOStub.Vote.SendAsync(voteInput);

            var input = new HasVotedInput
            {
                ProposalId = proposalId,
                Address = Accounts[0].Address
            };

            var result = await SimpleDAOStub.HasVoted.CallAsync(input);
            result.ShouldNotBeNull();
            result.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task HasVoted_ProposalNotFound_ShouldThrow()
        {
            await InitializeAndApproveSimpleDaoContract();
            
            var input = new HasVotedInput
            {
                ProposalId = "1",
                Address = Accounts[0].Address
            };

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await SimpleDAOStub.HasVoted.CallAsync(input));
            exception.Message.ShouldContain("Proposal not found.");
        }

        [Fact]
        public async Task HasVoted_MultipleVoters_ShouldSucceed()
        {
            await InitializeAndApproveSimpleDaoContract();

            var proposalId = await CreateTestProposalAsync();
            
            var user1Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[1].KeyPair);
            var user1TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[1].KeyPair);
            await TokenContractApprove(user1TokenStub);
            await SendTokenTo(Accounts[1].Address);
            var user2Stub = GetTester<SimpleDAOContainer.SimpleDAOStub>(ContractAddress, Accounts[2].KeyPair);
            var user2TokenStub = GetTester<TokenContractContainer.TokenContractStub>(TokenContractAddress, Accounts[2].KeyPair);
            await TokenContractApprove(user2TokenStub);
            await SendTokenTo(Accounts[2].Address);
            
            var voteInput1 = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await user1Stub.Vote.SendAsync(voteInput1);

            var voteInput2 = new VoteInput
            {
                ProposalId = proposalId,
                Vote = VoteOption.Rejected,
                Amount = BallotAmount
            };
            await user2Stub.Vote.SendAsync(voteInput2);

            var input1 = new HasVotedInput
            {
                ProposalId = proposalId,
                Address = Accounts[1].Address
            };
            var result1 = await SimpleDAOStub.HasVoted.CallAsync(input1);
            result1.ShouldNotBeNull();
            result1.Value.ShouldBeTrue();

            var input2 = new HasVotedInput
            {
                ProposalId = proposalId,
                Address = Accounts[2].Address
            };
            var result2 = await SimpleDAOStub.HasVoted.CallAsync(input2);
            result2.ShouldNotBeNull();
            result2.Value.ShouldBeTrue();
        }

        [Fact]
        public async Task HasVoted_DifferentProposals_ShouldSucceed()
        {
            await InitializeAndApproveSimpleDaoContract();

            var proposalId1 = await CreateTestProposalAsync();
            var proposalId2 = await CreateTestProposalAsync();

            var voteInput = new VoteInput
            {
                ProposalId = proposalId1,
                Vote = VoteOption.Approved,
                Amount = BallotAmount
            };
            await SimpleDAOStub.Vote.SendAsync(voteInput);

            var input1 = new HasVotedInput
            {
                ProposalId = proposalId1,
                Address = Accounts[0].Address
            };
            var result1 = await SimpleDAOStub.HasVoted.CallAsync(input1);
            result1.ShouldNotBeNull();
            result1.Value.ShouldBeTrue();

            var input2 = new HasVotedInput
            {
                ProposalId = proposalId2,
                Address = Accounts[0].Address
            };
            var result2 = await SimpleDAOStub.HasVoted.CallAsync(input2);
            result2.ShouldNotBeNull();
            result2.Value.ShouldBeFalse();
        }

        private async Task<IExecutionResult<Empty>> InitializeSimpleDaoContract()
        {
            return await SimpleDAOStub.Initialize.SendAsync(new InitializeInput
            {
                TokenSymbol = TokenSymbol
            });
        }
        
        private async Task<string> CreateTestProposalAsync(Timestamp startTime = null, Timestamp expireTime = null)
        {
            var input = new CreateProposalInput
            {
                Title = "Test Proposal",
                Description = "This is a test proposal.",
                EndTimestamp = expireTime ?? BlockTimeProvider.GetBlockTime().AddSeconds(100),
                StartTimestamp = startTime ?? BlockTimeProvider.GetBlockTime()
            };

            var result = await SimpleDAOStub.CreateProposal.SendAsync(input);
            result.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);

            var proposals = await SimpleDAOStub.GetAllProposals.CallAsync(new Empty());
            
            return proposals.Proposals.Count.ToString();
        }
        
        private async Task InitializeAndApproveSimpleDaoContract()
        {
            await InitializeSimpleDaoContract();
            await TokenContractApprove(TokenContractStub);
        }

        private async Task TokenContractApprove(TokenContractContainer.TokenContractStub tokenContractStub)
        {
            await tokenContractStub.Approve.SendAsync(new ApproveInput
            {
                Spender = ContractAddress,
                Symbol = TokenSymbol,
                Amount = 10
            });
        }

        private async Task SendTokenTo(Address address)
        {
            await TokenContractStub.Transfer.SendAsync(new TransferInput
            {
                To = address,
                Symbol = TokenSymbol,
                Amount = 100
            });
        }
    }
    
}