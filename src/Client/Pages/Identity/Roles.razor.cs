﻿using System.Net.Http.Json;
using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Roles
{
    private bool _bordered;
    private bool _canCreateRoles;
    private bool _canDeleteRoles;
    private bool _canEditRoles;
    private bool _canSearchRoles;
    private bool _canViewRoleClaims;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;
    private RoleResponse _role = new();

    private List<RoleResponse> _roleList = new();
    private string _searchString = "";
    private bool _striped = true;

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Create))
            .Succeeded;
        _canEditRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Edit)).Succeeded;
        _canDeleteRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete))
            .Succeeded;
        _canSearchRoles = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search))
            .Succeeded;
        _canViewRoleClaims = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.View))
            .Succeeded;

        await GetRolesAsync();
        _loaded = true;
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetRolesAsync()
    {
        var result = await HttpClient.GetFromJsonAsync<Result<List<RoleResponse>>>(RolesEndpoints.GetAll);
        result.HandleWithSnackBar(SnackBar, (_, roles) =>
        {
            _roleList = roles.ToList();
        });
    }

    private async Task Delete(string id)
    {
        string deleteContent = Localizer["Delete Content"];
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
        };
        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<DeleteConfirmation>(Localizer["Delete"], parameters, options);
        DialogResult dialogResult = await dialog.Result;
        if (!dialogResult.Canceled)
        {
            var result = await HttpClient.DeleteFromJsonAsync<Result<string>>(RolesEndpoints.DeleteById(id));
            await Reset();
            await result.HandleWithSnackBarAsync(SnackBar, async messages =>
            {
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                SnackBar.Success(messages[0]);
            });
        }
    }

    private async Task InvokeModal(string id = null)
    {
        var parameters = new DialogParameters();
        if (id != null)
        {
            _role = _roleList.FirstOrDefault(c => c.Id == id);
            if (_role != null)
            {
                parameters.Add(nameof(RoleModal.RoleModel),
                    new RoleRequest { Id = _role.Id, Name = _role.Name, Description = _role.Description });
            }
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog = await DialogService.ShowAsync<RoleModal>(
            id == null ? Localizer["Create"] : Localizer["Edit"],
            parameters,
            options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _role = new RoleResponse();
        await GetRolesAsync();
    }

    private bool Search(RoleResponse role)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private void ManagePermissions(string roleId) =>
        NavigationManager.NavigateTo($"/identity/role-permissions/{roleId}");
}
