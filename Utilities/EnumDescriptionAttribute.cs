using System;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Attribute class for storing description related to C# Enum items.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the description binded to this attribute.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
