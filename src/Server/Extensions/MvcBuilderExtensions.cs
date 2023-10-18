using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Validators.Features.ExtendedAttributes.Commands.AddEdit;
using FluentValidation;

namespace BlazorHero.CleanArchitecture.Server.Extensions;

internal static class MvcBuilderExtensions
{
    internal static void AddExtendedAttributesValidators(this IServiceCollection services)
    {
        #region AddEditExtendedAttributeCommandValidator

        Type addEditExtendedAttributeCommandValidatorType = typeof(AddEditExtendedAttributeCommandValidator<,,,>);
        var validatorTypes = addEditExtendedAttributeCommandValidatorType
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
            .Select(t => new { BaseGenericType = t.BaseType, CurrentType = t })
            .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() ==
                        typeof(AddEditExtendedAttributeCommandValidator<,,,>))
            .ToList();

        foreach (var validatorType in validatorTypes)
        {
            Type addEditExtendedAttributeCommandType =
                typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(validatorType.BaseGenericType
                    .GetGenericArguments());
            Type iValidator = typeof(IValidator<>).MakeGenericType(addEditExtendedAttributeCommandType);
            services.AddScoped(iValidator, validatorType.CurrentType);
        }

        #endregion AddEditExtendedAttributeCommandValidator
    }
}
