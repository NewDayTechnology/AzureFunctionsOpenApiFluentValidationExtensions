using FluentValidation;
using FluentValidation.Internal;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter;

internal class RulesMapper : IRulesMapper
{
    private readonly IValidatorMapper _validatorMapper;

    public RulesMapper(IValidatorMapper validatorMapper)
    {
        _validatorMapper = validatorMapper ?? throw new NullReferenceException(nameof(validatorMapper));
    }

    public (string Name, IEnumerable<Rule> Rules) Map(IValidationRule rule)
    {
        var schemaName = StringHelper.CamelCase(rule.PropertyName);
        var rules = MapRules(rule.Components);

        return (schemaName, rules);
    }

    private IEnumerable<Rule> MapRules(IEnumerable<IRuleComponent> components)
    {
        foreach (var component in components)
        {
            if (_validatorMapper.TryMap(component, out var rule))
            {
                yield return rule;
            }
        }
    }
}
