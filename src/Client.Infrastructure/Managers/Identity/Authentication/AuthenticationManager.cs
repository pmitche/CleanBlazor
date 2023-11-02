using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Authentication;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly HttpClient _httpClient;
    private readonly IStringLocalizer<AuthenticationManager> _localizer;
    private readonly ILocalStorageService _localStorage;

    public AuthenticationManager(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider,
        IStringLocalizer<AuthenticationManager> localizer)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
        _localizer = localizer;
    }

    public async Task<ClaimsPrincipal> CurrentUser()
    {
        AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }

    public async Task<Result> Login(TokenRequest model)
    {
        var result = await _httpClient.PostAsJsonAsync<TokenRequest, Result<TokenResponse>>(TokenEndpoints.Get, model);
        if (result.IsFailure)
        {
            return Result.Fail(result.ErrorMessages);
        }

        var token = result.Data.Token;
        var refreshToken = result.Data.RefreshToken;
        var userImageUrl = result.Data.UserImageUrl;
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
        if (!string.IsNullOrEmpty(userImageUrl))
        {
            await _localStorage.SetItemAsync(StorageConstants.Local.UserImageUrl, userImageUrl);
        }

        await ((BlazorHeroStateProvider)_authenticationStateProvider).StateChangedAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return Result.Ok();
    }

    public async Task<Result> Logout()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageUrl);
        ((BlazorHeroStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return Result.Ok();
    }

    public async Task<string> RefreshToken()
    {
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

        var request = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken };
        var result = await _httpClient.PostAsJsonAsync<RefreshTokenRequest, Result<TokenResponse>>(
            TokenEndpoints.Refresh, request);

        if (result.IsFailure)
        {
            throw new AuthenticationException(_localizer["Something went wrong during the refresh token action"]);
        }

        token = result.Data.Token;
        refreshToken = result.Data.RefreshToken;
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return token;
    }

    public async Task<string> TryRefreshToken()
    {
        //check if token exists
        var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(availableToken))
        {
            return string.Empty;
        }

        AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        DateTimeOffset expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        DateTime timeUtc = DateTime.UtcNow;
        TimeSpan diff = expTime - timeUtc;
        if (diff.TotalMinutes <= 1)
        {
            return await RefreshToken();
        }

        return string.Empty;
    }

    public async Task<string> TryForceRefreshToken() => await RefreshToken();
}
