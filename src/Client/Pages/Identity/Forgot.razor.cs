using System.Net.Http.Json;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Forgot
{
    private readonly ForgotPasswordRequest _emailModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    private async Task SubmitAsync()
    {
        var result = await HttpClient.PostAsJsonAsync<ForgotPasswordRequest, Result>(
            UsersEndpoints.ForgotPassword, _emailModel);
        result.HandleWithSnackBar(SnackBar, _ =>
        {
            SnackBar.Success(Localizer["Done!"]);
            NavigationManager.NavigateTo("/");
        });
    }
}
