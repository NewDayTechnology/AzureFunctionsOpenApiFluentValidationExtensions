using FluentValidation;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter
{
    internal interface ISchemaMapper
    {
        (string Name, Dictionary<string, List<Rule>> Fields) Map(Type type, IEnumerable<IValidationRule> rules);
    }
}
