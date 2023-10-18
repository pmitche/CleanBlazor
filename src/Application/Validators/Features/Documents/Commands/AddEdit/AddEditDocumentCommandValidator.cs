using BlazorHero.CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Validators.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
{
    public AddEditDocumentCommandValidator(IStringLocalizer<AddEditDocumentCommandValidator> localizer)
    {
        RuleFor(request => request.Title)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Title is required!"]);
        RuleFor(request => request.Description)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["Description is required!"]);
        RuleFor(request => request.DocumentTypeId)
            .GreaterThan(0).WithMessage(localizer["Document Type is required!"]);
        RuleFor(request => request.Url)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(localizer["File is required!"]);
    }
}
