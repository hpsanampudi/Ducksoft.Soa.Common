using System;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    /// Class which is used to store filter changed event arguments. 
    /// </summary>
    public class SortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether this instance is reset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reset; otherwise, <c>false</c>.
        /// </value>
        public bool IsRefresh { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortChangedEventArgs" /> class.
        /// </summary>
        /// <param name="isRefresh">if set to <c>true</c> [is refresh].</param>
        public SortChangedEventArgs(bool isRefresh) : base()
        {
            IsRefresh = isRefresh;
        }
    }
}
