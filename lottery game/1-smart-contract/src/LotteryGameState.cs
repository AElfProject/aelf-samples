using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace AElf.Contracts.LotteryGame
{
    // The state class is access the blockchain state
    public partial class LotteryGameState : ContractState 
    {
        // A state to check if contract is initialized
        public BoolState Initialized { get; set; }
        // A state to store the owner address
        public SingletonState<Address> Owner { get; set; }
    }
}