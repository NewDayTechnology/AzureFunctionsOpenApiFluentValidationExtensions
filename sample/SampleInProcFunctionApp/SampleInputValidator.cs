using FluentValidation;

namespace SampleInProcFunctionApp;

public class SampleInputValidator : AbstractValidator<SampleInput>
{
    public SampleInputValidator()
    {
        RuleFor(model => model.Name).NotNull().NotEmpty();
        RuleFor(model => model.Value).GreaterThan(0);
    }
}
