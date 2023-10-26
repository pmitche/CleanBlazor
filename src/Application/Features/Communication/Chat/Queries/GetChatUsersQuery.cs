using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Application.Features.Communication.Chat.Queries;

public sealed record GetChatUsersQuery : IQuery<Result<IEnumerable<ChatUserResponse>>>;

internal sealed class GetChatUsersQueryHandler : IQueryHandler<GetChatUsersQuery, Result<IEnumerable<ChatUserResponse>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetChatUsersQueryHandler(
        ICurrentUserService currentUserService,
        IUserService userService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ChatUserResponse>>> Handle(
        GetChatUsersQuery request,
        CancellationToken cancellationToken)
    {
        var userIsAdmin = await _userService.IsInRoleAsync(_currentUserService.UserId, RoleConstants.AdministratorRole);
        var allUsers = await _userService.Users
            .Where(user => user.Id != _currentUserService.UserId && (userIsAdmin || (user.IsActive && user.EmailConfirmed)))
            .ToListAsync(cancellationToken);
        var chatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(allUsers);
        return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
    }
}
