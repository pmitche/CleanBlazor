using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Misc;

public partial class DocumentTypes
{
    private bool _bordered;
    private bool _canCreateDocumentTypes;
    private bool _canDeleteDocumentTypes;
    private bool _canEditDocumentTypes;
    private bool _canExportDocumentTypes;
    private bool _canSearchDocumentTypes;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private GetAllDocumentTypesResponse _documentType = new();

    private List<GetAllDocumentTypesResponse> _documentTypeList = new();
    private bool _loaded;
    private string _searchString = "";
    private bool _striped = true;
    [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateDocumentTypes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Create)).Succeeded;
        _canEditDocumentTypes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Edit)).Succeeded;
        _canDeleteDocumentTypes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Delete)).Succeeded;
        _canExportDocumentTypes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Export)).Succeeded;
        _canSearchDocumentTypes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Search)).Succeeded;

        await GetDocumentTypesAsync();
        _loaded = true;
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetDocumentTypesAsync()
    {
        IResult<List<GetAllDocumentTypesResponse>> response = await DocumentTypeManager.GetAllAsync();
        if (response.Succeeded)
        {
            _documentTypeList = response.Data.ToList();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private async Task Delete(int id)
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
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            IResult<int> response = await DocumentTypeManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                await Reset();
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                SnackBar.Success(response.Messages[0]);
            }
            else
            {
                await Reset();
                SnackBar.Error(response.Messages);
            }
        }
    }

    private async Task ExportToExcel()
    {
        IResult<string> response = await DocumentTypeManager.ExportToExcelAsync(_searchString);
        if (response.Succeeded)
        {
            await JsRuntime.InvokeVoidAsync("Download",
                new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(DocumentTypes).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            SnackBar.Success(string.IsNullOrWhiteSpace(_searchString)
                    ? Localizer["Document Types exported"]
                    : Localizer["Filtered Document Types exported"]);
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private async Task InvokeModal(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            _documentType = _documentTypeList.FirstOrDefault(c => c.Id == id);
            if (_documentType != null)
            {
                parameters.Add(nameof(AddEditDocumentTypeModal.AddEditDocumentTypeModel),
                    new AddEditDocumentTypeCommand
                    {
                        Id = _documentType.Id, Name = _documentType.Name, Description = _documentType.Description
                    });
            }
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<AddEditDocumentTypeModal>(id == 0 ? Localizer["Create"] : Localizer["Edit"],
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
        _documentType = new GetAllDocumentTypesResponse();
        await GetDocumentTypesAsync();
    }

    private bool Search(GetAllDocumentTypesResponse brand)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}
