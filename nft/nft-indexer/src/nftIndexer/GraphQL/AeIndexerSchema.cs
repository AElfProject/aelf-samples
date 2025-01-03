using AeFinder.Sdk;

namespace nftIndexer.GraphQL;

public class AeIndexerSchema : AppSchema<Query>
{
    public AeIndexerSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}