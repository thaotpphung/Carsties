using System;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelper;

public class MappingProfiles : Profile
{
  public MappingProfiles()
  {
    // Entity to Dto
    CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
    CreateMap<Item, AuctionDto>();

    // Dto to Entity
    CreateMap<CreateAuctionDto, Auction>().ForMember(x => x.Item, opt => opt.MapFrom(y => y));
    CreateMap<CreateAuctionDto, Item>();
  }
}
