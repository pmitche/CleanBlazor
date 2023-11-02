using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Routes;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class RoleModal
{
    private FluentValidationValidator _fluentValidationValidator;

    [Parameter] public RoleRequest RoleModel { get; set; } = new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    protected override async Task OnInitializedAsync()
    {
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task SaveAsync() =>
        await HttpClient.PostAsJsonAsync<RoleRequest, Result<string>>(RolesEndpoints.Save, RoleModel)
            .Match(async (message, _) =>
                {
                    SnackBar.Success(message);
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    MudDialog.Close();
                },
                errors => SnackBar.Error(errors));
}
