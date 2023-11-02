using CleanBlazor.Application.Abstractions.Common;
using CleanBlazor.Application.Abstractions.Infrastructure.Services;
using CleanBlazor.Application.Abstractions.Messaging;
using CleanBlazor.Application.Abstractions.Persistence;
using CleanBlazor.Domain.Entities.Communication;
using CleanBlazor.Domain.Repositories;
using CleanBlazor.Shared.Wrapper;

namespace CleanBlazor.Application.Features.Communication.Chat.Commands;

public sealed record SaveChatMessageCommand(string ToUserId, string Message) : ICommand<Result>;

internal sealed class SaveChatMessageCommandHandler : ICommandHandler<SaveChatMessageCommand, Result>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveChatMessageCommandHandler(
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService,
        IChatMessageRepository chatMessageRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
        _chatMessageRepository = chatMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SaveChatMessageCommand request, CancellationToken cancellationToken)
    {
        var chatMessage = new ChatMessage<IChatUser>()
        {
            FromUserId = _currentUserService.UserId,
            ToUserId = request.ToUserId,
            CreatedDate = _dateTimeService.NowUtc,
            Message = request.Message
        };
        _chatMessageRepository.Add(chatMessage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
