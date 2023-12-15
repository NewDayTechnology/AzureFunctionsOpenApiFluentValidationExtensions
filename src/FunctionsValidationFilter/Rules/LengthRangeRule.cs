namespace FunctionsValidationFilter.Rules;

internal class LengthRangeRule : Rule
{
    public int Min { get; }
    public int Max { get; }

    public LengthRangeRule(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
