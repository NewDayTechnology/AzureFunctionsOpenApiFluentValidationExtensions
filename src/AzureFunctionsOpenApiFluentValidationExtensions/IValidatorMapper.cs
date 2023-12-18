using System.Diagnostics.CodeAnalysis;
using AzureFunctionsOpenApiFluentValidationExtensions.Rules;
using FluentValidation.Internal;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

internal interface IValidatorMapper
{
    public bool TryMap(IRuleComponent component, [NotNullWhen(true)] out Rule? rule);
}
