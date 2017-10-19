using System;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Maps the given enum value to class object which is usefull in binding data to UI controls.
    /// </summary>
    public class EnumMapper
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public Enum Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumMapper" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EnumMapper(Enum value)
        {
            var description = Utility.GetEnumDescription(value, false);
            Description = string.IsNullOrWhiteSpace(description) ? value.ToString() : description;
            Value = value;
        }
    }
}
