using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Requests.Documents;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Misc;

public partial class DocumentStore
{
    private bool _bordered;
    private bool _canCreateDocuments;
    private bool _canDeleteDocuments;
    private bool _canEditDocuments;
    private bool _canSearchDocuments;
    private bool _canViewDocumentExtendedAttributes;
    private int _currentPage;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;

    private IEnumerable<GetAllDocumentsResponse> _pagedData;
    private string _searchString = "";
    private bool _striped = true;
    private MudTable<GetAllDocumentsResponse> _table;
    private int _totalItems;
    [Inject] private IDocumentManager DocumentManager { get; set; }
    private string CurrentUserId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateDocuments = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Create))
            .Succeeded;
        _canEditDocuments = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit))
            .Succeeded;
        _canDeleteDocuments = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Delete))
            .Succeeded;
        _canSearchDocuments = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Search))
            .Succeeded;
        _canViewDocumentExtendedAttributes =
            (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentExtendedAttributes.View))
            .Succeeded;

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
    }

    private async Task<TableData<GetAllDocumentsResponse>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        await LoadData(state.Page, state.PageSize, state);
        return new TableData<GetAllDocumentsResponse> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state)
    {
        var request = new GetAllPagedDocumentsRequest
        {
            PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString
        };
        PaginatedResult<GetAllDocumentsResponse> response = await DocumentManager.GetAllAsync(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            List<GetAllDocumentsResponse> data = response.Data;
            IEnumerable<GetAllDocumentsResponse> loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(_searchString))
                {
                    return true;
                }

                if (element.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (element.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (element.DocumentType.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            });
            switch (state.SortLabel)
            {
                case "documentIdField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Id);
                    break;
                case "documentTitleField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Title);
                    break;
                case "documentDescriptionField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Description);
                    break;
                case "documentDocumentTypeField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, p => p.DocumentType);
                    break;
                case "documentIsPublicField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.IsPublic);
                    break;
                case "documentDateCreatedField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.CreatedOn);
                    break;
                case "documentOwnerField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.CreatedBy);
                    break;
            }

            data = loadedData.ToList();
            _pagedData = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                SnackBar.Add(message, Severity.Error);
            }
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task InvokeModal(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            GetAllDocumentsResponse doc = _pagedData.FirstOrDefault(c => c.Id == id);
            if (doc != null)
            {
                parameters.Add(nameof(AddEditDocumentModal.AddEditDocumentModel),
                    new AddEditDocumentCommand
                    {
                        Id = doc.Id,
                        Title = doc.Title,
                        Description = doc.Description,
                        Url = doc.Url,
                        IsPublic = doc.IsPublic,
                        DocumentTypeId = doc.DocumentTypeId
                    });
            }
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<AddEditDocumentModal>(id == 0 ? Localizer["Create"] : Localizer["Edit"],
                parameters,
                options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            OnSearch("");
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
        IDialogReference dialog = await DialogService.ShowAsync<DeleteConfirmation>(Localizer["Delete"], parameters, options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            IResult<int> response = await DocumentManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                OnSearch("");
                SnackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                OnSearch("");
                foreach (var message in response.Messages)
                {
                    SnackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private void ManageExtendedAttributes(int documentId) =>
        NavigationManager.NavigateTo($"/extended-attributes/{nameof(Document)}/{documentId}");
}
