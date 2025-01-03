using AeFinder.Sdk;
using AeFinder.Sdk.Processor;
using nftIndexer.Entities;
using nftIndexer.Tests.TestHelpers;
using Shouldly;
using Xunit;
using AElf.Types;
using System.Linq;
using System.Collections.Generic;
using Volo.Abp.ObjectMapping;
using AElf.Contracts.MultiToken;

namespace nftIndexer.Processors;

public class NFTTransferredProcessorTests : nftIndexerTestBase
{
    private readonly NFTTransferredProcessor _processor;
    private readonly AeFinder.Sdk.IRepository<Account> _repository;

    public NFTTransferredProcessorTests()
    {
        _processor = GetRequiredService<NFTTransferredProcessor>();
        _repository = GetRequiredService<AeFinder.Sdk.IRepository<Account>>();
    }

    [Fact]
    public async Task Test_NFT_Transfer()
    {
        // Arrange
        var testEvent = new TestIssuedEvent
        {
            To = Address.FromBase58("ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx"),
            Symbol = "SGR-106",
            Amount = 1,
            Memo = "Test NFT"
        };

        var logEventContext = GenerateLogEventContext();

        // Act
        await _processor.ProcessAsync((Issued)testEvent, logEventContext);

        // Assert
        var account = await _repository.GetAsync("SGR-106");
        account.ShouldNotBeNull();
        account.Symbol.ShouldBe("SGR-106");
        account.Amount.ShouldBe(1);
        account.Address.ShouldBe(testEvent.To.ToBase58());
    }

    private LogEventContext GenerateLogEventContext()
    {
        return new LogEventContext
        {
            ChainId = "tDVW",
            Transaction = new AeFinder.Sdk.Processor.Transaction { TransactionId = "test-tx-id" },
            LogEvent = new AeFinder.Sdk.Processor.LogEvent { Index = 0 }
        };
    }
}