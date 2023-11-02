using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Contracts.Chat;

namespace BlazorHero.CleanArchitecture.Application.Configuration.Mappings;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatUserResponse, IUser>().ReverseMap()
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(source => source.Email)); //Specific Mapping
    }
}
