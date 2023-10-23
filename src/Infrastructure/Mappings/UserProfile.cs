using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Responses.Chat;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserResponse, BlazorHeroUser>().ReverseMap();
        CreateMap<ChatUserResponse, BlazorHeroUser>().ReverseMap()
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(source => source.Email)); //Specific Mapping
    }
}
