using System.Net.Http.Json;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Misc;

public partial class AddEditDocumentTypeModal
{
    private FluentValidationValidator _fluentValidationValidator;

    [Parameter] public AddEditDocumentTypeRequest AddEditDocumentTypeModel { get; set; } = new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        var result = await HttpClient.PostAsJsonAsync<AddEditDocumentTypeRequest, Result<int>>(
            DocumentTypesEndpoints.Save, AddEditDocumentTypeModel);
        result.HandleWithSnackBar(SnackBar, messages =>
        {
            SnackBar.Success(messages[0]);
            MudDialog.Close();
        });

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
