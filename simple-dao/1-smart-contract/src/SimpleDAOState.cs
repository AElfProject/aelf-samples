using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace AElf.Contracts.SimpleDAO
{
    // The state class is access the blockchain state
    public partial class SimpleDAOState : ContractState
    {
        public BoolState Initialized { get; set; }
        public MappedState<string, Proposal> Proposals { get; set; }
        public MappedState<string, Address, bool> Voters { get; set; }
        public Int32State NextProposalId { get; set; }
        public StringState TokenSymbol { get; set; }
    }
}