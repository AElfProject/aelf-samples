using AeFinder.Sdk;
using GraphQL;
using nftIndexer.Entities;
using Volo.Abp.ObjectMapping;

namespace nftIndexer.GraphQL;

public class Query
{
    public static async Task<List<AccountDto>> Account(
        [FromServices] IReadOnlyRepository<Account> repository,
        [FromServices] IObjectMapper objectMapper,
        GetAccountInput input)
    {
        var queryable = await repository.GetQueryableAsync();

        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);

        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.Address == input.Address);
        }

        if (!input.Symbol.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.Symbol == input.Symbol && a.Symbol.Contains("-"));
        }

        var accounts = queryable.OrderBy(o => o.Metadata.Block.BlockHeight).ToList();

        return objectMapper.Map<List<Account>, List<AccountDto>>(accounts);
    }

    public static async Task<List<TransferRecordDto>> TransferRecord(
        [FromServices] IReadOnlyRepository<TransferRecord> repository,
        [FromServices] IObjectMapper objectMapper,
        GetTransferRecordInput input)
    {
        var queryable = await repository.GetQueryableAsync();

        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);

        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.ToAddress == input.Address);
        }

        // Filter by Symbol, ensuring it matches the NFT pattern
        if (!input.Symbol.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.Symbol == input.Symbol && a.Symbol.Contains("-"));
        }

        // Ensure NFT-specific conditions
        queryable = queryable.Where(a => a.Symbol.Contains("-") && a.Amount > 0);

        var transferRecords = queryable.OrderBy(o => o.Metadata.Block.BlockHeight).ToList();

        return objectMapper.Map<List<TransferRecord>, List<TransferRecordDto>>(transferRecords);
    }
}
