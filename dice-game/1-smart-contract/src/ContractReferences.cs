using AElf.Contracts.MultiToken;
using AetherLink.Contracts.Oracle;

namespace AElf.Contracts.DiceMaster
{
    public partial class DiceMasterState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
        internal OracleContractContainer.OracleContractReferenceState OracleContract { get; set; }
    }
}