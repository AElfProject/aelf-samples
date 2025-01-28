using System;
using System.Threading.Tasks;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.DonationApp
{
    public class DonationDAppTests : DonationDAppTestBase
    {
        [Fact]
        public async Task GetCampaignsData_Empty_Success()
        {
            // Act
            var result = await DonationContract.GetCampaignsData.CallAsync(new Empty());

            // Assert
            result.Value.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetUsersCampaigns_Empty_Success()
        {
            // Act
            var result = await DonationContract.GetUsersCampaigns.CallAsync(DefaultAddress);

            // Assert
            result.Value.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetUserDetails_Empty_Success()
        {
            // Act
            var result = await DonationContract.GetUserDetails.CallAsync(DefaultAddress);

            // Assert
            result.WalletAddress.ShouldBe(DefaultAddress);
            result.Campaigns.Count.ShouldBe(0);
            result.DonatedCampaigns.Count.ShouldBe(0);
            result.TotalRaisedAmount.ShouldBe(0);
        }
    }
} 