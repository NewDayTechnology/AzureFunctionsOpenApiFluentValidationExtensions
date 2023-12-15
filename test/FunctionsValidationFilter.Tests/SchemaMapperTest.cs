using FluentValidation;
using Moq;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter.Tests;

public class SchemaMapperTest
{
    private readonly Mock<IRulesMapper> _rulesMapperMock;
    private readonly SchemaMapper _mapper;

    public SchemaMapperTest()
    {
        _rulesMapperMock = new Mock<IRulesMapper>(MockBehavior.Strict);
        _mapper = new(_rulesMapperMock.Object);
    }

    [Fact]
    public void ShouldMapSchema()
    {
        // Arrange
        var validationRule = new CustomValidator(validator =>
        {
            validator.RuleFor(x => x.MyProperty).NotEmpty();
        });
        _rulesMapperMock.Setup(x => x.Map(validationRule.First())).Returns(("sample", Array.Empty<Rule>()));

        // Act
        var outcome = _mapper.Map(typeof(Sample), validationRule);

        // Assert
        var field = Assert.Single(outcome.Fields);
        Assert.Equal("sample", field.Key);
        Assert.Empty(field.Value);
    }

    [Fact]
    public void ShouldMapAllRules()
    {
        var validationRule = new CustomValidator(validator =>
        {
            validator.RuleFor(x => x.MyProperty).NotEmpty();
            validator.RuleFor(x => x.OtherProperty).NotEmpty();
        });
        var expectedRule = new ExactLengthRule(1);
        var otherExpectedRule = new ExactLengthRule(2);
        _rulesMapperMock.Setup(x => x.Map(validationRule.First())).Returns(("foo", new[] { expectedRule }));
        _rulesMapperMock.Setup(x => x.Map(validationRule.Skip(1).First())).Returns(("bar", new[] { otherExpectedRule }));

        // Act
        var outcome = _mapper.Map(typeof(Sample), validationRule);

        // Assert
        Assert.Collection(outcome.Fields,
            x =>
            {
                Assert.Equal("foo", x.Key);
                var rule = Assert.Single(x.Value);
                Assert.Same(expectedRule, rule);
            },
            x =>
            {
                Assert.Equal("bar", x.Key);
                var rule = Assert.Single(x.Value);
                Assert.Same(otherExpectedRule, rule);
            });
    }
}
