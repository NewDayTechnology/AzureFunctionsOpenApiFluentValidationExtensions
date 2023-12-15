using FluentValidation;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter
{
    internal interface ISchemaMapper
    {
        (string Name, Dictionary<string, List<Rule>> Fields) Map(Type type, IEnumerable<IValidationRule> rules);
    }
}
