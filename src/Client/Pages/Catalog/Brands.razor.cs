using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.Import;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;
using BlazorHero.CleanArchitecture.Client.Shared.Components;
using BlazorHero.CleanArchitecture.Client.Shared.Dialogs;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Catalog;

public partial class Brands
{
    private bool _bordered;
    private GetAllBrandsResponse _brand = new();

    private List<GetAllBrandsResponse> _brandList = new();
    private bool _canCreateBrands;
    private bool _canDeleteBrands;
    private bool _canEditBrands;
    private bool _canExportBrands;
    private bool _canImportBrands;
    private bool _canSearchBrands;

    private ClaimsPrincipal _currentUser;
    private bool _dense;
    private bool _loaded;
    private string _searchString = "";
    private bool _striped = true;
    [Inject] private IBrandManager BrandManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Create))
            .Succeeded;
        _canEditBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Edit)).Succeeded;
        _canDeleteBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Delete))
            .Succeeded;
        _canExportBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Export))
            .Succeeded;
        _canSearchBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Search))
            .Succeeded;
        _canImportBrands = (await AuthorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Import))
            .Succeeded;

        await GetBrandsAsync();
        _loaded = true;

        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetBrandsAsync()
    {
        IResult<List<GetAllBrandsResponse>> response = await BrandManager.GetAllAsync();
        if (response.Succeeded)
        {
            _brandList = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                SnackBar.Add(message, Severity.Error);
            }
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
            IResult<int> response = await BrandManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                await Reset();
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                SnackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    SnackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task ExportToExcel()
    {
        IResult<string> response = await BrandManager.ExportToExcelAsync(_searchString);
        if (response.Succeeded)
        {
            await JsRuntime.InvokeVoidAsync("Download",
                new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Brands).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            SnackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? Localizer["Brands exported"]
                    : Localizer["Filtered Brands exported"],
                Severity.Success);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                SnackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task InvokeModal(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            _brand = _brandList.FirstOrDefault(c => c.Id == id);
            if (_brand != null)
            {
                parameters.Add(nameof(AddEditBrandModal.AddEditBrandModel),
                    new AddEditBrandCommand
                    {
                        Id = _brand.Id, Name = _brand.Name, Description = _brand.Description, Tax = _brand.Tax
                    });
            }
        }

        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog =
            await DialogService.ShowAsync<AddEditBrandModal>(id == 0 ? Localizer["Create"] : Localizer["Edit"],
                parameters,
                options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task<IResult<int>> ImportExcel(UploadRequest uploadFile)
    {
        var request = new ImportBrandsCommand { UploadRequest = uploadFile };
        IResult<int> result = await BrandManager.ImportAsync(request);
        return result;
    }

    private async Task InvokeImportModal()
    {
        var parameters = new DialogParameters
        {
            { nameof(ImportExcelModal.ModelName), Localizer["Brands"].ToString() }
        };
        Func<UploadRequest, Task<IResult<int>>> importExcel = ImportExcel;
        parameters.Add(nameof(ImportExcelModal.OnSaved), importExcel);
        var options = new DialogOptions
        {
            CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true
        };
        IDialogReference dialog = await DialogService.ShowAsync<ImportExcelModal>(Localizer["Import"], parameters, options);
        DialogResult result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _brand = new GetAllBrandsResponse();
        await GetBrandsAsync();
    }

    private bool Search(GetAllBrandsResponse brand)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
    }
}
