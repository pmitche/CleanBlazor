using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Commands;

public class AddEditBrandCommandValidator : AbstractValidator<AddEditBrandCommand>
{
    public AddEditBrandCommandValidator(IStringLocalizer<AddEditBrandCommandValidator> localizer)
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage(localizer["Name is required!"]);
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage(localizer["Description is required!"]);
        RuleFor(request => request.Tax)
            .GreaterThan(0).WithMessage(localizer["Tax must be greater than 0"]);
    }
}
