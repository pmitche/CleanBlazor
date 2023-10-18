using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Responses.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile() => CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
}
