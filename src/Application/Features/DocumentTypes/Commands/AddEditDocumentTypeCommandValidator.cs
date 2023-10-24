using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands;

public class AddEditDocumentTypeCommandValidator : AbstractValidator<AddEditDocumentTypeCommand>
{
    public AddEditDocumentTypeCommandValidator(IStringLocalizer<AddEditDocumentTypeCommandValidator> localizer)
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage(localizer["Name is required!"]);
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage(localizer["Description is required!"]);
    }
}
