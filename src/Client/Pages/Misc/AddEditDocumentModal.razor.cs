using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Enums;
using BlazorHero.CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.DocumentType;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Enums;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Misc;

public partial class AddEditDocumentModal
{
    private List<GetAllDocumentTypesResponse> _documentTypes = new();

    private IBrowserFile _file;

    private FluentValidationValidator _fluentValidationValidator;
    [Inject] private IDocumentManager DocumentManager { get; set; }
    [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }

    [Parameter] public AddEditDocumentCommand AddEditDocumentModel { get; set; } = new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        IResult<int> response = await DocumentManager.SaveAsync(AddEditDocumentModel);
        if (response.Succeeded)
        {
            SnackBar.Success(response.Messages[0]);
            MudDialog.Close();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private async Task LoadDataAsync() => await LoadDocumentTypesAsync();

    private async Task LoadDocumentTypesAsync()
    {
        IResult<List<GetAllDocumentTypesResponse>> data = await DocumentTypeManager.GetAllAsync();
        if (data.Succeeded)
        {
            _documentTypes = data.Data;
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var buffer = new byte[_file.Size];
            var extension = Path.GetExtension(_file.Name);
            const string format = "application/octet-stream";
            await _file.OpenReadStream(_file.Size).ReadAsync(buffer);
            AddEditDocumentModel.Url = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            AddEditDocumentModel.UploadRequest = new UploadRequest
            {
                Data = buffer, UploadType = UploadType.Document, Extension = extension
            };
        }
    }

    private async Task<IEnumerable<int>> SearchDocumentTypes(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _documentTypes.Select(x => x.Id);
        }

        return _documentTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}
