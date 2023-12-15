using System.Diagnostics.CodeAnalysis;
using FluentValidation.Internal;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter;

internal interface IValidatorMapper
{
    public bool TryMap(IRuleComponent component, [NotNullWhen(true)] out Rule? rule);
}
