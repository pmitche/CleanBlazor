using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
{
    public AddEditDocumentCommandValidator(IStringLocalizer<AddEditDocumentCommandValidator> localizer)
    {
        RuleFor(request => request.Title)
            .NotEmpty().WithMessage(localizer["Title is required!"]);
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage(localizer["Description is required!"]);
        RuleFor(request => request.DocumentTypeId)
            .GreaterThan(0).WithMessage(localizer["Document Type is required!"]);
        RuleFor(request => request.Url)
            .NotEmpty().WithMessage(localizer["File is required!"]);
    }
}
