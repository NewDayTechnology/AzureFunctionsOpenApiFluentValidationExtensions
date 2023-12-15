namespace FunctionsValidationFilter.Rules;

internal class GreaterThanOrEqualRule : Rule
{
    public int ValueToCompare { get; }

    public GreaterThanOrEqualRule(int valueToCompare)
    {
        ValueToCompare = valueToCompare;
    }
}
