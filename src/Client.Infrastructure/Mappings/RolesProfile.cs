using AutoMapper;
using BlazorHero.CleanArchitecture.Contracts.Identity;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
        CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
    }
}
