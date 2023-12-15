using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation.Internal;
using FluentValidation.Validators;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter;

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
            IPropertyValidator validator => SetIPropertyValidatorRules(validator),
            _ => null
        };

        return rule is not null;
    }

    private static Rule? SetIComparisonValidatorRules(IComparisonValidator validator)
    {
        var type = validator.GetType();

        if (!type.IsGenericType) return null;

        if (type.GetGenericTypeDefinition() == typeof(GreaterThanValidator<,>))
        {
            var valueToCompare = validator.ValueToCompare;
            if (valueToCompare is int value)
            {
                return new GreaterThanRule(value);
            }
        }
        else if (type.GetGenericTypeDefinition() == typeof(LessThanValidator<,>))
        {
            var valueToCompare = validator.ValueToCompare;
            if (valueToCompare is int value)
            {
                return new LessThanRule(value);
            }
        }

        return null;
    }

    private static Rule? SetIPropertyValidatorRules(IPropertyValidator validator)
    {
        var type = validator.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ScalePrecisionValidator<>))
        {
            if (type.GetProperty(nameof(ScalePrecisionValidator<object>.Scale))?.GetValue(validator) is int scale
                && type.GetProperty(nameof(ScalePrecisionValidator<object>.Precision))?.GetValue(validator) is int precision)
            {
                return new ScalePrecisionRule(scale, precision);
            }
        }


        return null;
    }
}

