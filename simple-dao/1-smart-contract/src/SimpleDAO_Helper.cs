using AElf.Contracts.MultiToken;
using AElf.Types;

namespace AElf.Contracts.SimpleDAO
{
    public partial class SimpleDAO
    {
        private static Hash GetVirtualAddressHash(Address user, string proposalId)
        {
            return HashHelper.ConcatAndCompute(HashHelper.ComputeFrom(user), HashHelper.ComputeFrom(proposalId));
        }
    
        private Address GetVirtualAddress(Address user, string proposalId)
        {
            return Context.ConvertVirtualAddressToContractAddress(GetVirtualAddressHash(user, proposalId));
        }
        
        private Address GetVirtualAddress(Hash virtualAddressHash)
        {
            return Context.ConvertVirtualAddressToContractAddress(virtualAddressHash);
        }
        
        private void TransferFrom(Address from, Address to, string symbol, long amount)
        {
            State.TokenContract.TransferFrom.Send(
                new TransferFromInput
                {
                    Symbol = symbol,
                    Amount = amount,
                    From = from,
                    Memo = "TransferIn",
                    To = to
                });
        }
        
        private void TransferFromVirtualAddress(Hash virtualAddressHash, Address to, string symbol, long amount)
        {
            State.TokenContract.Transfer.VirtualSend(virtualAddressHash,
                new TransferInput
                {
                    Symbol = symbol,
                    Amount = amount,
                    Memo = "TransferOut",
                    To = to
                });
        }
        
        private void AssertProposalExists(string proposalId)
        {
            var proposal = GetProposal(proposalId);
            Assert(proposal != null, "Proposal not found.");
        }
        
        private Proposal GetProposal(string proposalId)
        {
            return State.Proposals[proposalId];
        }
        
        private void TransferTokensForProposalBallot(string proposalId, long amount)
        {
            var virtualAddress = GetVirtualAddress(Context.Sender, proposalId);
            TransferFrom(Context.Sender, virtualAddress, State.TokenSymbol.Value, amount);
        }
        
        private void TransferTokensForProposalWithdrawal(string proposalId)
        {
            var virtualAddressHash = GetVirtualAddressHash(Context.Sender, proposalId);

            var output = State.TokenContract.GetBalance.VirtualCall(virtualAddressHash, new GetBalanceInput
            {
                Symbol = State.TokenSymbol.Value,
                Owner = GetVirtualAddress(virtualAddressHash)
            });
            
            TransferFromVirtualAddress(virtualAddressHash, Context.Sender, State.TokenSymbol.Value, output.Balance);
        }
        
        private void UpdateVoteCounts(Proposal proposal, Address voter, VoteOption voteOption, long amount)
        {
            switch (voteOption)
            {
                case VoteOption.Approved:
                    proposal.Result.ApproveCounts += amount;
                    break;
                case VoteOption.Rejected:
                    proposal.Result.RejectCounts += amount;
                    break;
                case VoteOption.Abstained:
                    proposal.Result.AbstainCounts += amount;
                    break;
                default:
                    Assert(false, "Vote Option is invalid.");
                    break;
            }
            
            State.Voters[proposal.Id][voter] = true;
        }
    }
}