using AElf.Sdk.CSharp.State;
using AElf.Contracts.MultiToken;
using AElf.Types;

namespace AElf.Contracts.SimpleDAO
{
    // The state class is access the blockchain state
    public partial class SimpleDAOState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
    }
}