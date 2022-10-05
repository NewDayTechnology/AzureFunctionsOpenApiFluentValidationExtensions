using FluentValidation;
using Microsoft.OpenApi.Models;
using Moq;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter.Tests
{
    public class FunctionsValidationDocumentFilterSchemasTest
    {
        private readonly OpenApiDocument _document;
        private readonly OpenApiSchema _schema;
        private readonly OpenApiSchema _property;
        private readonly List<(Type Type, IEnumerable<IValidationRule> Rules)> _validators;
        private readonly Mock<ISchemaMapper> _schemaMapperMock;
        private readonly List<Rule> _rules;

        public FunctionsValidationDocumentFilterSchemasTest()
        {
            _validators = new List<(Type Type, IEnumerable<IValidationRule> Rules)>() { (typeof(string), null!) };
            _schemaMapperMock = new Mock<ISchemaMapper>();
            _rules = new List<Rule>();
            var fields = new Dictionary<string, List<Rule>>() { ["bar"] = _rules };
            _schemaMapperMock.Setup(x => x.Map(typeof(string), null!)).Returns(("foo", fields));

            _document = new OpenApiDocument() { Components = new OpenApiComponents() { Schemas = new Dictionary<string, OpenApiSchema>() } };
            _schema = new OpenApiSchema();
            _property = new OpenApiSchema();
            _schema.Properties.Add("bar", _property);
            _document.Components.Schemas.Add("foo", _schema);
        }

        [Fact]
        public void ShouldApplyNotEmptyRuleToSchema()
        {
            _rules.Add(NotEmptyRule.Instance);

            Act();

            Assert.Contains("bar", _schema.Required);
        }

        [Fact]
        public void ShouldApplyExactLengthRuleToSchema()
        {
            _rules.Add(new ExactLengthRule(3));

            Act();

            Assert.Equal(3, _property.MinLength);
            Assert.Equal(3, _property.MaxLength);
        }

        [Fact]
        public void ShouldApplyRegularExpressionRuleToSchema()
        {
            _rules.Add(new RegularExpressionRule("^$"));

            Act();

            Assert.Equal("^$", _property.Pattern);
        }

        [Fact]
        public void ShouldIgnoreMissingProperties()
        {
            _schema.Properties.Clear();

            Act();
        }

        [Fact]
        public void ShouldIgnoreMissingSchemas()
        {
            _document.Components.Schemas.Clear();

            Act();
        }

        [Fact]
        public void ShouldSetDescriptionForScalePrecisionRuleToSchema()
        {
            _rules.Add(new ScalePrecisionRule(2, 6));

            Act();

            Assert.Equal("Must not be more than 6 digits in total, with allowance for 2 decimals.", _property.Description);
        }

        [Fact]
        public void ShouldAppendDescriptionForScalePrecisionRuleToSchema()
        {
            _property.Description = "Something!";
            _rules.Add(new ScalePrecisionRule(2, 6));

            Act();

            Assert.Equal("Something!\n\nMust not be more than 6 digits in total, with allowance for 2 decimals.", _property.Description);
        }

        private void Act()
        {
            var fixture = BuildFixture();
            fixture.Apply(null!, _document);
        }

        private FunctionsValidationDocumentFilter BuildFixture() =>
            new(_schemaMapperMock.Object, _validators, new FunctionsValidationFilterOptions());
    }
}
