using System.Net.Http.Json;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Products;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Enums;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Catalog;

public partial class AddEditProductModal
{
    private List<GetAllBrandsResponse> _brands = new();

    private IBrowserFile _file;

    private FluentValidationValidator _fluentValidationValidator;

    [Parameter] public AddEditProductRequest AddEditProductModel { get; set; } = new();
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        var result = await HttpClient.PostAsJsonAsync<AddEditProductRequest, Result<int>>(
            ProductsEndpoints.Save, AddEditProductModel);
        await result.HandleWithSnackBarAsync(SnackBar, async messages =>
        {
            SnackBar.Success(messages[0]);
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
            MudDialog.Close();
        });
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

    private async Task LoadDataAsync()
    {
        await LoadImageAsync();
        await LoadBrandsAsync();
    }

    private async Task LoadBrandsAsync()
    {
        var result = await HttpClient.GetFromJsonAsync<Result<List<GetAllBrandsResponse>>>(BrandsEndpoints.GetAll);
        if (result.IsSuccess)
        {
            _brands = result.Data;
        }
    }

    private async Task LoadImageAsync()
    {
        var result = await HttpClient.GetFromJsonAsync<Result<string>>(
                ProductsEndpoints.GetProductImage(AddEditProductModel.Id));
        if (result.IsSuccess)
        {
            var imageData = result.Data;
            if (!string.IsNullOrEmpty(imageData))
            {
                AddEditProductModel.ImageDataUrl = imageData;
            }
        }
    }

    private void DeleteAsync()
    {
        AddEditProductModel.ImageDataUrl = null;
        AddEditProductModel.UploadRequest = new UploadRequest();
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var extension = Path.GetExtension(_file.Name);
            const string format = "image/png";
            IBrowserFile imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            AddEditProductModel.ImageDataUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            AddEditProductModel.UploadRequest =
                new UploadRequest { Data = buffer, UploadType = UploadType.Product, Extension = extension };
        }
    }

    private async Task<IEnumerable<int>> SearchBrands(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _brands.Select(x => x.Id);
        }

        return _brands.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}
