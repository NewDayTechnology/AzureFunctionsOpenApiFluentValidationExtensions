using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.OpenApi.Models;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;
public class FunctionsValidationFilter : IDocumentFilter
{
    // TODO: threadsafe static cache 
    private readonly Dictionary<string, Dictionary<string, List<Rule>>> _schemaCollection;

    public FunctionsValidationFilter()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetLoadableTypes());
        var validators = GetValidators(types);

        var schemaMapper = new SchemaMapper(new RulesMapper(new ValidatorMapper()));

        _schemaCollection = validators.Select(x => schemaMapper.Map(x.Type, x.Rules)).ToDictionary(x => x.Name, x => x.Fields);
    }

    public void Apply(IHttpRequestDataObject request, OpenApiDocument document)
    {
        foreach (var item in _schemaCollection)
        {
            var schema = document.Components.Schemas[item.Key];
            foreach (var field in item.Value)
            {
                var prop = schema.Properties[field.Key];
                foreach (var property in field.Value)
                {
                    switch (property)
                    {
                        case Rules.NotEmptyRule:
                            schema.Required.Add(field.Key);
                            break;
                    }
                }
            }
        }
    }

    private static IEnumerable<(Type Type, IEnumerable<IValidationRule> Rules)> GetValidators(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            if (ValidatorInspector.TryGetRules(type, out var validatingType, out var rules))
            {
                yield return (validatingType, rules);
            }
        }
    }
}
