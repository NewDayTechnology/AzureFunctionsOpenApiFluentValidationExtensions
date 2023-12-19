namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class ExactLengthRule : Rule
{
    public int Length { get; }

    public ExactLengthRule(int length)
    {
        Length = length;
    }
}
