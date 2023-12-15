using FluentValidation;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter;

internal interface IRulesMapper
{
    public (string Name, IEnumerable<Rule> Rules) Map(IValidationRule rule);
}
