namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class ExclusiveBetweenRule : Rule
{
    public int Min { get; }
    public int Max { get; }

    public ExclusiveBetweenRule(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
