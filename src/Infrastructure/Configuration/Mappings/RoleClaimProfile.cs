using AutoMapper;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration.Mappings;

public class RoleClaimProfile : Profile
{
    public RoleClaimProfile()
    {
        CreateMap<RoleClaimResponse, ApplicationRoleClaim>()
            .ForMember(nameof(ApplicationRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
            .ForMember(nameof(ApplicationRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
            .ReverseMap();

        CreateMap<RoleClaimRequest, ApplicationRoleClaim>()
            .ForMember(nameof(ApplicationRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
            .ForMember(nameof(ApplicationRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
            .ReverseMap();
    }
}
