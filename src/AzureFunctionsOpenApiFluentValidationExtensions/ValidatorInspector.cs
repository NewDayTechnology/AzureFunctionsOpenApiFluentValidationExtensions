using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace AzureFunctionsOpenApiFluentValidationExtensions;

internal static class ValidatorInspector
{
    [Obsolete]
    public static bool TryGetRules(Type type, [NotNullWhen(true)] out Type? validatingType, [NotNullWhen(true)] out IEnumerable<IValidationRule>? rules)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        if (!type.IsAbstract && !type.IsGenericType)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                {
                    validatingType = interfaceType.GenericTypeArguments[0];
                    try
                    {
                        rules = Activator.CreateInstance(type) as IEnumerable<IValidationRule>;
                        if (rules != null) return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }
            }
        }

        validatingType = null;
        rules = null;
        return false;
    }
}
