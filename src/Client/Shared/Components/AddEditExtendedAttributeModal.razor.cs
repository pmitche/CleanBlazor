using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.ExtendedAttribute;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Domain.Enums;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Shared.Components;

public class AddEditExtendedAttributeModalLocalization
{
    // for localization
}

public partial class AddEditExtendedAttributeModal<TId, TEntityId, TEntity, TExtendedAttribute>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private MudDatePicker _datePicker;

    private FluentValidationValidator _fluentValidationValidator;
    private TimeSpan? _time;
    private MudTimePicker _timePicker;

    [Inject]
    private IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute> ExtendedAttributeManager
    {
        get;
        set;
    }

    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> AddEditExtendedAttributeModel
    {
        get;
        set;
    } = new();

    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        switch (AddEditExtendedAttributeModel.Type)
        {
            case EntityExtendedAttributeType.Decimal:
                AddEditExtendedAttributeModel.DateTime = null;
                AddEditExtendedAttributeModel.Text = null;
                AddEditExtendedAttributeModel.Json = null;
                break;
            case EntityExtendedAttributeType.Text:
                AddEditExtendedAttributeModel.Decimal = null;
                AddEditExtendedAttributeModel.DateTime = null;
                AddEditExtendedAttributeModel.Json = null;
                break;
            case EntityExtendedAttributeType.DateTime:
                AddEditExtendedAttributeModel.DateTime ??= new DateTime(0, 0, 0);
                AddEditExtendedAttributeModel.DateTime += _time ?? new TimeSpan(0, 0, 0);
                AddEditExtendedAttributeModel.Decimal = null;
                AddEditExtendedAttributeModel.Text = null;
                AddEditExtendedAttributeModel.Json = null;
                break;
            case EntityExtendedAttributeType.Json:
                AddEditExtendedAttributeModel.Decimal = null;
                AddEditExtendedAttributeModel.Text = null;
                AddEditExtendedAttributeModel.DateTime = null;
                break;
        }

        IResult<TId> response = await ExtendedAttributeManager.SaveAsync(AddEditExtendedAttributeModel);
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
