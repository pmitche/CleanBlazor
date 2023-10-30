using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class ChatMessageRepository : GenericRepository<ChatMessage<IChatUser>, long>, IChatMessageRepository
{
    private readonly IMapper _mapper;

    public ChatMessageRepository(BlazorHeroContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public override IQueryable<ChatMessage<IChatUser>> Entities => DbContext.Set<ChatMessage<BlazorHeroUser>>()
        .ProjectTo<ChatMessage<IChatUser>>(_mapper.ConfigurationProvider);

    public override ChatMessage<IChatUser> Add(ChatMessage<IChatUser> entity)
    {
        var mapped = _mapper.Map<ChatMessage<BlazorHeroUser>>(entity);
        var added = DbContext.Set<ChatMessage<BlazorHeroUser>>().Add(mapped).Entity;
        return _mapper.Map<ChatMessage<IChatUser>>(added);
    }
}
