using System.Diagnostics.CodeAnalysis;
using FluentValidation.Internal;
using FluentValidation.Validators;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;

internal class ValidatorMapper : IValidatorMapper
{
    public bool TryMap(IRuleComponent component, [NotNullWhen(true)] out Rule? rule)
    {
        rule = component.Validator switch
        {
            INotEmptyValidator => NotEmptyRule.Instance,
            IExactLengthValidator validator => new ExactLengthRule(validator.Max),
            IRegularExpressionValidator validator => new RegularExpressionRule(validator.Expression),
            _ => null
        };

        return rule is not null;
    }
}
