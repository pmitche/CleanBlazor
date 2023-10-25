﻿using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Users;

public class UserManager : IUserManager
{
    private readonly HttpClient _httpClient;

    public UserManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<List<UserResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UsersEndpoints.GetAll);
        return await response.ToResult<List<UserResponse>>();
    }

    public async Task<IResult<UserResponse>> GetAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UsersEndpoints.GetById(userId));
        return await response.ToResult<UserResponse>();
    }

    public async Task<IResult> RegisterUserAsync(RegisterRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UsersEndpoints.Register, request);
        return await response.ToResult();
    }

    public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(UsersEndpoints.ToggleUserStatus(request.UserId), request);
        return await response.ToResult();
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(UsersEndpoints.GetUserRolesById(userId));
        return await response.ToResult<UserRolesResponse>();
    }

    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(UsersEndpoints.GetUserRolesById(request.UserId), request);
        return await response.ToResult<UserRolesResponse>();
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UsersEndpoints.ForgotPassword, request);
        return await response.ToResult();
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(UsersEndpoints.ResetPassword, request);
        return await response.ToResult();
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? UsersEndpoints.Export
            : UsersEndpoints.ExportFiltered(searchString));
        var data = await response.Content.ReadAsStringAsync();
        return data;
    }
}
