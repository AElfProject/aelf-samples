using nftIndexer.Entities;
using nftIndexer.GraphQL;
using AutoMapper;

namespace nftIndexer;

public class nftIndexerAutoMapperProfile : Profile
{
    public nftIndexerAutoMapperProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<TransferRecord, TransferRecordDto>();
    }
}