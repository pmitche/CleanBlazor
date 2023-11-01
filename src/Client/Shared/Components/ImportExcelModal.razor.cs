using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Shared.Enums;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Polly.Caching;

namespace BlazorHero.CleanArchitecture.Client.Shared.Components;

public partial class ImportExcelModal
{
    private IBrowserFile _file;

    private FluentValidationValidator _fluentValidationValidator;
    private bool _uploading;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public UploadRequest UploadRequest { get; set; } = new();
    [Parameter] public string ModelName { get; set; }
    [Parameter] public Func<UploadRequest, Task<Result<int>>> OnSaved { get; set; }
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    public void Cancel() => MudDialog.Cancel();

    private async Task SaveAsync()
    {
        if (OnSaved != null)
        {
            _uploading = true;
            Result<int> result = await OnSaved.Invoke(UploadRequest);
            result.HandleWithSnackBar(SnackBar, messages =>
            {
                SnackBar.Success(messages[0]);
                MudDialog.Close();
            });

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
