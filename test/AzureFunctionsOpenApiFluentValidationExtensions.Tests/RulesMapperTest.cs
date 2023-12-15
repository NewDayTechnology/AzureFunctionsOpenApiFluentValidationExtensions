using FluentValidation;
using FluentValidation.Internal;
using AzureFunctionsOpenApiFluentValidationExtensions.Rules;
using Moq;

namespace AzureFunctionsOpenApiFluentValidationExtensions.Tests;

public class RulesMapperTest
{
    private readonly Mock<IValidatorMapper> _validatorMapperMock;
    private readonly RulesMapper _mapper;

    public RulesMapperTest()
    {
        _validatorMapperMock = new Mock<IValidatorMapper>(MockBehavior.Strict);
        _mapper = new(_validatorMapperMock.Object);
    }

    [Fact]
    public void ShouldMapProperty()
    {
        // Arrange
        var validationRule = CreateValidationRule(x => x.NotEmpty());
        Rule? expectedRule = NotEmptyRule.Instance;
        _validatorMapperMock.Setup(x => x.TryMap(validationRule.Components.Single(), out expectedRule)).Returns(true);

        // Act
        var outcome = _mapper.Map(validationRule);

        // Assert
        var rule = Assert.Single(outcome.Rules);
        Assert.Equal(expectedRule, rule);
    }

    [Fact]
    public void ShouldHavePropertyName()
    {
        // Arrange
        var validationRule = CreateValidationRule(x => x.NotEmpty());
        Rule? rule;
        _validatorMapperMock.Setup(x => x.TryMap(It.IsAny<IRuleComponent>(), out rule)).Returns(true);

        // Act
        var outcome = _mapper.Map(validationRule);

        // Assert
        Assert.Equal("myProperty", outcome.Name);
    }

    [Fact]
    public void ShouldSkipRulesThatCannotBeMapped()
    {
        // Arrange
        var validationRule = CreateValidationRule(x => x.NotEmpty());
        Rule? rule;
        _validatorMapperMock.Setup(x => x.TryMap(It.IsAny<IRuleComponent>(), out rule)).Returns(false);

        // Act
        var outcome = _mapper.Map(validationRule);

        // Assert
        Assert.Empty(outcome.Rules);
    }

    [Fact]
    public void ShouldMapAllPropertyRules()
    {
        // Arrange
        var validationRule = CreateValidationRule(x => x.NotEmpty().Length(6));
        Rule? expectedRule = NotEmptyRule.Instance;
        Rule? otherExpectedRule = new ExactLengthRule(6);
        _validatorMapperMock.Setup(x => x.TryMap(validationRule.Components.First(), out expectedRule)).Returns(true).Verifiable();
        _validatorMapperMock.Setup(x => x.TryMap(validationRule.Components.Skip(1).First(), out otherExpectedRule)).Returns(true).Verifiable();

        // Act
        var outcome = _mapper.Map(validationRule);

        // Assert
        Assert.Collection(outcome.Rules,
            x => Assert.Equal(expectedRule, x),
            x => Assert.Equal(otherExpectedRule, x));
        _validatorMapperMock.VerifyAll();
    }

    private static IValidationRule CreateValidationRule(Action<IRuleBuilderInitial<Sample, string?>> action)
    {
        return new CustomRuleValidator(action).First();
    }
}
