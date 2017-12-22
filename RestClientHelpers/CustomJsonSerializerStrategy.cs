using Ducksoft.Soa.Common.Utilities;
using RestSharp;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to configure given JSOn serialization strategy type
    /// </summary>
    public class CustomJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        /// <summary>
        /// Gets the type of the strategy.
        /// </summary>
        /// <value>
        /// The type of the strategy.
        /// </value>
        public JsonStrategyTypes StrategyType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJsonSerializerStrategy" /> class.
        /// </summary>
        /// <param name="strategyType">Type of the strategy.</param>
        public CustomJsonSerializerStrategy(JsonStrategyTypes strategyType)
        {
            StrategyType = strategyType;
        }

        /// <summary>
        /// Maps the name of the color member name to json field.
        /// </summary>
        /// <param name="clrPropertyName">Name of the color property.</param>
        /// <returns></returns>
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            ErrorBase.CheckArgIsNullOrDefault(clrPropertyName, nameof(clrPropertyName));
            var propertyName = string.Empty;
            var errMessage = string.Empty;
            switch (StrategyType)
            {
                case JsonStrategyTypes.None:
                    {
                        propertyName = base.MapClrMemberNameToJsonFieldName(clrPropertyName);
                    }
                    break;

                case JsonStrategyTypes.Pascal:
                    {
                        propertyName = clrPropertyName.ToTitleCase();
                    }
                    break;

                case JsonStrategyTypes.Camel:
                    {
                        propertyName = clrPropertyName.ToCamelCase();
                    }
                    break;

                case JsonStrategyTypes.Snake:
                    {
                        propertyName = clrPropertyName.ToSnakeCase();
                    }
                    break;

                default:
                    {
                        errMessage = $"The given {nameof(JsonStrategyTypes)}: \"{StrategyType}\"" +
                            " is not handled programatically!";
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(errMessage))
            {
                throw (new ExceptionBase(errMessage));
            }

            return (propertyName);
        }

    }
}
