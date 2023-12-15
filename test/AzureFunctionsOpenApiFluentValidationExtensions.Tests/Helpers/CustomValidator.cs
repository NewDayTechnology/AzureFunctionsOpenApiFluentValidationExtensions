using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions.Tests;

public class CustomValidator : AbstractValidator<Sample>
{
    public CustomValidator(Action<AbstractValidator<Sample>> action)
    {
        action(this);
    }
}
