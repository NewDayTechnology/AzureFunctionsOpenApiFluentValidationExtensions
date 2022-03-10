using FluentValidation;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;

internal interface IRulesMapper
{
    public (string Name, IEnumerable<Rule> Rules) Map(IValidationRule rule);
}
