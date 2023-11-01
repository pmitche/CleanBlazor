﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Shared;

public partial class MainBody
{
    private bool _drawerOpen = true;
    private bool _rightToLeft = false;

    private HubConnection _hubConnection;

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public EventCallback OnDarkModeToggle { get; set; }

    [Parameter] public EventCallback<bool> OnRightToLeftToggle { get; set; }

    [Inject] private ILogger<MainBody> Logger { get; set; }

    private string CurrentUserId { get; set; }
    private string ImageDataUrl { get; set; }
    private string FirstName { get; set; }
    private string SecondName { get; set; }
    private string Email { get; set; }
    private char FirstLetterOfName { get; set; }
    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

    private async Task RightToLeftToggle()
    {
        var isRtl = await ClientPreferenceManager.ToggleLayoutDirection();
        _rightToLeft = isRtl;

        await OnRightToLeftToggle.InvokeAsync(isRtl);
    }

    public async Task ToggleDarkMode() => await OnDarkModeToggle.InvokeAsync();

    protected override async Task OnInitializedAsync()
    {
        _rightToLeft = await ClientPreferenceManager.IsRtl();
        Interceptor.RegisterEvent();
        _hubConnection = _hubConnection.TryInitialize(NavigationManager, LocalStorage);
        await _hubConnection.StartAsync();
        _hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification,
            async (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId != receiverUserId)
                {
                    return;
                }

                await JsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                SnackBar.Add(message,
                    Severity.Info,
                    config =>
                    {
                        config.VisibleStateDuration = 10000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.Action = Localizer["Chat?"];
                        config.ActionColor = Color.Primary;
                        config.Onclick = _ =>
                        {
                            NavigationManager.NavigateTo($"chat/{senderUserId}");
                            return Task.CompletedTask;
                        };
                    });
            });
        _hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens,
            async () =>
            {
                try
                {
                    var token = await AuthenticationManager.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        SnackBar.Success(Localizer["Refreshed Token."]);
                        HttpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    SnackBar.Error(Localizer["You are Logged Out."]);
                    await AuthenticationManager.Logout();
                    NavigationManager.NavigateTo("/");
                }
            });
        _hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole,
            async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResult = await HttpClient.GetFromJsonAsync<Result<List<RoleResponse>>>(RolesEndpoints.GetAll);
                    if (rolesResult.IsSuccess)
                    {
                        RoleResponse role = rolesResult.Data.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var userRolesResult = await HttpClient.GetFromJsonAsync<Result<UserRolesResponse>>(
                                UsersEndpoints.GetUserRolesById(CurrentUserId));
                            if (userRolesResult.IsSuccess &&
                                userRolesResult.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                SnackBar.Error(
                                    Localizer[
                                        "You are logged out because the Permissions of one of your Roles have been updated."]);
                                await _hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect,
                                    CurrentUserId);
                                await AuthenticationManager.Logout();
                                NavigationManager.NavigateTo("/login");
                            }
                        }
                    }
                }
            });
        _hubConnection.On<string>(ApplicationConstants.SignalR.PingRequest,
            async userName =>
            {
                await _hubConnection.SendAsync(ApplicationConstants.SignalR.PingResponse, CurrentUserId, userName);
            });

        await _hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);

        SnackBar.Success(string.Format(Localizer["Welcome {0}"], FirstName));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = state.User;
        if (user == null)
        {
            return;
        }

        if (user.Identity?.IsAuthenticated == true)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                CurrentUserId = user.GetUserId();
                FirstName = user.GetFirstName();
                if (FirstName.Length > 0)
                {
                    FirstLetterOfName = FirstName[0];
                }

                SecondName = user.GetLastName();
                Email = user.GetEmail();
                var profilePictureResult = await HttpClient.GetFromJsonAsync<Result<string>>(
                    AccountsEndpoints.GetProfilePicture(CurrentUserId));
                if (profilePictureResult.IsSuccess)
                {
                    ImageDataUrl = profilePictureResult.Data;
                }

                var result =
                    await HttpClient.GetFromJsonAsync<Result<UserResponse>>(UsersEndpoints.GetById(CurrentUserId));
                if (result.IsFailure || result.Data == null)
                {
                    SnackBar.Error(
                        Localizer["You are logged out because the user with your Token has been deleted."]);
                    CurrentUserId = string.Empty;
                    ImageDataUrl = string.Empty;
                    FirstName = string.Empty;
                    SecondName = string.Empty;
                    Email = string.Empty;
                    FirstLetterOfName = char.MinValue;
                    await AuthenticationManager.Logout();
                }
            }
        }
    }

    private void DrawerToggle() => _drawerOpen = !_drawerOpen;

    private void Logout()
    {
        var parameters = new DialogParameters
        {
            { nameof(Dialogs.Logout.ContentText), $"{Localizer["Logout Confirmation"]}" },
            { nameof(Dialogs.Logout.ButtonText), $"{Localizer["Logout"]}" },
            { nameof(Dialogs.Logout.Color), Color.Error },
            { nameof(Dialogs.Logout.CurrentUserId), CurrentUserId },
            { nameof(Dialogs.Logout.HubConnection), _hubConnection }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        DialogService.Show<Logout>(Localizer["Logout"], parameters, options);
    }
}
