using FluentValidation;

namespace SampleFunctionApp;

public class SampleInputValidator : AbstractValidator<SampleInput>
{
    public SampleInputValidator()
    {
        RuleFor(model => model.Name).NotNull().NotEmpty();
        RuleFor(model => model.Value).NotNull().GreaterThan(0);
        RuleFor(model => model.Description).NotNull().MinimumLength(10);
    }
}
