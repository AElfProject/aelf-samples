using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace AElf.Contracts.DiceMaster
{
    // The state class is access the blockchain state
    public partial class DiceMasterState : ContractState 
    {
        // A state to check if contract is initialized
        public BoolState Initialized { get; set; }
        // A state to store the owner address
        public SingletonState<Address> Owner { get; set; }
        public MappedState<Hash, PlayedRecord> PlayedRecords { get; set; }
        public SingletonState<int> OracleNodeId { get; set; }
        public SingletonState<long> SubscriptionId { get; set; }
        public MappedState<Address, PlayerInfo> PlayerInfos { get; set; }
    }
}