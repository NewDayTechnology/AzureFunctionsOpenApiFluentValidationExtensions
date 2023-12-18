using AzureFunctionsOpenApiFluentValidationExtensions.Rules;
using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions
{
    internal interface ISchemaMapper
    {
        (string Name, Dictionary<string, List<Rule>> Fields) Map(Type type, IEnumerable<IValidationRule> rules);
    }
}
