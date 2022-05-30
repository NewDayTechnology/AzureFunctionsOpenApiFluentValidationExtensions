using System.Diagnostics;
using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.OpenApi.Models;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;

/// <inheritdoc/>
public class FunctionsValidationDocumentFilter : IDocumentFilter
{
    private readonly Dictionary<string, Dictionary<string, List<Rule>>> _schemaCollection;
    private readonly FunctionsValidationFilterOptions _options;

    internal FunctionsValidationDocumentFilter(ISchemaMapper schemaMapper, IEnumerable<(Type Type, IEnumerable<IValidationRule> Rules)> validators, FunctionsValidationFilterOptions options)
    {
        _schemaCollection = validators
            .Select(x => schemaMapper.Map(x.Type, x.Rules))
            .ToDictionary(x => x.Name, x => x.Fields);
        _options = options;
    }

    /// <inheritdoc/>
    public void Apply(IHttpRequestDataObject request, OpenApiDocument document)
    {
        ApplySchemas(document);

        ApplySchemasToOperations(document);
    }

    private void ApplySchemas(OpenApiDocument document)
    {
        foreach (var item in _schemaCollection)
        {
            try
            {
                var schema = document.Components.Schemas[item.Key];
                foreach (var field in item.Value)
                {
                    var prop = schema.Properties[field.Key];
                    foreach (var rule in field.Value)
                    {
                        switch (rule)
                        {
                            case NotEmptyRule:
                                schema.Required.Add(field.Key);
                                break;
                            case ExactLengthRule exactLengthRule:
                                prop.MaxLength = prop.MinLength = exactLengthRule.Length;
                                break;
                            case RegularExpressionRule regularExpressionRule:
                                prop.Pattern = regularExpressionRule.Regex;
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private void ApplySchemasToOperations(OpenApiDocument document)
    {
        foreach (var operationEntry in _options.Operations)
        {
            try
            {
                var schema = _schemaCollection[StringHelper.CamelCase(operationEntry.SchemaName)];
                var operation = Find(document.Paths, operationEntry.OperationName);

                foreach (var field in schema)
                {
                    var parameter = operation.Parameters.Single(x => x.Name == field.Key);
                    foreach (var rule in field.Value)
                    {
                        switch (rule)
                        {
                            case NotEmptyRule:
                                parameter.Required = true;
                                break;
                            case ExactLengthRule exactLengthRule:
                                parameter.Schema.MaxLength = parameter.Schema.MinLength = exactLengthRule.Length;
                                break;
                            case RegularExpressionRule regularExpressionRule:
                                parameter.Schema.Pattern = regularExpressionRule.Regex;
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private OpenApiOperation Find(OpenApiPaths paths, string v)
    {
        foreach (var path in paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                if (operation.Value.OperationId == v) return operation.Value;
            }
        }

        throw new Exception($"Operation '{v}' is not present.");
    }
}
