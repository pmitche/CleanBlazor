using AutoMapper;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class ChatProfile : Profile
{
    public ChatProfile() => CreateMap<ChatMessage<IChatUser>, ChatMessage<BlazorHeroUser>>().ReverseMap();
}
