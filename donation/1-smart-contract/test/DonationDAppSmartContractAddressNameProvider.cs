using AElf.Kernel.SmartContract.Application;
using AElf.Types;

namespace AElf.Contracts.DonationApp
{
    public class DonationDAppSmartContractAddressNameProvider
    {
        public static readonly Hash Name = HashHelper.ComputeFrom("AElf.ContractNames.DonationApp");
    }
} 