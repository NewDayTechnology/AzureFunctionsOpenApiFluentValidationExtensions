namespace NewDay.Extensions.FunctionsValidationFilter.Rules;

internal class NotEmptyRule : Rule
{
    public static NotEmptyRule Instance { get; } = new NotEmptyRule();

    private NotEmptyRule()
    {

    }
}
