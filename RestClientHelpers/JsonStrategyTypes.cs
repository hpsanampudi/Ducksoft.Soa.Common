using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.RestClientHelpers
{
    /// <summary>
    /// Enum which stores the type of Json serialize/deserialize rest sharp strategy related information
    /// </summary>
    public enum JsonStrategyTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = 0,
        /// <summary>
        /// The Pascal case
        /// </summary>
        [EnumDescription("Pascal")]
        Pascal,
        /// <summary>
        /// The camel case
        /// </summary>
        [EnumDescription("Camel")]
        Camel,
        /// <summary>
        /// The snake_case
        /// </summary>
        [EnumDescription("Snake")]
        Snake
    }
}
