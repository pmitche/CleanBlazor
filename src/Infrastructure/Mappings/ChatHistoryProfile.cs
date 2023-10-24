using AutoMapper;
using BlazorHero.CleanArchitecture.Domain.Contracts.Chat;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Models.Chat;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class ChatHistoryProfile : Profile
{
    public ChatHistoryProfile() => CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
}
