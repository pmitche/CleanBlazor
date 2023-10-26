using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Communication.Chat.Queries;

public sealed record GetChatHistoryQuery(string ContactId) : IQuery<Result<IEnumerable<ChatHistoryResponse>>>;

internal sealed class GetChatHistoryQueryHandler : IQueryHandler<GetChatHistoryQuery, Result<IEnumerable<ChatHistoryResponse>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly IStringLocalizer<GetChatHistoryQueryHandler> _localizer;

    public GetChatHistoryQueryHandler(
        ICurrentUserService currentUserService,
        IUserService userService,
        IChatHistoryRepository chatHistoryRepository,
        IStringLocalizer<GetChatHistoryQueryHandler> localizer)
    {
        _currentUserService = currentUserService;
        _userService = userService;
        _chatHistoryRepository = chatHistoryRepository;
        _localizer = localizer;
    }

    public async Task<Result<IEnumerable<ChatHistoryResponse>>> Handle(
        GetChatHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var response = await _userService.GetAsync(_currentUserService.UserId);
        if (!response.Succeeded)
        {
            return await Result<IEnumerable<ChatHistoryResponse>>.FailAsync(_localizer["User Not Found!"]);
        }

        List<ChatHistoryResponse> chatMessages = await _chatHistoryRepository.Entities
            .Where(h => (h.FromUserId == _currentUserService.UserId && h.ToUserId == request.ContactId) ||
                        (h.FromUserId == request.ContactId && h.ToUserId == _currentUserService.UserId))
            .OrderBy(a => a.CreatedDate)
            .Select(x => new ChatHistoryResponse
            {
                FromUserId = x.FromUserId,
                FromUserFullName = $"{x.FromUser.FirstName} {x.FromUser.LastName}",
                Message = x.Message,
                CreatedDate = x.CreatedDate,
                Id = x.Id,
                ToUserId = x.ToUserId,
                ToUserFullName = $"{x.ToUser.FirstName} {x.ToUser.LastName}",
                ToUserImageUrl = x.ToUser.ProfilePictureDataUrl,
                FromUserImageUrl = x.FromUser.ProfilePictureDataUrl
            }).ToListAsync(cancellationToken);

        return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(chatMessages);
    }
}
