using System;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    /// Class which is used to store filter changed event arguments. 
    /// </summary>
    public class FilterChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether this instance is reset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reset; otherwise, <c>false</c>.
        /// </value>
        public bool IsReset { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterChangedEventArgs" /> class.
        /// </summary>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        public FilterChangedEventArgs(bool isReset) : base()
        {
            IsReset = isReset;
        }
    }
}
