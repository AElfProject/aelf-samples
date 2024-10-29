using AElf.Contracts.Consensus.AEDPoS;
using AElf.Contracts.MultiToken;

namespace AElf.Contracts.LotteryGame
{
    public partial class LotteryGameState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }
        internal AEDPoSContractContainer.AEDPoSContractReferenceState ConsensusContract { get; set; }
    }
}