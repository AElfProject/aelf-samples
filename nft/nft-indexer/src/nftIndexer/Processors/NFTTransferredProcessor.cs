using AElf.Contracts.MultiToken;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AeFinder.Sdk;
using nftIndexer.Entities;
using Volo.Abp.DependencyInjection;

namespace nftIndexer.Processors;

public class NFTTransferredProcessor : LogEventProcessorBase<Issued>, ITransientDependency
{
    private readonly IBlockChainService _blockChainService;

    public NFTTransferredProcessor(IBlockChainService blockChainService)
    {
        _blockChainService = blockChainService;
    }

    public override string GetContractAddress(string chainId)
    {
        return chainId switch
        {
            "AELF" => "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE",
            "tDVW" => "ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx",
            _ => string.Empty
        };
    }

    public override async Task ProcessAsync(Issued logEvent, LogEventContext context)
    {
        if (!IsNftTransfer(logEvent))
        {
            return;
        }

        var tokenInfoParam = new GetTokenInfoInput
        {
            Symbol = logEvent.Symbol
        };

        var nftTransfer = new TransferRecord
        {
            Id = $"{context.ChainId}-{context.Transaction.TransactionId}-{context.LogEvent.Index}",
            ToAddress = logEvent.To.ToBase58(),
            Symbol = logEvent.Symbol,
            Amount = logEvent.Amount,
            Memo = logEvent.Memo
        };
        await SaveEntityAsync(nftTransfer);

        await ChangeNFTBalanceAsync(context.ChainId, logEvent.To.ToBase58(), logEvent.Symbol, logEvent.Amount);
    }

    private async Task ChangeNFTBalanceAsync(string chainId, string address, string symbol, long amount)
    {
        var accountId = $"{chainId}-{address}-{symbol}";
        var account = await GetEntityAsync<Account>(accountId);
        var tokenInfoParam = new GetTokenInfoInput
        {
            Symbol = symbol
        };
        var contractAddress = GetContractAddress(chainId);
        var tokenInfo = await _blockChainService.ViewContractAsync<TokenInfo>(
            chainId, contractAddress,
            "GetTokenInfo", tokenInfoParam);

        Logger.LogDebug("TokenInfo response: {@TokenInfo}", tokenInfo);

        if (account == null)
        {
            account = new Account
            {
                Id = accountId,
                Symbol = symbol,
                Amount = amount,
                Address = address,
                TokenName = tokenInfo.TokenName,
                NftImageUri = tokenInfo.ExternalInfo?.Value != null 
                    ? (tokenInfo.ExternalInfo.Value.TryGetValue("__nft_image_uri", out var uri) ? uri :
                       tokenInfo.ExternalInfo.Value.TryGetValue("__nft_image_url", out var url) ? url : null)
                    : null,
                NftAttributes = tokenInfo.ExternalInfo?.Value != null 
                    ? (tokenInfo.ExternalInfo.Value.TryGetValue("__nft_attributes", out var attributes) ? attributes : null)
                    : null
            };
        }
        else
        {
            account.Amount += amount;
        }

        Logger.LogDebug("NFT Balance changed: {0} {1} {2} {3}", account.Address, account.Symbol, account.Amount, account.TokenName);

        await SaveEntityAsync(account);
    }

    private bool IsNftTransfer(Issued logEvent)
    {
        return !string.IsNullOrEmpty(logEvent.Symbol) && logEvent.Symbol.Contains("-") &&
            logEvent.Amount > 0 &&
            logEvent.To != null && !string.IsNullOrEmpty(logEvent.To.ToBase58());
    }
}
