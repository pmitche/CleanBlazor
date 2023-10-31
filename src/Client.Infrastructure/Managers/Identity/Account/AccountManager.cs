using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Account;

public class AccountManager : IAccountManager
{
    private readonly HttpClient _httpClient;

    public AccountManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest model)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(AccountsEndpoints.ChangePassword, model);
        return await response.ToResult();
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest model)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(AccountsEndpoints.UpdateProfile, model);
        return await response.ToResult();
    }

    public async Task<Result<string>> GetProfilePictureAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(AccountsEndpoints.GetProfilePicture(userId));
        return await response.ToResult<string>();
    }

    public async Task<Result<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(AccountsEndpoints.UpdateProfilePicture(userId), request);
        return await response.ToResult<string>();
    }
}
