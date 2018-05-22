using System;
using System.ComponentModel.DataAnnotations;

namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsentUserViewModel
    {
        /// <summary>
        /// Gets or sets the terms conditions agreed date.
        /// </summary>
        /// <value>
        /// The terms conditions agreed date.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy h:mm tt}")]
        public DateTime? TermsConditionsAgreedDate { get; set; }

        /// <summary>
        /// Gets or sets the data protection agreed date.
        /// </summary>
        /// <value>
        /// The data protection agreed date.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy h:mm tt}")]
        public DateTime? DataProtectionAgreedDate { get; set; }

        /// <summary>
        /// Gets or sets the is notify promotions by email.
        /// </summary>
        /// <value>
        /// The is notify promotions by email.
        /// </value>
        public bool? IsNotifyPromotionsByEmail { get; set; }

        /// <summary>
        /// Gets or sets the notify promotions by email consent date.
        /// </summary>
        /// <value>
        /// The notify promotions by email consent date.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy h:mm tt}")]
        public DateTime? PromotionsByEmailConsentDate { get; set; }

        /// <summary>
        /// Gets or sets the is notify promotions by text.
        /// </summary>
        /// <value>
        /// The is notify promotions by text.
        /// </value>
        public bool? IsNotifyPromotionsByText { get; set; }

        /// <summary>
        /// Gets or sets the promotions by text consent date.
        /// </summary>
        /// <value>
        /// The promotions by text consent date.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy h:mm tt}")]
        public DateTime? PromotionsByTextConsentDate { get; set; }
    }
}
