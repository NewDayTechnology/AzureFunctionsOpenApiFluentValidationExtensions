namespace NewDay.Extensions.FunctionsValidationFilter;

internal class StringHelper
{
    public static string CamelCase(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        if (str.Length > 1)
        {
            return char.ToLowerInvariant(str[0]) + str[1..];
        }
        return str.ToLowerInvariant();
    }
}
