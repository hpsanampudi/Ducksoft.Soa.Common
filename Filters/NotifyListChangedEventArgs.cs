using System;
using System.Collections.Generic;

namespace Ducksoft.SOA.Common.Filters
{
    /// <summary>
    /// Class which is used to store binding list changed notification event arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyListChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the type of the list changed.
        /// </summary>
        /// <value>
        /// The type of the list changed.
        /// </value>
        public ListChangedNotifyTypes ListChangedType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyListChangedEventArgs" /> class.
        /// </summary>
        public NotifyListChangedEventArgs() : base()
        {
            Items = new List<T>();
            ListChangedType = ListChangedNotifyTypes.None;
        }
    }
}
