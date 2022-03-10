using FluentValidation;

namespace NewDay.Extensions.FunctionsValidationFilter.Tests;

public class CustomValidator : AbstractValidator<Sample>
{
    public CustomValidator(Action<AbstractValidator<Sample>> action)
    {
        action(this);
    }
}
