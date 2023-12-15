using FluentValidation;

namespace FunctionsValidationFilter.Tests;

public class SampleValidator : AbstractValidator<Sample>
{
    public SampleValidator()
    {
        RuleFor(x => x.MyProperty).NotEmpty();
    }
}
