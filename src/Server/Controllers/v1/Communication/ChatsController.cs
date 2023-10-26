using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Features.Communication.Chat.Commands;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.Communication;

[Authorize(Policy = Permissions.Communication.Chat)]
public class ChatsController : BaseApiController
{
    private readonly IChatService _chatService;
    private readonly ICurrentUserService _currentUserService;

    public ChatsController(ICurrentUserService currentUserService, IChatService chatService)
    {
        _currentUserService = currentUserService;
        _chatService = chatService;
    }

    /// <summary>
    ///     Get user wise chat history
    /// </summary>
    /// <param name="contactId"></param>
    /// <returns>Status 200 OK</returns>
    //Get user wise chat history
    [HttpGet("{contactId}")]
    public async Task<IActionResult> GetChatHistoryAsync(string contactId) =>
        Ok(await _chatService.GetChatHistoryAsync(_currentUserService.UserId, contactId));

    /// <summary>
    ///     get available users
    /// </summary>
    /// <returns>Status 200 OK</returns>
    //get available users - sorted by date of last message if exists
    [HttpGet("users")]
    public async Task<IActionResult> GetChatUsersAsync() =>
        Ok(await _chatService.GetChatUsersAsync(_currentUserService.UserId));

    /// <summary>
    ///     Save Chat Message
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Status 200 OK</returns>
    //save chat message
    [HttpPost]
    public async Task<IActionResult> SaveMessageAsync(ChatHistory<IChatUser> message) =>
        Ok(await Mediator.Send(new SaveChatMessageCommand(message.ToUserId, message.Message)));
}
