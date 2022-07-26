namespace NewDay.Extensions.FunctionsValidationFilter.Rules;

internal class NotNullRule : Rule
{
    public static NotNullRule Instance { get; } = new NotNullRule();

    private NotNullRule()
    {

    }
}
