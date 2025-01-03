using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.SimpleDAO
{
    public partial class SimpleDAO : SimpleDAOContainer.SimpleDAOBase
    {
        private const int StartProposalId = 1;
        
        // Initializes the DAO with a default proposal. Members are defined by token holders.
        public override Empty Initialize(InitializeInput input)
        {
            Assert(!State.Initialized.Value, "already initialized");
            
            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            Assert(State.TokenContract.Value != null, "Cannot find token contract!");
            
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput
            {
                Symbol = input.TokenSymbol
            });
            Assert(!string.IsNullOrEmpty(tokenInfo.Symbol), $"Token {input.TokenSymbol} not found");
            
            State.TokenSymbol.Value = input.TokenSymbol;
            State.NextProposalId.Value = StartProposalId;
            State.Initialized.Value = true;
            
            return new Empty();
        }

        // Creates a new proposal in the DAO. Anyone can create proposals, even non-members.
        public override Empty CreateProposal(CreateProposalInput input)
        {
            Assert(!string.IsNullOrEmpty(input.Title), "Title should not be empty.");
            Assert(!string.IsNullOrEmpty(input.Description), "Description should not be empty.");
            Assert(input.StartTimestamp >= Context.CurrentBlockTime, "Start time should be greater or equal to current block time.");
            Assert(input.EndTimestamp > Context.CurrentBlockTime, "Expire time should be greater than current block time.");
            
            var proposalId = State.NextProposalId.Value.ToString();
            
            var newProposal = new Proposal {
                Id = proposalId, 
                Title = input.Title, 
                Description = input.Description, 
                Proposer = Context.Sender, 
                StartTimestamp = input.StartTimestamp, 
                EndTimestamp = input.EndTimestamp,
                Result = new ProposalResult
                {
                    ApproveCounts = 0,
                    RejectCounts = 0,
                    AbstainCounts = 0
                }
            };
            State.Proposals[proposalId] = newProposal;
            
            State.NextProposalId.Value += 1;
            return new Empty();
        }

        // Casts a vote on a proposal. Only members can vote.
        public override Empty Vote(VoteInput input)
        {
            AssertProposalExists(input.ProposalId);
            var proposal = GetProposal(input.ProposalId);
            Assert(proposal.StartTimestamp <= Context.CurrentBlockTime, $"Proposal {proposal.Id} has not started. Voting is not allowed.");
            Assert(proposal.EndTimestamp > Context.CurrentBlockTime, $"Proposal {proposal.Id} has ended. Voting is not allowed.");
            var amount = input.Amount;
            Assert(amount > 0, "Amount must be greater than 0");
            Assert(State.Voters[proposal.Id][Context.Sender] == false, "You have already voted.");
            
            TransferTokensForProposalBallot(proposal.Id, amount);

            UpdateVoteCounts(proposal, Context.Sender, input.Vote, amount);
            
            return new Empty();
        }
        
        // Withdraws vote from a proposal. Can only be done after the proposal has ended.
        public override Empty Withdraw(WithdrawInput input)
        {
            AssertProposalExists(input.ProposalId);
            var proposal = GetProposal(input.ProposalId);
            Assert(proposal.EndTimestamp <= Context.CurrentBlockTime, $"Proposal {proposal.Id} has not ended. Withdrawal is not allowed.");
            
            TransferTokensForProposalWithdrawal(proposal.Id);

            return new Empty();
        }

        // Returns all proposals in the DAO.
        public override ProposalList GetAllProposals(Empty input)
        {
            // Create a new list called ProposalList
            var proposals = new ProposalList();
            // Start iterating through Proposals from index 0 until the value of NextProposalId, read the corresponding proposal, add it to ProposalList, and finally return ProposalList
            for (var i = StartProposalId; i < State.NextProposalId.Value; i++)
            {
                var proposalCount = i.ToString();
                var proposal = State.Proposals[proposalCount];
                proposals.Proposals.Add(proposal);
            }
            return proposals;
        }
        
        // Get information of a particular proposal.
        public override Proposal GetProposal(StringValue input)
        {
            AssertProposalExists(input.Value);
            
            return GetProposal(input.Value);
        }
        
        // Check if an address has voted
        public override BoolValue HasVoted(HasVotedInput input)
        {
            var id = input.ProposalId;
            AssertProposalExists(id);

            return new BoolValue { Value = State.Voters[id][input.Address] };
        }
        
        public override StringValue GetTokenSymbol(Empty input)
        {
            return new StringValue { Value = State.TokenSymbol.Value };
        }
    }
}