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
            INotNullValidator => NotNullRule.Instance,
            IMaximumLengthValidator validator => new MaxLengthRule(Convert.ToInt32(validator.Max)),
            IMinimumLengthValidator validator => new MinLengthRule(Convert.ToInt32(validator.Min)),
            IExactLengthValidator validator => new ExactLengthRule(validator.Max),
            ILengthValidator validator => new LengthRangeRule(validator.Min, validator.Max),
            IRegularExpressionValidator validator => new RegularExpressionRule(validator.Expression),
            IInclusiveBetweenValidator validator => new InclusiveBetweenRule((int)validator.From, (int)validator.To),
            IBetweenValidator validator => new ExclusiveBetweenRule((int)validator.From, (int)validator.To),
            IGreaterThanOrEqualValidator validator => new GreaterThanOrEqualRule(Convert.ToInt32(validator.ValueToCompare)),
            ILessThanOrEqualValidator validator => new LessThanOrEqualRule(Convert.ToInt32(validator.ValueToCompare)),
            IComparisonValidator validator => SetIComparisonValidatorRules(validator),
            _ => null
        };

        return rule is not null;
    }

    private static Rule? SetIComparisonValidatorRules(IPropertyValidator validator)
    {
        if (validator.Name == "GreaterThanValidator")
        {
            var valueToCompare = ((IComparisonValidator)validator).ValueToCompare;
            if (valueToCompare is int)
            {
                return new GreaterThanRule(Convert.ToInt32(valueToCompare));
            }
        }
        else if (validator.Name == "LessThanValidator")
        {
            var valueToCompare = ((IComparisonValidator)validator).ValueToCompare;
            if (valueToCompare is int)
            {
                return new LessThanRule(Convert.ToInt32(valueToCompare));
            }
        }

        return null;
    }
}

