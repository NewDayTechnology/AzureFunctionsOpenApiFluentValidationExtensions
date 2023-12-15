namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class ScalePrecisionRule : Rule
{
    public int Scale { get; }
    public int Precision { get; }

    public ScalePrecisionRule(int scale, int precision)
    {
        Scale = scale;
        Precision = precision;
    }
}
