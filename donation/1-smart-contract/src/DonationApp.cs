using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using System.Linq;
using System.Collections.Generic;
using AElf.Contracts.MultiToken;

namespace AElf.Contracts.DonationApp
{
    public class DonationApp : DonationDAppContainer.DonationDAppBase
    {
        // Token contract constants
        private const string TokenSymbol = "ELF";
        private const long MaximumAmount = 1000_00000000; // 1000 ELF

        public override BoolValue IsContractInitialized(Empty input) 
        {
            return new BoolValue { Value = State.Initialized.Value };
        }

        public override StringValue Initialize(Empty input)
        {
            if (State.Initialized.Value)
            {
                return new StringValue { Value = "failed" };
            }

            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            Assert(State.TokenContract.Value != null, "Failed to get token contract address");
            
            State.Initialized.Value = true;
            State.Owner.Value = Context.Sender;
            State.CampaignCount.Value = 0;

            return new StringValue { Value = "success" };
        }

        public override StringValue CreateCampaign(CampaignInput input)
        {
            Assert(State.Initialized.Value, "Contract not initialized.");
            Assert(input.GoalAmount <= MaximumAmount, 
                "Goal amount should be less than equal to 1000 ELF");

            var campaignId = HashHelper.ComputeFrom(input.Title + Context.Sender.ToBase58() + Context.CurrentBlockTime.Seconds).ToHex();
            var currentTime = Context.CurrentBlockTime.Seconds;
            var campaign = new Campaign
            {
                Id = campaignId,
                Title = input.Title,
                Description = input.Description,
                ImageUrl = input.ImageUrl,
                Type = input.Type,
                GoalAmount = input.GoalAmount,
                CurrentAmount = 0,
                Creator = Context.Sender,
                StartTime = currentTime,
                EndTime = currentTime + input.Duration,
                IsActive = true,
                IsWithdrawn = false
            };

            State.Campaigns[campaignId] = campaign;

            // Update user's campaign list
            var userInfo = State.UserInfos[Context.Sender] ?? new UserInfo 
            { 
                Campaigns = { campaignId },
                DonatedCampaigns = { },
                TotalRaisedAmount = 0
            };
            if (!userInfo.Campaigns.Contains(campaignId))
            {
                userInfo.Campaigns.Add(campaignId);
            }
            State.UserInfos[Context.Sender] = userInfo;

            // Add to campaign index
            var currentIndex = State.CampaignCount.Value;
            State.CampaignIdsByIndex[currentIndex] = campaignId;
            State.CampaignCount.Value = currentIndex + 1;

            // Fire campaign created event
            Context.Fire(new CampaignCreatedEvent
            {
                CampaignId = campaignId,
                Title = input.Title,
                Creator = Context.Sender,
                GoalAmount = input.GoalAmount
            });

            return new StringValue { Value = campaignId }; 
        }

        public override Empty Donate(DonationInput input)
        {
            Assert(State.Initialized.Value, "Contract not initialized.");
            var campaign = State.Campaigns[input.CampaignId];
            Assert(campaign != null, "Campaign does not exist.");
            Assert(IsCampaignActive(campaign), "Campaign is not active or has ended.");

            // Check if donor has enough tokens
            var balance = State.TokenContract.GetBalance.Call(new GetBalanceInput
            {
                Owner = Context.Sender,
                Symbol = TokenSymbol
            }).Balance;
            Assert(balance >= input.Amount, "Insufficient balance for donation.");

            // Transfer donation amount
            State.TokenContract.TransferFrom.Send(new TransferFromInput
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = TokenSymbol,
                Amount = input.Amount
            });

            // Record donation
            var donation = new Donation
            {
                Donor = Context.Sender,
                Amount = input.Amount,
                Timestamp = Context.CurrentBlockTime.Seconds
            };
            campaign.Donators.Add(donation);
            campaign.CurrentAmount += input.Amount;

            // Update campaign
            State.Campaigns[input.CampaignId] = campaign;

            // Update user's donation history
            var userInfo = State.UserInfos[Context.Sender] ?? new UserInfo
            {
                Campaigns = { },
                DonatedCampaigns = { input.CampaignId },
                TotalRaisedAmount = input.Amount
            };
            if (!userInfo.DonatedCampaigns.Contains(input.CampaignId))
            {
                userInfo.DonatedCampaigns.Add(input.CampaignId);
                userInfo.TotalRaisedAmount += input.Amount;
            }
            State.UserInfos[Context.Sender] = userInfo;

            // Fire donation made event
            Context.Fire(new DonationMadeEvent
            {
                CampaignId = input.CampaignId,
                Donor = Context.Sender,
                Amount = input.Amount
            });

            return new Empty();
        }

        public override CampaignList GetCampaignsData(Empty input)
        {
            var campaigns = new List<Campaign>();
            var totalCampaigns = State.CampaignCount.Value;
            
            for (var i = 0L; i < totalCampaigns; i++)
            {
                var campaignId = State.CampaignIdsByIndex[i];
                var campaign = State.Campaigns[campaignId];
                if (campaign != null)
                {
                    campaign.IsActive = IsCampaignActive(campaign);
                    campaigns.Add(campaign);
                }
            }

            return new CampaignList { Value = { campaigns } };
        }

