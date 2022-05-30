using FluentValidation;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;

internal class SchemaMapper : ISchemaMapper
{
    private readonly IRulesMapper _ruleMapper;

    public SchemaMapper(IRulesMapper ruleMapper)
    {
        _ruleMapper = ruleMapper;
    }

    public (string Name, Dictionary<string, List<Rule>> Fields) Map(Type type, IEnumerable<IValidationRule> rules)
    {
        var schemaName = StringHelper.CamelCase(type.Name);

        var mappedRules = rules
            .Where(x => x.PropertyName is not null)
            .Select(_ruleMapper.Map)
            .GroupBy(x => x.Name)
            .ToDictionary(
                x => x.Key,
                x => x.SelectMany(z => z.Rules).ToList()
            );

        return (schemaName, mappedRules);
    }
}
