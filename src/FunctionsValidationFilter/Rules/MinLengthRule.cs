namespace FunctionsValidationFilter.Rules;

internal class MinLengthRule : Rule
{
    public int Min { get; }

    public MinLengthRule(int min)
    {
        Min = min;
    }
}
