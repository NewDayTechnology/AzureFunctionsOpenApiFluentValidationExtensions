using FluentValidation.Validators;

namespace NewDay.Extensions.FunctionsValidationFilter.Tests;

public class ValidatorInspectorTest
{
    [Fact]
    public void ShouldGetType()
    {
        var result = ValidatorInspector.TryGetRules(typeof(SampleValidator), out var type, out var _);

        Assert.True(result);
        Assert.Equal(typeof(Sample), type);
    }

    [Fact]
    public void ShouldGetRules()
    {
        var result = ValidatorInspector.TryGetRules(typeof(SampleValidator), out var _, out var rules);

        Assert.True(result);
        var rule = Assert.Single(rules);
        Assert.Equal("MyProperty", rule.PropertyName);
        var component = Assert.Single(rule.Components);
        Assert.IsType<NotEmptyValidator<Sample, string>>(component.Validator);
    }

    [Fact]
    public void ShouldNotAllowNull()
    {
        Assert.Throws<ArgumentNullException>(() => ValidatorInspector.TryGetRules(null!, out var _, out var _));
    }

    [Fact]
    public void ShouldIgnoreOtherTypes()
    {
        var result = ValidatorInspector.TryGetRules(typeof(string), out var _, out var _);

        Assert.False(result);
    }
}
