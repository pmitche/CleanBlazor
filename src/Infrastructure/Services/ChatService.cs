using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Exceptions;
using BlazorHero.CleanArchitecture.Application.Interfaces.Chat;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Models.Chat;
using BlazorHero.CleanArchitecture.Application.Responses.Chat;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly BlazorHeroContext _context;
    private readonly IStringLocalizer<ChatService> _localizer;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ChatService(
        BlazorHeroContext context,
        IMapper mapper,
        IUserService userService,
        IStringLocalizer<ChatService> localizer)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
        _localizer = localizer;
    }

    public async Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId)
    {
        IResult<UserResponse> response = await _userService.GetAsync(userId);
        if (!response.Succeeded)
        {
            throw new ApiException(_localizer["User Not Found!"]);
        }

        List<ChatHistoryResponse> query = await _context.ChatHistories
            .Where(h => (h.FromUserId == userId && h.ToUserId == contactId) ||
                        (h.FromUserId == contactId && h.ToUserId == userId))
            .OrderBy(a => a.CreatedDate)
            .Include(a => a.FromUser)
            .Include(a => a.ToUser)
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
            }).ToListAsync();
        return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(query);

    }

    public async Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId)
    {
        IResult<UserRolesResponse> userRoles = await _userService.GetRolesAsync(userId);
        var userIsAdmin =
            userRoles.Data?.UserRoles?.Exists(x => x.Selected && x.RoleName == RoleConstants.AdministratorRole) == true;
        List<BlazorHeroUser> allUsers = await _context.Users
            .Where(user => user.Id != userId && (userIsAdmin || (user.IsActive && user.EmailConfirmed))).ToListAsync();
        var chatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(allUsers);
        return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message)
    {
        message.ToUser = await _context.Users.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
        await _context.ChatHistories.AddAsync(_mapper.Map<ChatHistory<BlazorHeroUser>>(message));
        await _context.SaveChangesAsync();
        return await Result.SuccessAsync();
    }
}
