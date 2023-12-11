using System.Diagnostics;
using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NewDay.Extensions.FunctionsValidationFilter;

namespace Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;

/// <summary>
/// Extensions for configuring OpenAPI with FLuentValidation rules.
/// </summary>
public static class OpenApiConfigurationOptionsExtensions
{
    /// <summary>
    /// Register the FluentValidation decorator to the DocumentFilter, which loads all validators via reflection.
    /// </summary>
    /// <typeparam name="T">The Startup type, to be used to determine the assembly in which to discover the FluentValidation validators.</typeparam>
    /// <param name="filters">The <see cref="List{IDocumentFilter}"/> filters.</param>
    public static void AddFunctionsValidationFilter<T>(this List<IDocumentFilter> filters)
    {
        filters.Add(Build<T>());
    }

    /// <summary>
    /// Register the FluentValidation decorator to the DocumentFilter, which loads all validators via reflection.
    /// </summary>
    /// <typeparam name="T">The Startup type, to be used to determine the assembly in which to discover the FluentValidation validators.</typeparam>
    /// <param name="filters">The <see cref="List{IDocumentFilter}"/> filters.</param>
    /// <param name="configureServices">A lambda to configure the ServiceCollection with extra services required to construct the FluentValidation validators.</param>

    public static void AddFunctionsValidationFilter<T>(this List<IDocumentFilter> filters, Action<ServiceCollection> configureServices)
    {
        filters.Add(Build<T>(configureServices));
    }

    /// <summary>
    /// Register the FluentValidation decorator to the DocumentFilter, which loads all validators via reflection.
    /// </summary>
    /// <typeparam name="T">The Startup type, to be used to determine the assembly in which to discover the FluentValidation validators.</typeparam>
    /// <param name="filters">The <see cref="List{IDocumentFilter}"/> filters.</param>
    /// <param name="configureOptions">A lambda to configure the options of FunctionsValidationFilter.</param>
    public static void AddFunctionsValidationFilter<T>(this List<IDocumentFilter> filters, Action<FunctionsValidationFilterOptions> configureOptions)
    {
        filters.Add(Build<T>(configureOptions: configureOptions));
    }

    /// <summary>
    /// Register the FluentValidation decorator to the DocumentFilter, which loads all validators via reflection.
    /// </summary>
    /// <typeparam name="T">The Startup type, to be used to determine the assembly in which to discover the FluentValidation validators.</typeparam>
    /// <param name="filters">The <see cref="List{IDocumentFilter}"/> filters.</param>
    /// <param name="configureServices">A lambda to configure the ServiceCollection with extra services required to construct the FluentValidation validators.</param>
    /// <param name="configureOptions">A lambda to configure the options of FunctionsValidationFilter.</param>
    public static void AddFunctionsValidationFilter<T>(this List<IDocumentFilter> filters, Action<ServiceCollection> configureServices, Action<FunctionsValidationFilterOptions> configureOptions)
    {
        filters.Add(Build<T>(configureServices, configureOptions));
    }

    private static FunctionsValidationDocumentFilter Build<T>(Action<ServiceCollection>? configureServices = null, Action<FunctionsValidationFilterOptions>? configureOptions = null)
    {
        var services = new ServiceCollection();
        AddValidators<T>(services);
        configureServices?.Invoke(services);
        services.AddOptions<FunctionsValidationFilterOptions>();
        if (configureOptions is not null) services.Configure(configureOptions);

        using var serviceProvider = services.BuildServiceProvider();

        var validators = GetValidators(services, serviceProvider);
        var options = serviceProvider.GetRequiredService<IOptions<FunctionsValidationFilterOptions>>();
        var schemaMapper = new SchemaMapper(new RulesMapper(new ValidatorMapper()));

        return new FunctionsValidationDocumentFilter(schemaMapper, validators, options.Value);
    }

    private static IServiceCollection AddValidators<T>(IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<T>();
    }

    private static IEnumerable<(Type Type, IEnumerable<IValidationRule> Rules)> GetValidators(ServiceCollection services, ServiceProvider serviceProvider)
    {
        foreach (var service in services)
        {
            if (service.ServiceType.IsGenericType && service.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            {
                var x = serviceProvider.GetService((Type?)service.ServiceType);
                var rules = x as IEnumerable<IValidationRule>;

                if (rules is not null)
                {
                    yield return (Type: service.ServiceType.GetGenericArguments()[0], Rules: rules);
                }
                else
                {
                    Debug.Fail($"Unable to analyze validator rules for {service.ServiceType}");
                }
            }
        }
    }
}
