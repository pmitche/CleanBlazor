using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;

namespace BlazorHero.CleanArchitecture.Application.Mappings;

public class BrandProfile : Profile
{
    public BrandProfile()
    {
        CreateMap<AddEditBrandCommand, Brand>().ReverseMap();
        CreateMap<GetBrandByIdResponse, Brand>().ReverseMap();
        CreateMap<GetAllBrandsResponse, Brand>().ReverseMap();
    }
}
