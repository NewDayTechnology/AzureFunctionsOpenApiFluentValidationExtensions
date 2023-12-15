using FluentValidation;
using Microsoft.OpenApi.Models;
using Moq;
using FunctionsValidationFilter.Rules;

namespace FunctionsValidationFilter.Tests
{
    public class FunctionsValidationDocumentFilterOperationsTest
    {
        private const string SchemaName = "foo";
        private const string PropertyName = "bar";
        private const string OperationId = "sample";

        private readonly Dictionary<OperationType, OpenApiOperation> _operations;
        private readonly OpenApiDocument _document;
        private readonly FunctionsValidationFilterOptions _options;
        private readonly List<(Type Type, IEnumerable<IValidationRule> Rules)> _validators;
        private readonly Mock<ISchemaMapper> _schemaMapperMock;
        private readonly List<Rule> _rules;
        private readonly OpenApiParameter _parameter;

        public FunctionsValidationDocumentFilterOperationsTest()
        {
            _options = new FunctionsValidationFilterOptions()
            {
                Operations = new OperationEntryCollection() { new OperationEntry(OperationId, SchemaName) }
            };
            _validators = new List<(Type Type, IEnumerable<IValidationRule> Rules)>() { (typeof(string), null!) };
            _schemaMapperMock = new Mock<ISchemaMapper>();
            _rules = new List<Rule>();
            var fields = new Dictionary<string, List<Rule>>() { [PropertyName] = _rules };
            _schemaMapperMock.Setup(x => x.Map(typeof(string), null!)).Returns((SchemaName, fields));

            _parameter = new OpenApiParameter() { Name = PropertyName, Schema = new OpenApiSchema() };
            var operation = new OpenApiOperation()
            {
                OperationId = OperationId,
                Parameters = new List<OpenApiParameter>() { _parameter }
            };
            _operations = new Dictionary<OperationType, OpenApiOperation>() { [OperationType.Get] = operation };
            _document = new OpenApiDocument()
            {
                Components = new OpenApiComponents() { Schemas = new Dictionary<string, OpenApiSchema>() },
                Paths = new OpenApiPaths() { ["/path"] = new OpenApiPathItem() { Operations = _operations } }
            };
        }

        [Fact]
        public void ShouldApplyNotEmptyRuleToSchema()
        {
            _rules.Add(NotEmptyRule.Instance);

            Act();

            Assert.True(_parameter.Required);
        }

        [Fact]
        public void ShouldApplyExactLengthRuleToSchema()
        {
            _rules.Add(new ExactLengthRule(3));

            Act();

            Assert.Equal(3, _parameter.Schema.MinLength);
            Assert.Equal(3, _parameter.Schema.MaxLength);
        }

        [Fact]
        public void ShouldApplyRegularExpressionRuleToSchema()
        {
            const string Regex = "^$";
            _rules.Add(new RegularExpressionRule(Regex));

            Act();

            Assert.Equal(Regex, _parameter.Schema.Pattern);
        }

        [Fact]
        public void ShouldIgnoreMissingOperations()
        {
            _operations.Clear();

            Act();
        }

        private void Act()
        {
            var fixture = BuildFixture();
            fixture.Apply(null!, _document);
        }

        private FunctionsValidationDocumentFilter BuildFixture() =>
            new(_schemaMapperMock.Object, _validators, _options);
    }
}
