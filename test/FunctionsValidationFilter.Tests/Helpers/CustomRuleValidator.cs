using FluentValidation;

namespace NewDay.Extensions.FunctionsValidationFilter.Tests;

public class CustomRuleValidator : AbstractValidator<Sample>
{
    public CustomRuleValidator(Action<IRuleBuilderInitial<Sample, string?>> action)
    {
        action(RuleFor(request => request.MyProperty));
    }
}
