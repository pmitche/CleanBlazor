using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Commands;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;

namespace BlazorHero.CleanArchitecture.Application.Configuration.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile() => CreateMap<AddEditProductCommand, Product>().ReverseMap();
}
