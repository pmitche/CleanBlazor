using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Commands;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;

namespace BlazorHero.CleanArchitecture.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile() => CreateMap<AddEditProductCommand, Product>().ReverseMap();
}
