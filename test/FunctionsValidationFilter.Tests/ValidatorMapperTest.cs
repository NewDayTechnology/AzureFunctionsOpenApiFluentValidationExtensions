using FluentValidation;
using FluentValidation.Internal;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter.Tests;

public class ValidatorMapperTest
{
    private readonly ValidatorMapper _mapper = new();

    [Fact]
    public void NotEmptyRule()
    {
        // Arrange
        var component = CreateComponent(x => x.NotEmpty());

        // Act
        var outcome = _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<NotEmptyRule>(result);
    }

    [Fact]
    public void ExactLengthRule()
    {
        // Arrange
        var component = CreateComponent(x => x.Length(6));

        // Act
        var outcome = _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<ExactLengthRule>(result);
        Assert.Equal(6, rule.Length);
    }

    [Fact]
    public void RegularExpressionRule()
    {
        // Arrange
        var component = CreateComponent(x => x.Matches(".*"));

        // Act
        var outcome = _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<RegularExpressionRule>(result);
        Assert.Equal(".*", rule.Regex);
    }

    private static IRuleComponent CreateComponent(Action<IRuleBuilderInitial<Sample, string>> action)
    {
        var validator = new SampleValidator(action).First();
        return validator.Components.First();
    }

    public class Sample
    {
        public string? MyProperty { get; set; }
    }

    public class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator(Action<IRuleBuilderInitial<Sample, string>> action)
        {
            action(RuleFor(request => request.MyProperty));
        }
    }
}
