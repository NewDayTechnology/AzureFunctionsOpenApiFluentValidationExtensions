# FunctionsValidationFilter

Load FluentValidation rules at runtime and it decorates the schema generator with all rules that can be translated in the OpenApi spec.

## Motivation

Currently, the Spec generator in the Microsoft library is not open for extension but there is already an open PR to allow for this. If it doesn’t get merged, it will require working with a fork or internal package.
Azure/azure-functions-openapi-extension#344

Rules can get tricky to convert and there are hard to solve problems (e.g. translating regex dialect) so we should take a pragmatic approach of including only what is easy to port programmatically, otherwise, a manual approach would be better (eventually this could become a validation library to check that most of the rules in the spec match the one in FluentValidation to avoid rules getting out of sync).

## Quick Start

Add to your OpenApi configuration:

```csharp
DocumentFilters.AddFunctionsValidationFilter<Startup>();
```

## Usage

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
