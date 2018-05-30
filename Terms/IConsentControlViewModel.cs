using System;

namespace Ducksoft.SOA.Common.Terms
{
    public interface IConsentControlViewModel
    {
        /// <summary>
        /// Gets the is selected.
        /// </summary>
        /// <value>
        /// The is selected.
        /// </value>
        bool? IsSelected { get; }

        /// <summary>
        /// Gets or sets the most recently accepted (or) rejected date.
        /// </summary>
        /// <value>
        /// The most recently accepted (or) rejected date.
        /// </value>        
        DateTime? ConsentDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>        
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is user consent required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is user consent required; otherwise, <c>false</c>.
        /// </value>
        bool IsUserConsentRequired { get; set; }
    }
}
