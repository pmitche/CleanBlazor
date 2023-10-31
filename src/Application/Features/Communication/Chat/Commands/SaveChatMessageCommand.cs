using BlazorHero.CleanArchitecture.Application.Abstractions.Common;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.Communication.Chat.Commands;

public sealed record SaveChatMessageCommand(string ToUserId, string Message) : ICommand<IResult>;

internal sealed class SaveChatMessageCommandHandler : ICommandHandler<SaveChatMessageCommand, IResult>
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

    public async Task<IResult> Handle(SaveChatMessageCommand request, CancellationToken cancellationToken)
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
        return await Result.SuccessAsync();
    }
}
