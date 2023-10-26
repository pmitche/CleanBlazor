using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public class ChatManager : IChatManager
{
    private readonly HttpClient _httpClient;

    public ChatManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<IEnumerable<ChatMessageResponse>>> GetChatHistoryAsync(string chatId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetChatHistory(chatId));
        IResult<IEnumerable<ChatMessageResponse>> data = await response.ToResult<IEnumerable<ChatMessageResponse>>();
        return data;
    }

    public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetAvailableUsers);
        IResult<IEnumerable<ChatUserResponse>> data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<IResult> SaveMessageAsync(ChatMessage<IChatUser> chatMessage)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ChatEndpoint.SaveMessage, chatMessage);
        IResult data = await response.ToResult();
        return data;
    }
}
