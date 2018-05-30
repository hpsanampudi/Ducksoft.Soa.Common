using System;

namespace Ducksoft.SOA.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Ducksoft.SOA.Common.Terms.IConsentControlViewModel" />
    public abstract class ConsentControlViewModelBase : IConsentControlViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentControlViewModelBase"/> class.
        /// </summary>
        public ConsentControlViewModelBase()
        {
        }

        #region Interface: IConsentControlViewModel implementation
        /// <summary>
        /// Gets the option value.
        /// </summary>
        /// <value>
        /// The option value.
        /// </value>
        public abstract bool? IsSelected { get; }

        /// <summary>
        /// Gets or sets the most recently accepted (or) rejected date.
        /// </summary>
        /// <value>
        /// The most recently accepted (or) rejected date.
        /// </value>        
        public DateTime? ConsentDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>        
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is user consent required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is user consent required; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserConsentRequired { get; set; } = false;
        #endregion
    }
}
