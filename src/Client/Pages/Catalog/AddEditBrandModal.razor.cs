using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Catalog;

public partial class AddEditBrandModal
{
    private FluentValidationValidator _fluentValidationValidator;
    [Inject] private IBrandManager BrandManager { get; set; }

    [Parameter] public AddEditBrandCommand AddEditBrandCommandModel { get; set; } = new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        IResult<int> response = await BrandManager.SaveAsync(AddEditBrandCommandModel);
        if (response.Succeeded)
        {
            SnackBar.Success(response.Messages[0]);
            MudDialog.Close();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }

        await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private static async Task LoadDataAsync() => await Task.CompletedTask;
}
