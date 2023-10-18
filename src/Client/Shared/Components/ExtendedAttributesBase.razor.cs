using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.ExtendedAttribute;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Domain.Enums;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Shared.Components;

public class ExtendedAttributesLocalization
{
    // for localization
}

public abstract partial class ExtendedAttributesBase<TId, TEntityId, TEntity, TExtendedAttribute>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByDescription =
        response => response.Description;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByExternalId =
        response => response.ExternalId;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByGroup =
        response => response.Group;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortById = response =>
        response.Id;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByIsActive =
        response => response.IsActive;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByKey = response =>
        response.Key;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByType = response =>
        response.Type;

    private readonly Func<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>, object> _sortByValue = response =>
        response.Type switch
        {
            EntityExtendedAttributeType.Decimal => response.Decimal,
            EntityExtendedAttributeType.Text => response.Text,
            EntityExtendedAttributeType.DateTime => response.DateTime,
            EntityExtendedAttributeType.Json => response.Json,
            _ => response.Text
        };

    private int _activeGroupIndex;
    private bool _bordered;
    private bool _canCreateExtendedAttributes;
    private bool _canDeleteExtendedAttributes;
    private bool _canEditExtendedAttributes;
    private bool _canExportExtendedAttributes;
    private bool _canSearchExtendedAttributes;
    private bool _canViewExtendedAttributes;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> _extendedAttributes = new();
    private bool _includeEntity;
    private bool _loaded;
    private List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>> _model;
    private MudTabs _mudTabs;
    private bool _onlyCurrentGroup;
    private string _searchString = "";
    private GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> _selectedItem = new();
    private bool _striped = true;

    [Inject]
    private IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute> ExtendedAttributeManager
    {
        get;
        set;
    }

    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [Parameter] public string EntityIdString { get; set; }
    [Parameter] public string EntityName { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }

    protected abstract Func<string, TEntityId> FromStringToEntityIdTypeConverter { get; }
    protected abstract string ExtendedAttributesViewPolicyName { get; }
    protected abstract string ExtendedAttributesEditPolicyName { get; }
    protected abstract string ExtendedAttributesCreatePolicyName { get; }
    protected abstract string ExtendedAttributesDeletePolicyName { get; }
    protected abstract string ExtendedAttributesExportPolicyName { get; }
    protected abstract string ExtendedAttributesSearchPolicyName { get; }

    private TEntityId EntityId => FromStringToEntityIdTypeConverter.Invoke(EntityIdString);
    private string CurrentUserId { get; set; }

    private Dictionary<string, List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>
        GroupedExtendedAttributes { get; } = new();

    protected abstract RenderFragment Inherited();

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canViewExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesViewPolicyName)).Succeeded;
        if (!_canViewExtendedAttributes)
        {
            SnackBar.Error(Localizer["Not Allowed."]);
            NavigationManager.NavigateTo("/");
        }

        _canEditExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesEditPolicyName)).Succeeded;
        _canCreateExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesCreatePolicyName)).Succeeded;
        _canDeleteExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesDeletePolicyName)).Succeeded;
        _canExportExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesExportPolicyName)).Succeeded;
        _canSearchExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, ExtendedAttributesSearchPolicyName)).Succeeded;

        await GetExtendedAttributesAsync();
        _loaded = true;

        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = state.User;
        if (user == null)
        {
            return;
        }

        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUserId = user.GetUserId();
        }

        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetExtendedAttributesAsync()
    {
        IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>> response =
            await ExtendedAttributeManager.GetAllByEntityIdAsync(EntityId);
        if (response.Succeeded)
        {
            GroupedExtendedAttributes.Clear();
            _model = response.Data;
            GroupedExtendedAttributes.Add(Localizer["All Groups"], _model);
            foreach (GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> extendedAttribute in _model)
            {
                if (!string.IsNullOrWhiteSpace(extendedAttribute.Group))
                {
                    if (GroupedExtendedAttributes.TryGetValue(extendedAttribute.Group,
                            out List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>> attribute))
                    {
                        attribute.Add(extendedAttribute);
                    }
                    else
                    {
                        GroupedExtendedAttributes.Add(extendedAttribute.Group,
                            new List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>> { extendedAttribute });
                    }
                }
            }

            if (_model != null)
            {
                Description = string.Format(Localizer["Manage {0} {1}'s Extended Attributes"], EntityId, EntityName);
            }
        }
        else
        {
            SnackBar.Error(response.Messages);

            NavigationManager.NavigateTo("/");
        }
    }

    private async Task ExportToExcel()
    {
        var request = new ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        {
            SearchString = _searchString,
            EntityId = EntityId,
            IncludeEntity = _includeEntity,
            OnlyCurrentGroup = _onlyCurrentGroup && _activeGroupIndex != 0,
            CurrentGroup = _mudTabs.Panels[_activeGroupIndex].Text
        };
        IResult<string> response = await ExtendedAttributeManager.ExportToExcelAsync(request);
        if (response.Succeeded)
        {
            await JsRuntime.InvokeVoidAsync("Download",
                new
                {
                    ByteArray = response.Data,
                    FileName = $"{typeof(TExtendedAttribute).Name.ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            SnackBar.Success(string.IsNullOrWhiteSpace(request.SearchString) && !request.IncludeEntity &&
                         !request.OnlyCurrentGroup
                    ? Localizer["Extended Attributes exported"]
                    : Localizer["Filtered Extended Attributes exported"]);
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private async Task InvokeModal(TId id = default)
    {
        var parameters = new DialogParameters();
        if (!id.Equals(default))
        {
            GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> documentExtendedAttribute =
                _model.FirstOrDefault(c => c.Id.Equals(id));
            if (documentExtendedAttribute != null)
            {
                parameters.Add(
                    nameof(
                        AddEditExtendedAttributeModal<TId, TEntityId, TEntity, TExtendedAttribute>
                            .AddEditExtendedAttributeModel),
                    new AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute>
                    {
                        Id = documentExtendedAttribute.Id,
                        EntityId = documentExtendedAttribute.EntityId,
                        Type = documentExtendedAttribute.Type,
                        Key = documentExtendedAttribute.Key,
                        Text = documentExtendedAttribute.Text,
                        Decimal = documentExtendedAttribute.Decimal,
                        DateTime = documentExtendedAttribute.DateTime,
                        Json = documentExtendedAttribute.Json,
                        ExternalId = documentExtendedAttribute.ExternalId,
                        Group = documentExtendedAttribute.Group,
                        Description = documentExtendedAttribute.Description,
                        IsActive = documentExtendedAttribute.IsActive
                    });
            }
        }
        else
        {
            parameters.Add(
                nameof(AddEditExtendedAttributeModal<TId, TEntityId, TEntity, TExtendedAttribute>
                    .AddEditExtendedAttributeModel),
                new AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute>
                {
                    EntityId = EntityId, Type = EntityExtendedAttributeType.Text
                });
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<AddEditExtendedAttributeModal<TId, TEntityId, TEntity, TExtendedAttribute>>(
                id.Equals(default) ? Localizer["Create"] : Localizer["Edit"],
                parameters,
                options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Delete(TId id)
    {
        string deleteContent = Localizer["Delete Extended Attribute?"];
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
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            IResult<TId> response = await ExtendedAttributeManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                await Reset();
                SnackBar.Success(response.Messages[0]);
            }
            else
            {
                await Reset();
                SnackBar.Error(response.Messages);
            }
        }
    }

    private async Task Reset()
    {
        _model = new List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>();
        _searchString = "";
        await GetExtendedAttributesAsync();
    }

    private bool Search(GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> extendedAttributes)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (extendedAttributes.Key.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (extendedAttributes.Text?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.Decimal?.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.DateTime?.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.Json?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.ExternalId?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.Group?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (extendedAttributes.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private static Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
        {
            return Color.Error;
        }

        return selected == all ? Color.Success : Color.Info;
    }
}
