using System.Net.Http.Headers;
using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;

namespace BlazorHero.CleanArchitecture.Client.Authentication;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticationHeaderHandler(ILocalStorageService localStorage)
        => _localStorage = localStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Scheme == "Bearer")
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var savedToken =
            await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken, cancellationToken);

        if (!string.IsNullOrWhiteSpace(savedToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
