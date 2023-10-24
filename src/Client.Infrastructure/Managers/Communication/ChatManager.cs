using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Models.Chat;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public class ChatManager : IChatManager
{
    private readonly HttpClient _httpClient;

    public ChatManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetChatHistory(cId));
        IResult<IEnumerable<ChatHistoryResponse>> data = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
        return data;
    }

    public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetAvailableUsers);
        IResult<IEnumerable<ChatUserResponse>> data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ChatEndpoint.SaveMessage, chatHistory);
        IResult data = await response.ToResult();
        return data;
    }
}
