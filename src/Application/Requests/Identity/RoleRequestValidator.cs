using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Requests.Identity;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator(IStringLocalizer<RoleRequestValidator> localizer) =>
        RuleFor(request => request.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Name is required"]);
}
