using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommandValidator : AbstractValidator<AddEditProductCommand>
{
    public AddEditProductCommandValidator(IStringLocalizer<AddEditProductCommandValidator> localizer)
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage(localizer["Name is required!"]);
        RuleFor(request => request.Barcode)
            .NotEmpty().WithMessage(localizer["Barcode is required!"]);
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage(localizer["Description is required!"]);
        RuleFor(request => request.BrandId)
            .GreaterThan(0).WithMessage(localizer["Brand is required!"]);
        RuleFor(request => request.Rate)
            .GreaterThan(0).WithMessage(localizer["Rate must be greater than 0"]);
    }
}
