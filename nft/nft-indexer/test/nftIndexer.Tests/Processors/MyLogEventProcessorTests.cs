using AeFinder.Sdk;
using nftIndexer.Entities;
using nftIndexer.GraphQL;
using Shouldly;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace nftIndexer.Processors;

public class MyLogEventProcessorTests: nftIndexerTestBase
{
    // private readonly MyLogEventProcessor _myLogEventProcessor;
    // private readonly IReadOnlyRepository<MyEntity> _repository;
    // private readonly IObjectMapper _objectMapper;
    
    // public MyLogEventProcessorTests()
    // {
    //     _myLogEventProcessor = GetRequiredService<MyLogEventProcessor>();
    //     _repository = GetRequiredService<IReadOnlyRepository<MyEntity>>();
    //     _objectMapper = GetRequiredService<IObjectMapper>();
    // }
    
    // [Fact]
    // public async Task Test()
    // {
    //     var logEvent = new MyLogEvent
    //     {   
    //         ChainId = ChainId,
    //         LogIndex = 1,
    //         TransactionHash = "0x123",
    //         LogType = LogType.Transfer,
    //         LogData = "0x123",
    //         LogDataJson = "0x123",
    //         LogDataJsonBytes = "0x123",
    //         LogDataBytes = "0x123",
    //     };
    //     var logEventContext = GenerateLogEventContext(logEvent);
    //     await _myLogEventProcessor.ProcessAsync(logEvent, logEventContext);
        
    //     var entities = await Query.MyEntity(_repository, _objectMapper, new GetMyEntityInput
    //     {
    //         ChainId = ChainId
    //     });
    //     entities.Count.ShouldBe(1);
    // }
}