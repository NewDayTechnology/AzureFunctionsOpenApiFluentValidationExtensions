using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                    Title = $"{DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle()}",
                    Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription(),
                    Contact = new OpenApiContact()
                    {
                        Name = "sample",
                        Email = "test@example.com",
                        Url = new Uri("https://github.com/NewDayTechnology/FunctionsValidationFilter"),
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Apache-2.0",
                        Url = new Uri("https://opensource.org/license/apache-2-0/"),
                    }
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = DefaultOpenApiConfigurationOptions.GetOpenApiVersion(),
                IncludeRequestingHostName = DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
                ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
                ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced(),
            };

            options.DocumentFilters.AddFunctionsValidationFilter<Program>();

            return options;
        });
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    })
    .Build();

host.Run();
