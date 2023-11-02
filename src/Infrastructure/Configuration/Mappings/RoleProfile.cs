using AutoMapper;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile() => CreateMap<RoleResponse, ApplicationRole>().ReverseMap();
}
