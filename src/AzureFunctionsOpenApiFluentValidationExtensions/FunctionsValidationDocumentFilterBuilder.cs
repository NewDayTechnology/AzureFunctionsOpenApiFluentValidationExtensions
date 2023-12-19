using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

/// <summary>
/// Builder for the document filter to be used when using static functions or not using DI.
/// </summary>
public class FunctionsValidationDocumentFilterBuilder
{
    private readonly Dictionary<Type, IEnumerable<IValidationRule>> _validators = new();
    private readonly OperationEntryCollection _operations = new();

    /// <summary>
    /// Register validators.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="validator">The validator.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">When the validator provider is not a FluentValidation validator or a validator for that type has already been registerd.</exception>
    public FunctionsValidationDocumentFilterBuilder AddValidator<T>(IValidator<T> validator)
    {
        var validatorType = typeof(T);

        if (_validators.ContainsKey(validatorType)) throw new ArgumentException($"Builder already contains a validator for type {validatorType}", nameof(validator));

        var rules = validator as IEnumerable<IValidationRule>;

        if (rules is null) throw new ArgumentException($"Validators should extend AbstractValidator or implement IEnumerable<IValidationRule>>", nameof(validator));

        _validators.Add(validatorType, rules);

        return this;
    }

    /// <summary>
    /// Register a model which is used to map and manually validate query string parameters of an operation.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="operationName">The identifier of the operation in the OpenAPI spec.</param>
    /// <returns></returns>
    public FunctionsValidationDocumentFilterBuilder AddOperationSchema<T>(string operationName)
    {
        _operations.Add<T>(operationName);

        return this;
    }

    /// <summary>
    /// Register a model which is used to map and manually validate query string parameters of an operation.
    /// </summary>
    /// <param name="operationName">The identifier of the operation in the OpenAPI spec.</param>
    /// <param name="schemaName">The identifier of the model in the OpenAPI spec.</param>
    /// <returns></returns>
    public FunctionsValidationDocumentFilterBuilder AddOperationSchema(string operationName, string schemaName)
    {
        _operations.Add(operationName, schemaName);

        return this;
    }

    /// <summary>
    /// Build the document filter.
    /// </summary>
    /// <returns>The document filter.</returns>
    public FunctionsValidationDocumentFilter Build()
    {
        var validators = _validators.AsEnumerable().Select<
                    KeyValuePair<Type, IEnumerable<IValidationRule>>,
                    (Type Type, IEnumerable<IValidationRule> Rules)>
                    (x => new(x.Key, x.Value));

        var operations = new OperationEntryCollection();
        foreach (var operation in _operations) operations.Add(operation);
        var options = new AzureFunctionsOpenApiFluentValidationExtensionsOptions { Operations = operations };

        var schemaMapper = new SchemaMapper(new RulesMapper(new ValidatorMapper()));

        return new FunctionsValidationDocumentFilter(schemaMapper, validators, options);
    }
}
