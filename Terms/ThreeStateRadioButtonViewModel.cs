using Ducksoft.Soa.Common.Helpers;
using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Ducksoft.Soa.Common.Terms.IConsentControlViewModel" />
    public class ThreeStateRadioButtonViewModel : ConsentControlViewModelBase
    {
        /// <summary>
        /// Gets or sets the type of the option selection used by radio button.
        /// </summary>
        /// <value>
        /// The type of the option.
        /// </value>
        [MustSelect]
        public ThreeStateOptionTypes OptionType { get; set; } = ThreeStateOptionTypes.None;

        /// <summary>
        /// Gets the option value.
        /// </summary>
        /// <value>
        /// The option value.
        /// </value>
        public override bool? IsSelected => OptionType.ToBool();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeStateRadioButtonViewModel"/> class.
        /// </summary>
        public ThreeStateRadioButtonViewModel()
        {
        }
    }
}
