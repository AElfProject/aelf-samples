using AElf.Sdk.CSharp.State;
using AElf.Types;
using System.Collections.Generic;

namespace AElf.Contracts.DonationApp
{
    public partial class DonationAppState : ContractState
    {
        // A state to check if contract is initialized
        public BoolState Initialized { get; set; }
        
        // A state to store the owner address
        public SingletonState<Address> Owner { get; set; }
        
        // Maps campaign ID to Campaign
        public MappedState<string, Campaign> Campaigns { get; set; }
        
        // Maps user address to their info (campaigns and donations)
        public MappedState<Address, UserInfo> UserInfos { get; set; }
        
        // Maps campaign ID to list of rewards sent
        public MappedState<string, DonationList> CampaignRewards { get; set; }

        // Store campaign IDs by index
        public MappedState<long, string> CampaignIdsByIndex { get; set; }
        
        // Total number of campaigns
        public SingletonState<long> CampaignCount { get; set; }
    }
} 