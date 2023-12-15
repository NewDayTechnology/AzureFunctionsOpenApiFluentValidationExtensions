using System.Diagnostics.CodeAnalysis;
using FluentValidation.Internal;
using AzureFunctionsOpenApiFluentValidationExtensions.Rules;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

internal interface IValidatorMapper
{
    public bool TryMap(IRuleComponent component, [NotNullWhen(true)] out Rule? rule);
}
