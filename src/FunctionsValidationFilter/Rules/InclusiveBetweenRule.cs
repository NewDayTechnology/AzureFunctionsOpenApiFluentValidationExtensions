namespace FunctionsValidationFilter.Rules;

internal class InclusiveBetweenRule : Rule
{
    public int Min { get; }
    public int Max { get; }

    public InclusiveBetweenRule(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
