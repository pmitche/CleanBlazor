using AutoMapper;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration.Mappings;

public class ChatProfile : Profile
{
    public ChatProfile() => CreateMap<ChatMessage<IChatUser>, ChatMessage<BlazorHeroUser>>()
        .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())
        .ReverseMap()
        .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
}