        private bool IsCampaignActive(Campaign campaign)
        {
            return campaign.IsActive && 
                   Context.CurrentBlockTime.Seconds <= campaign.EndTime;
        }

        public override Campaign GetCampaign(StringValue campaignId)
        {
            var campaign = State.Campaigns[campaignId.Value];
            campaign.IsActive = IsCampaignActive(campaign);
            return campaign;
        }

        public override CampaignList GetUsersCampaigns(Address input)
        {
            var userInfo = State.UserInfos[input];
            if (userInfo == null || userInfo.Campaigns.Count == 0)
            {
                return new CampaignList();
            }

            var campaigns = new List<Campaign>();
            foreach (var id in userInfo.Campaigns)
            {
                var campaign = State.Campaigns[id];
                if (campaign != null)
                {
                    campaign.IsActive = IsCampaignActive(campaign);
                    campaigns.Add(campaign);
                }
            }

            return new CampaignList { Value = { campaigns } };
        }

        public override Empty EditCampaign(EditCampaignInput input)
        {
            var campaign = State.Campaigns[input.CampaignId];
            Assert(campaign != null, "Campaign does not exist.");
            Assert(campaign.Id == input.CampaignId, "Campaign ID mismatch");
            Assert(campaign.Creator == Context.Sender, "Only the creator can edit the campaign.");

            if (!string.IsNullOrEmpty(input.NewTitle))
                campaign.Title = input.NewTitle;
            if (!string.IsNullOrEmpty(input.NewDescription))
                campaign.Description = input.NewDescription;
            if (!string.IsNullOrEmpty(input.NewImageUrl))
                campaign.ImageUrl = input.NewImageUrl;
            if (!string.IsNullOrEmpty(input.NewType))
                campaign.Type = input.NewType;
            if (input.NewGoalAmount != 0)
                campaign.GoalAmount = input.NewGoalAmount;
            
            campaign.IsActive = input.NewIsActive;

            State.Campaigns[input.CampaignId] = campaign;
            return new Empty();
        }

        public override Empty DeleteCampaign(StringValue input)
        {
            var campaign = State.Campaigns[input.Value];
            Assert(campaign != null, "Campaign does not exist.");
            Assert(campaign.Creator == Context.Sender, "Only the creator can delete the campaign.");

            // Remove campaign
            State.Campaigns.Remove(input.Value);

            // Update user info
            var userInfo = State.UserInfos[Context.Sender];
            if (userInfo != null)
            {
                userInfo.Campaigns.Remove(input.Value);
                State.UserInfos[Context.Sender] = userInfo;
            }

            return new Empty();
        }

        public override Empty WithdrawCampaignAmount(WithdrawCampaignInput input)
        {
            var campaign = State.Campaigns[input.CampaignId];
            Assert(campaign != null, "Campaign does not exist.");
            Assert(campaign.Creator == Context.Sender, "Only the campaign creator can withdraw funds.");
            Assert(Context.CurrentBlockTime.Seconds >= campaign.EndTime, "Campaign duration has not ended yet.");
            Assert(!campaign.IsWithdrawn, "Campaign funds have already been withdrawn.");

            // Transfer campaign amount to creator
            State.TokenContract.Transfer.Send(new TransferInput
            {
                To = campaign.Creator,
                Symbol = TokenSymbol,
                Amount = campaign.CurrentAmount
            });

            // Update withdrawal status
            campaign.IsWithdrawn = true;
            State.Campaigns[input.CampaignId] = campaign;

            // Fire withdrawal event
            Context.Fire(new CampaignWithdrawnEvent
            {
                CampaignId = input.CampaignId,
                Amount = campaign.CurrentAmount,
                Recipient = campaign.Creator
            });

            return new Empty();
        }

        public override UserDetails GetUserDetails(Address input)
        {
            var userInfo = State.UserInfos[input] ?? new UserInfo();
            
            var campaigns = new List<Campaign>();
            foreach (var id in userInfo.Campaigns)
            {
                var campaign = State.Campaigns[id];
                if (campaign != null)
                {
                    campaign.IsActive = IsCampaignActive(campaign);
                    campaigns.Add(campaign);
                }
            }

            var donatedCampaigns = new List<Campaign>();
            foreach (var id in userInfo.DonatedCampaigns)
            {
                var campaign = State.Campaigns[id];
                if (campaign != null)
                {
                    campaign.IsActive = IsCampaignActive(campaign);
                    donatedCampaigns.Add(campaign);
                }
            }

            return new UserDetails
            {
                WalletAddress = input,
                Campaigns = { campaigns },
                DonatedCampaigns = { donatedCampaigns },
                TotalRaisedAmount = userInfo.TotalRaisedAmount
            };
        }
    }
} 