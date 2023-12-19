using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions.Tests;

public class SampleValidator : AbstractValidator<Sample>
{
    public SampleValidator()
    {
        RuleFor(x => x.MyProperty).NotEmpty();
    }
}
