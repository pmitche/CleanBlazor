using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string url,
        TRequest request,
        JsonSerializerOptions options = null)
    {
        using var response = await httpClient.PostAsJsonAsync(url, request, options);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(options);
    }

    public static async Task<TResponse> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string url,
        TRequest request,
        JsonSerializerOptions options = null)
    {
        using var response = await httpClient.PutAsJsonAsync(url, request, options);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(options);
    }
}
