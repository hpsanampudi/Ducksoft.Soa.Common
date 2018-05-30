using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Helpers;
using Ducksoft.SOA.Common.Utilities;

namespace Ducksoft.SOA.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Ducksoft.SOA.Common.Terms.IConsentControlViewModel" />
    public class TwoStateRadioButtonViewModel : ConsentControlViewModelBase
    {
        /// <summary>
        /// Gets or sets the type of the option selection used by radio button.
        /// </summary>
        /// <value>
        /// The type of the option.
        /// </value>
        [MustSelect(true)]
        public ThreeStateOptionTypes OptionType { get; set; } = ThreeStateOptionTypes.None;

        /// <summary>
        /// Gets the option value.
        /// </summary>
        /// <value>
        /// The option value.
        /// </value>
        public override bool? IsSelected => OptionType.ToBool();

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoStateRadioButtonViewModel"/> class.
        /// </summary>
        public TwoStateRadioButtonViewModel()
        {
        }
    }
}
