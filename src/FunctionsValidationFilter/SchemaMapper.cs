using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using NewDay.Extensions.FunctionsValidationFilter.Rules;

namespace NewDay.Extensions.FunctionsValidationFilter;

internal class SchemaMapper
{
    private readonly IRulesMapper _ruleMapper;

    public SchemaMapper(IRulesMapper ruleMapper)
    {
        _ruleMapper = ruleMapper;
    }

    public (string Name, Dictionary<string, List<Rule>> Fields) Map(Type type, IEnumerable<IValidationRule> rules)
    {
        var schemaName = CamelCase(type.Name);

        // TODO: do we need GroupBy? (can it have duplicates?)
        var mappedRules = rules.Select(_ruleMapper.Map).ToDictionary(x => x.Name, x => x.Rules.ToList());

        return (schemaName, mappedRules);
    }

    private string CamelCase(string str)
    {
        if (str.Length > 1)
        {
            return char.ToLowerInvariant(str[0]) + str[1..];
        }
        return str.ToLowerInvariant();
    }

}
