namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class LessThanRule : Rule
{
    public int ValueToCompare { get; }

    public LessThanRule(int valueToCompare)
    {
        ValueToCompare = valueToCompare;
    }
}
