﻿using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Enums;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.Products.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Product;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
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
    [Inject] private IProductManager ProductManager { get; set; }
    [Inject] private IBrandManager BrandManager { get; set; }

    [Parameter] public AddEditProductCommand AddEditProductModel { get; set; } = new();
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        IResult<int> response = await ProductManager.SaveAsync(AddEditProductModel);
        if (response.Succeeded)
        {
            SnackBar.Success(response.Messages[0]);
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
            MudDialog.Close();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
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
        IResult<List<GetAllBrandsResponse>> data = await BrandManager.GetAllAsync();
        if (data.Succeeded)
        {
            _brands = data.Data;
        }
    }

    private async Task LoadImageAsync()
    {
        IResult<string> data = await ProductManager.GetProductImageAsync(AddEditProductModel.Id);
        if (data.Succeeded)
        {
            var imageData = data.Data;
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
