# FunctionsValidationFilter

Decorates the [OpenApi Specification](https://swagger.io/specification/) generated using [Azure Functions OpenAPI Extension](https://github.com/Azure/azure-functions-openapi-extension) with [FluentValidation](https://docs.fluentvalidation.net/en/latest/) rules loaded at runtime.

## Motivation

The recommended library for generating the OpenAPI specification for Azure Functions is [Azure Functions OpenAPI Extension](https://github.com/Azure/azure-functions-openapi-extension). While the library can annotate the model when using `Data Annotations` it doesn't support [FluentValidation](https://docs.fluentvalidation.net/en/latest/).

As `FluentValidation` is a widely used library for building strongly-typed validation rules, this package fills the gap by decorating the `OpenAPI Specification` with the appropriate constraints for HTTP request models, based on the validation rules. It also allows to define rules for HTTP parameters in URLs (see [Advanced Usage](#advanced-usage)).

## Limitations

Currently, only basic rules are supported and the regex dialect is not translated.

## Quick Start

Add to your project:

```xml
<PackageReference Include="FunctionsValidationFilter.DependencyInjection" Version="0.1.19" />
```

Add to your OpenApi configuration:

```csharp
DocumentFilters.AddFunctionsValidationFilter<Startup>();
```

## Usage

Add to your project:

```xml
<PackageReference Include="FunctionsValidationFilter.DependencyInjection" Version="0.1.19" />
```

In your custom object to define the OpenApi configuration, use `AddFunctionsValidationFilter` on the `DocumentFilters` property.

```csharp
public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public OpenApiConfigurationOptions()
    {
        // Add all validators via reflection.
        DocumentFilters.AddFunctionsValidationFilter<Startup>(services =>
        {
            // Add dependencies required to build the validators.
            // Note the OpenAPI generator doesn't use the DI container.
            services.AddSingleton<IClock>(SystemClock.Instance);
        },
        options =>
        {
            // If any operation has query string parameters that you group into an object and use a validator,
            // add it here using the operation id specified for that function endpoint with [OpenApiOperation(operationId: nameof(MyOperation))]
            options.Operations.Add<MyRequestModel>(nameof(MyOperation));
        });
    }

    public override OpenApiInfo Info { get; set; } = new()
    {
        Version = "v1",
        Title = "NewDay 🥳 API"
        // something else
    };
}
```

## Advanced Usage

Add to your project:

```xml
<PackageReference Include="FunctionsValidationFilter" Version="0.1.19" />
```

If you are not using DI in Azure Functions or using static Functions, you would need to manually register all the validators you are using.

```csharp
public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public OpenApiConfigurationOptions()
    {
        var builder = new FunctionsValidationDocumentFilterBuilder();

        // register a validator
        builder.AddValidator(new MyValidator());

        // register an operation which has query string parameters that you group into an object and use a validator,
        // add it here using the operation id specified for that function endpoint with [OpenApiOperation(operationId: nameof(MyOperation))]
        builder.AddOperationSchema<MyRequestModel>(nameof(MyOperation))

        var filter = builder.Build();

        DocumentFilters.Add(filter);
    }

    public override OpenApiInfo Info { get; set; } = new()
    {
        Version = "v1",
        Title = "NewDay 🥳 API"
        // something else
    };
}
```

## Contributing

### Handling new rules

1. Implement a new rule in [`FunctionsValidationFilter.Rules`](https://github.com/NewDayTechnology/FunctionsValidationFilter/tree/main/src/FunctionsValidationFilter/Rules) that extend [`Rule`](https://github.com/NewDayTechnology/FunctionsValidationFilter/blob/main/src/FunctionsValidationFilter/Rules/Rule.cs) and define the required properties. This represents an internal representation of a rule.

1. Add a new case in [`FunctionsValidationFilter.ValidatorMapper`](https://github.com/NewDayTechnology/FunctionsValidationFilter/blob/main/src/FunctionsValidationFilter/ValidatorMapper.cs) that map from a `FluentValidation` type of a rule to the new modeled rule.

1. Extend [`FunctionsValidationFilter.FunctionsValidationDocumentFilter`](https://github.com/NewDayTechnology/FunctionsValidationFilter/blob/main/src/FunctionsValidationFilter/FunctionsValidationDocumentFilter.cs):
    1. Add a new case in the `ApplySchemas` method for the new rule to set the necessary properties of a field of a schema, based on the rule.
    1. Add a new case in the `ApplySchemasToOperations` method for the new rule to set the necessary properties of a parameter of an operation, based on the rule.
