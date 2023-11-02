using AutoMapper;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration.Mappings;

public class RoleClaimProfile : Profile
{
    public RoleClaimProfile()
    {
        CreateMap<RoleClaimResponse, BlazorHeroRoleClaim>()
            .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
            .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
            .ReverseMap();

        CreateMap<RoleClaimRequest, BlazorHeroRoleClaim>()
            .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
            .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
            .ReverseMap();
    }
}
