using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Enums;
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Shared.Components;

public partial class ImportExcelModal
{
    private IBrowserFile _file;

    private FluentValidationValidator _fluentValidationValidator;
    private bool _uploading;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public UploadRequest UploadRequest { get; set; } = new();
    [Parameter] public string ModelName { get; set; }
    [Parameter] public Func<UploadRequest, Task<IResult<int>>> OnSaved { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        if (OnSaved != null)
        {
            _uploading = true;
            IResult<int> result = await OnSaved.Invoke(UploadRequest);
            if (result.Succeeded)
            {
                SnackBar.Success(result.Messages[0]);
                MudDialog.Close();
            }
            else
            {
                SnackBar.Error(result.Messages);
            }

            _uploading = false;
        }
        else
        {
            MudDialog.Close();
        }

        await Task.CompletedTask;
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var buffer = new byte[_file.Size];
            var extension = Path.GetExtension(_file.Name);
            await _file.OpenReadStream(_file.Size).ReadAsync(buffer);
            UploadRequest = new UploadRequest
            {
                Data = buffer, FileName = _file.Name, UploadType = UploadType.Document, Extension = extension
            };
        }
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private static async Task LoadDataAsync() => await Task.CompletedTask;
}
