using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class ChatHistoryRepository : GenericRepository<ChatHistory<IChatUser>, long>, IChatHistoryRepository
{
    private readonly IMapper _mapper;

    public ChatHistoryRepository(BlazorHeroContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public override IQueryable<ChatHistory<IChatUser>> Entities => DbContext.Set<ChatHistory<BlazorHeroUser>>()
        .ProjectTo<ChatHistory<IChatUser>>(_mapper.ConfigurationProvider);

    public override ChatHistory<IChatUser> Add(ChatHistory<IChatUser> entity)
    {
        var mapped = _mapper.Map<ChatHistory<BlazorHeroUser>>(entity);
        var added = DbContext.Set<ChatHistory<BlazorHeroUser>>().Add(mapped).Entity;
        return _mapper.Map<ChatHistory<IChatUser>>(added);
    }
}
