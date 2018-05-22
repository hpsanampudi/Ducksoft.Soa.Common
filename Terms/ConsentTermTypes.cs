namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// Enum which stores the information related to consent terms type.
    /// </summary>
    public enum ConsentTermTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The terms and conditions
        /// </summary>
        TermsAndConditions,
        /// <summary>
        /// The data protection notice
        /// </summary>
        DataProtectionNotice,
        /// <summary>
        /// The notify promotions by email
        /// </summary>
        NotifyPromotionsByEmail,
        /// <summary>
        /// The notify promotions by text (SMS)
        /// </summary>
        NotifyPromotionsByText
    }
}
