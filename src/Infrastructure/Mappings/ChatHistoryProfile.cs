using AutoMapper;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class ChatHistoryProfile : Profile
{
    public ChatHistoryProfile() => CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
}
