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
        var component = CreateStringComponent(x => x.NotEmpty());

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        Assert.IsType<NotEmptyRule>(result);
    }

    [Fact]
    public void NotNullRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.NotNull());

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        Assert.IsType<NotNullRule>(result);
    }

    [Fact]
    public void MaxLengthRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.MaximumLength(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<MaxLengthRule>(result);
        Assert.Equal(6, rule.Max);
    }

    [Fact]
    public void MinLengthRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.MinimumLength(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<MinLengthRule>(result);
        Assert.Equal(6, rule.Min);
    }

    [Fact]
    public void ExactLengthRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.Length(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<ExactLengthRule>(result);
        Assert.Equal(6, rule.Length);
    }

    [Fact]
    public void LengthRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.Length(6, 10));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<LengthRangeRule>(result);
        Assert.Equal(6, rule.Min);
        Assert.Equal(10, rule.Max);
    }

    [Fact]
    public void InclusiveBetweenRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.InclusiveBetween(6, 10));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<InclusiveBetweenRule>(result);
        Assert.Equal(6, rule.Min);
        Assert.Equal(10, rule.Max);
    }

    [Fact]
    public void ExclusiveBetweenRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.ExclusiveBetween(6, 10));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<ExclusiveBetweenRule>(result);
        Assert.Equal(6, rule.Min);
        Assert.Equal(10, rule.Max);
    }

    [Fact]
    public void GreaterThanOrEqualRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.GreaterThanOrEqualTo(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<GreaterThanOrEqualRule>(result);
        Assert.Equal(6, rule.ValueToCompare);
    }

    [Fact]
    public void GreaterThanRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.GreaterThan(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<GreaterThanRule>(result);
        Assert.Equal(6, rule.ValueToCompare);
    }

    [Fact]
    public void LessThanOrEqualRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.LessThanOrEqualTo(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<LessThanOrEqualRule>(result);
        Assert.Equal(6, rule.ValueToCompare);
    }

    [Fact]
    public void LessThanRule()
    {
        // Arrange
        var component = CreateIntComponent(x => x.LessThan(6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<LessThanRule>(result);
        Assert.Equal(6, rule.ValueToCompare);
    }

    [Fact]
    public void RegularExpressionRule()
    {
        // Arrange
        var component = CreateStringComponent(x => x.Matches(".*"));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<RegularExpressionRule>(result);
        Assert.Equal(".*", rule.Regex);
    }

    [Fact]
    public void ScalePrecisionRule()
    {
        // Arrange
        var component = CreateDecimalComponent(x => x.ScalePrecision(2, 6));

        // Act
        _mapper.TryMap(component, out var result);

        // Assert
        var rule = Assert.IsType<ScalePrecisionRule>(result);
        Assert.Equal(2, rule.Scale);
        Assert.Equal(6, rule.Precision);
    }

    private static IRuleComponent CreateStringComponent(Action<IRuleBuilderInitial<Sample, string?>> action)
    {
        var validator = new SampleValidator(action).First();
        return validator.Components.First();
    }

    private static IRuleComponent CreateIntComponent(Action<IRuleBuilderInitial<Sample, int?>> action)
    {
        var validator = new SampleValidator(action).First();
        return validator.Components.First();
    }

    private static IRuleComponent CreateDecimalComponent(Action<IRuleBuilderInitial<Sample, decimal?>> action)
    {
        var validator = new SampleValidator(action).First();
        return validator.Components.First();
    }

    public class Sample
    {
        public string? MyStringProperty { get; set; }
        public int? MyIntProperty { get; set; }
        public decimal? MyDecimalProperty { get; set; }
    }

    public class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator(Action<IRuleBuilderInitial<Sample, string?>> action)
        {
            action(RuleFor(request => request.MyStringProperty));
        }

        public SampleValidator(Action<IRuleBuilderInitial<Sample, int?>> action)
        {
            action(RuleFor(request => request.MyIntProperty));
        }

        public SampleValidator(Action<IRuleBuilderInitial<Sample, decimal?>> action)
        {
            action(RuleFor(request => request.MyDecimalProperty));
        }
    }
}
