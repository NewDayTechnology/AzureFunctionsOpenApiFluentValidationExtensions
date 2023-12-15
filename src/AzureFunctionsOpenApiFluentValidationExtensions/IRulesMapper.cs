using FluentValidation;
using AzureFunctionsOpenApiFluentValidationExtensions.Rules;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

internal interface IRulesMapper
{
    public (string Name, IEnumerable<Rule> Rules) Map(IValidationRule rule);
}
