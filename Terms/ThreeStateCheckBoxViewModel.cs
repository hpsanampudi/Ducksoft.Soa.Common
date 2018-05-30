using Ducksoft.SOA.Common.Helpers;

namespace Ducksoft.SOA.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Ducksoft.SOA.Common.Terms.IConsentControlViewModel" />
    public class ThreeStateCheckBoxViewModel : ConsentControlViewModelBase
    {
        /// <summary>
        /// Gets or sets the is checked state of checkbox.
        /// </summary>
        /// <value>
        /// The is checked.
        /// </value>
        [MustAccept]
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Gets the option value.  
        /// </summary>
        /// <value>
        /// The option value.
        /// </value>
        public override bool? IsSelected => IsChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeStateCheckBoxViewModel"/> class.
        /// </summary>
        public ThreeStateCheckBoxViewModel()
        {
        }
    }
}
