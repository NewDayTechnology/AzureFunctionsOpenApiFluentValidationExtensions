namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class MaxLengthRule : Rule
{
    public int Max { get; }

    public MaxLengthRule(int max)
    {
        Max = max;
    }
}
