using AzureFunctionsOpenApiFluentValidationExtensions.Rules;
using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

internal interface IRulesMapper
{
    public (string Name, IEnumerable<Rule> Rules) Map(IValidationRule rule);
}
