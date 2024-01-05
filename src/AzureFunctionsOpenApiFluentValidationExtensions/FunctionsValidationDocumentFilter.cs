using System.Diagnostics;
using System.Reflection.Metadata;
using AzureFunctionsOpenApiFluentValidationExtensions.Rules;
using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.OpenApi.Models;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

/// <inheritdoc/>
public class FunctionsValidationDocumentFilter : IDocumentFilter
{
    private readonly Dictionary<string, Dictionary<string, List<Rule>>> _schemaCollection;
    private readonly AzureFunctionsOpenApiFluentValidationExtensionsOptions _options;

    internal FunctionsValidationDocumentFilter(ISchemaMapper schemaMapper, IEnumerable<(Type Type, IEnumerable<IValidationRule> Rules)> validators, AzureFunctionsOpenApiFluentValidationExtensionsOptions options)
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

                    if (prop is null) continue;

                    foreach (var rule in field.Value)
                    {
                        switch (rule)
                        {
                            case NotEmptyRule:
                            case NotNullRule:
                                schema.Required.Add(field.Key);
                                break;
                        }
                        ApplyRuleToSchemaProperty(rule, prop, field.Key);
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
                            case NotNullRule:
                                parameter.Required = true;
                                break;
                        }
                        ApplyRuleToSchemaProperty(rule, parameter.Schema, field.Key);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private void ApplyRuleToSchemaProperty(Rule rule, OpenApiSchema prop, string key)
    {
        switch (rule)
        {
            case ExactLengthRule exactLengthRule:
                prop.MaxLength = prop.MinLength = exactLengthRule.Length;
                break;
            case MaxLengthRule maxLengthRule:
                prop.MaxLength = maxLengthRule.Max;
                break;
            case MinLengthRule minLengthRule:
                prop.MinLength = minLengthRule.Min;
                break;
            case LengthRangeRule lengthRangeRule:
                prop.MinLength = lengthRangeRule.Min != 0 ? lengthRangeRule.Min : null;
                prop.MaxLength = lengthRangeRule.Max != 0 ? lengthRangeRule.Max : null;
                break;
            case GreaterThanRule greaterThanRule:
                prop.Minimum = greaterThanRule.ValueToCompare;
                prop.ExclusiveMinimum = true;
                break;
            case GreaterThanOrEqualRule greaterThanOrEqualRule:
                prop.Minimum = greaterThanOrEqualRule.ValueToCompare;
                break;
            case LessThanRule lessThanRule:
                prop.Maximum = lessThanRule.ValueToCompare;
                prop.ExclusiveMaximum = true;
                break;
            case LessThanOrEqualRule lessThanOrEqualRule:
                prop.Maximum = lessThanOrEqualRule.ValueToCompare;
                break;
            case InclusiveBetweenRule inclusiveBetweenRule:
                prop.Minimum = inclusiveBetweenRule.Min;
                prop.Maximum = inclusiveBetweenRule.Max;
                break;
            case ExclusiveBetweenRule exclusiveBetweenRule:
                prop.Minimum = exclusiveBetweenRule.Min;
                prop.ExclusiveMinimum = true;
                prop.Maximum = exclusiveBetweenRule.Max;
                prop.ExclusiveMaximum = true;
                break;
            case RegularExpressionRule regularExpressionRule:
                prop.Pattern = regularExpressionRule.Regex;
                break;
            case ScalePrecisionRule scalePrecisionRule:
                AppendDescription(prop,
                    $"Must not be more than {scalePrecisionRule.Precision} digits in total, with allowance for {scalePrecisionRule.Scale} decimals.");
                break;
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

    private void AppendDescription(OpenApiSchema schema, string description)
    {
        if (string.IsNullOrEmpty(schema.Description))
        {
            schema.Description = description;
        }
        else
        {
            // SwaggerUI render \n\n as new lines.
            schema.Description += "\n\n" + description;
        }
    }
}
