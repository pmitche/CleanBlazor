﻿using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Application.Exceptions;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly IStringLocalizer<ChatService> _localizer;
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly UserManager<BlazorHeroUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ChatService(
        IMapper mapper,
        IUserService userService,
        IStringLocalizer<ChatService> localizer,
        IChatHistoryRepository chatHistoryRepository,
        UserManager<BlazorHeroUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userService = userService;
        _localizer = localizer;
        _chatHistoryRepository = chatHistoryRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId)
    {
        IResult<UserResponse> response = await _userService.GetAsync(userId);
        if (!response.Succeeded)
        {
            throw new ApiException(_localizer["User Not Found!"]);
        }

        List<ChatHistoryResponse> query = await _chatHistoryRepository.Entities
            .Where(h => (h.FromUserId == userId && h.ToUserId == contactId) ||
                        (h.FromUserId == contactId && h.ToUserId == userId))
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
            }).ToListAsync();
        return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(query);

    }

    public async Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId)
    {
        var userIsAdmin = await _userService.IsInRoleAsync(userId, RoleConstants.AdministratorRole);
        List<BlazorHeroUser> allUsers = await _userManager.Users
            .Where(user => user.Id != userId && (userIsAdmin || (user.IsActive && user.EmailConfirmed))).ToListAsync();
        var chatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(allUsers);
        return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message)
    {
        _chatHistoryRepository.Add(message);
        await _unitOfWork.SaveChangesAsync();
        return await Result.SuccessAsync();
    }
}
