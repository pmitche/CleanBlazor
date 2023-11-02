using AutoMapper;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserResponse, ApplicationUser>().ReverseMap();
        CreateMap<ChatUserResponse, ApplicationUser>().ReverseMap()
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(source => source.Email)); //Specific Mapping
    }
}
