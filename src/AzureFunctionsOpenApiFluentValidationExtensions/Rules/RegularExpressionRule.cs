﻿namespace AzureFunctionsOpenApiFluentValidationExtensions.Rules;

internal class RegularExpressionRule : Rule
{
    public string Regex { get; }

    public RegularExpressionRule(string regex)
    {
        Regex = regex;
    }
}
