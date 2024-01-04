using FluentValidation;

namespace SampleInProcStaticFunctionApp;

public class SampleInputValidator : AbstractValidator<SampleInput>
{
    public SampleInputValidator()
    {
        RuleFor(model => model.Name).NotNull().NotEmpty();
        RuleFor(model => model.Value).GreaterThan(0);
    }
}
