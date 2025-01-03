using AeFinder.App.TestBase;
using AeFinder.Sdk.Processor;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using nftIndexer.Processors;
using AeFinder.Sdk;
using Moq;
using nftIndexer.Entities;
using AeFinder.App.BlockChain;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace nftIndexer;

[DependsOn(
    typeof(AeFinderAppTestBaseModule),
    typeof(nftIndexerModule))]
public class nftIndexerTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AeFinderAppEntityOptions>(options => { options.AddTypes<nftIndexerModule>(); });
        
        context.Services.AddTransient<NFTTransferredProcessor>();
        
        // Mock the repository
        var mockRepository = new Mock<IRepository<Account>>();
        context.Services.AddSingleton(mockRepository.Object);

        // Configure blockchain nodes
        Configure<ChainNodeOptions>(options =>
        {
            options.ChainNodes = new Dictionary<string, string>
            {
                { "tDVW", "http://tdvw-test-node.aelf.io" }
            };
        });
    }
}