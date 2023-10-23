﻿using System.Text;
using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class Reset
{
    private readonly ResetPasswordRequest _resetPasswordModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _passwordVisibility;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

    protected override void OnInitialized()
    {
        Uri uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Token", out StringValues param))
        {
            var queryToken = param.First();
            _resetPasswordModel.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(queryToken));
        }
    }

    private async Task SubmitAsync()
    {
        if (!string.IsNullOrEmpty(_resetPasswordModel.Token))
        {
            IResult result = await UserManager.ResetPasswordAsync(_resetPasswordModel);
            if (result.Succeeded)
            {
                SnackBar.Success(result.Messages[0]);
                NavigationManager.NavigateTo("/");
            }
            else
            {
                SnackBar.Error(result.Messages);
            }
        }
        else
        {
            SnackBar.Error(Localizer["Token Not Found!"]);
        }
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }
}
