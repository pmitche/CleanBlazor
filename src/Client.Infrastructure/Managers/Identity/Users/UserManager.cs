using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Application.Responses.Identity;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Users;

public class UserManager : IUserManager
{
    private readonly HttpClient _httpClient;

    public UserManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<List<UserResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UserEndpoints.GetAll);
        return await response.ToResult<List<UserResponse>>();
    }

    public async Task<IResult<UserResponse>> GetAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UserEndpoints.Get(userId));
        return await response.ToResult<UserResponse>();
    }

    public async Task<IResult> RegisterUserAsync(RegisterRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UserEndpoints.Register, request);
        return await response.ToResult();
    }

    public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UserEndpoints.ToggleUserStatus, request);
        return await response.ToResult();
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UserEndpoints.GetUserRoles(userId));
        return await response.ToResult<UserRolesResponse>();
    }

    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(UserEndpoints.GetUserRoles(request.UserId), request);
        return await response.ToResult<UserRolesResponse>();
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UserEndpoints.ForgotPassword, request);
        return await response.ToResult();
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UserEndpoints.ResetPassword, request);
        return await response.ToResult();
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? UserEndpoints.Export
            : UserEndpoints.ExportFiltered(searchString));
        var data = await response.Content.ReadAsStringAsync();
        return data;
    }
}
