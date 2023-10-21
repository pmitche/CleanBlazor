using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Requests.Identity;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator(IStringLocalizer<UpdateProfileRequestValidator> localizer)
    {
        RuleFor(request => request.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["First Name is required"]);
        RuleFor(request => request.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Last Name is required"]);
    }
}
