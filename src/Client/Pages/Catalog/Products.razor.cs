using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Features.Products.Commands;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Product;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Catalog;

public partial class Products
{
    private bool _bordered;
    private bool _canCreateProducts;
    private bool _canDeleteProducts;
    private bool _canEditProducts;
    private bool _canExportProducts;
    private bool _canSearchProducts;
    private int _currentPage;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;

    private IEnumerable<GetAllPagedProductsResponse> _pagedData;
    private string _searchString = "";
    private bool _striped = true;
    private MudTable<GetAllPagedProductsResponse> _table;
    private int _totalItems;
    [Inject] private IProductManager ProductManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateProducts = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Create))
            .Succeeded;
        _canEditProducts = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Edit))
            .Succeeded;
        _canDeleteProducts = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Delete))
            .Succeeded;
        _canExportProducts = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Export))
            .Succeeded;
        _canSearchProducts = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Search))
            .Succeeded;

        _loaded = true;
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task<TableData<GetAllPagedProductsResponse>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        await LoadData(state.Page, state.PageSize, state);
        return new TableData<GetAllPagedProductsResponse> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state)
    {
        string[] orderings = null;
        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            orderings = state.SortDirection != SortDirection.None
                ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                : new[] { $"{state.SortLabel}" };
        }

        var request = new GetAllPagedProductsRequest
        {
            PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, OrderBy = orderings
        };
        PaginatedResult<GetAllPagedProductsResponse> response = await ProductManager.GetProductsAsync(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            _pagedData = response.Data;
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task ExportToExcel()
    {
        IResult<string> response = await ProductManager.ExportToExcelAsync(_searchString);
        if (response.Succeeded)
        {
            await JsRuntime.InvokeVoidAsync("Download",
                new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Products).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            SnackBar.Success(string.IsNullOrWhiteSpace(_searchString)
                    ? Localizer["Products exported"]
                    : Localizer["Filtered Products exported"]);
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
            GetAllPagedProductsResponse product = _pagedData.FirstOrDefault(c => c.Id == id);
            if (product != null)
            {
                parameters.Add(nameof(AddEditProductModal.AddEditProductModel),
                    new AddEditProductCommand
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Rate = product.Rate,
                        BrandId = product.BrandId,
                        Barcode = product.Barcode
                    });
            }
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<AddEditProductModal>(id == 0 ? Localizer["Create"] : Localizer["Edit"],
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
        IDialogReference dialog =
            await DialogService.ShowAsync<DeleteConfirmation>(Localizer["Delete"], parameters, options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            IResult<int> response = await ProductManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                OnSearch("");
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                SnackBar.Success(response.Messages[0]);
            }
            else
            {
                OnSearch("");
                SnackBar.Error(response.Messages);
            }
        }
    }
}
