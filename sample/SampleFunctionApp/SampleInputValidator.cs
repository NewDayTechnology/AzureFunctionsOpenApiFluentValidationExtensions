using FluentValidation;
using SampleFunctionApp;

namespace NewDay.Extensions.SampleFunctionApp;

public class SampleInputValidator : AbstractValidator<SampleInput>
{
    public SampleInputValidator()
    {
        RuleFor(model => model.Name).NotNull().NotEmpty();
        RuleFor(model => model.Value).GreaterThan(0);
    }
}
