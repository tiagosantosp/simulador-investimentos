using AutoMapper;
using Investimentos.Domain.Entities;
using Investimentos.Application.DTOs;

namespace Investimentos.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Asset, AssetDto>();
            CreateMap<CreatePortfolioDto, Portfolio>();

            CreateMap<Position, PositionDto>()
                .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.CurrentPrice : 0))
                .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.Asset != null ? src.Quantity * src.Asset.CurrentPrice : 0));

            CreateMap<Portfolio, PortfolioDto>();
        }
    }
}