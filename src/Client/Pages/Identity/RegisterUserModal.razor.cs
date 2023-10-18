﻿using Blazored.FluentValidation;
using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Identity;

public partial class RegisterUserModal
{
    private readonly RegisterRequest _registerUserModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private bool _passwordVisibility;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private void Cancel() => MudDialog.Cancel();

    private async Task SubmitAsync()
    {
        IResult response = await UserManager.RegisterUserAsync(_registerUserModel);
        if (response.Succeeded)
        {
            SnackBar.Add(response.Messages[0], Severity.Success);
            MudDialog.Close();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                SnackBar.Add(message, Severity.Error);
            }
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
