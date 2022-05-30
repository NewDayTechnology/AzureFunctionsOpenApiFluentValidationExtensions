using System.Collections.ObjectModel;

namespace NewDay.Extensions.FunctionsValidationFilter
{
    /// <summary>
    /// Options to configure FunctionsValidationFilter.
    /// </summary>
    public class FunctionsValidationFilterOptions
    {
        /// <summary>
        /// OpenAPI operations with custom mappings.
        /// </summary>
        public OperationEntryCollection Operations { get; set; } = new();
    }

    /// <summary>
    /// A custom mapping for a OpenAPI operation.
    /// </summary>
    public class OperationEntryCollection : Collection<OperationEntry>
    {
        /// <summary>
        /// Define a schema to be used as source of the rules for the validation of the operation query string parameters.
        /// </summary>
        /// <param name="operationName">The OpenAPI operation.</param>
        /// <param name="schemaName">The model used to validate the parameters.</param>
        /// <exception cref="ArgumentNullException">The provided parameter is null.</exception>
        public void Add(string operationName, string schemaName)
        {
            if (operationName == null)
            {
                throw new ArgumentNullException(nameof(operationName));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            Add(new OperationEntry(operationName, schemaName));
        }

        /// <summary>
        /// Define a schema to be used as source of the rules for the validation of the operation query string parameters.
        /// </summary>
        /// <typeparam name="T">The type of the model.</typeparam>
        /// <param name="operationName">The OpenAPI operation.</param>
        /// <exception cref="ArgumentNullException">The provided parameter is null.</exception>
        public void Add<T>(string operationName)
        {
            Add(operationName, typeof(T).Name);
        }
    }

    /// <summary>
    /// A custom mapping for a OpenAPI operation. Instead of creating it directly, use the convenience
    /// <see cref="OperationEntryCollection.Add(string, string)"/> or <see cref="OperationEntryCollection.Add{T}(string)"/> methods.
    /// </summary>
    public class OperationEntry
    {
        internal OperationEntry(
            string operationName,
            string schemaName)
        {
            if (operationName == null)
            {
                throw new ArgumentNullException(nameof(operationName));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            OperationName = operationName;
            SchemaName = schemaName;
        }

        internal string OperationName { get; }

        internal string SchemaName { get; }
    }
}
