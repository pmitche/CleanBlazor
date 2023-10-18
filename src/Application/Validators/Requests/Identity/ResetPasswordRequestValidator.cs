using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Validators.Requests.Identity;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator(IStringLocalizer<ResetPasswordRequestValidator> localizer)
    {
        RuleFor(request => request.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Email is required"])
            .EmailAddress().WithMessage(localizer["Email is not correct"]);
        RuleFor(request => request.Password)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Password is required!"])
            .MinimumLength(8).WithMessage(localizer["Password must be at least of length 8"])
            .Matches(@"[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"])
            .Matches(@"[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
            .Matches(@"[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
        RuleFor(request => request.ConfirmPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Password Confirmation is required!"])
            .Equal(request => request.Password).WithMessage(localizer["Passwords don't match"]);
        RuleFor(request => request.Token)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Token is required"]);
    }
}
