using Ducksoft.Soa.Common.Utilities;
using System;

namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>    
    public class SetupConsentTermsViewModel
    {
        /// <summary>
        /// Gets the consent terms and conditions.
        /// </summary>
        /// <value>
        /// The consent terms and conditions.
        /// </value>
        public ThreeStateCheckBoxViewModel ConsentTermsAndConditions { get; private set; }

        /// <summary>
        /// Gets the consent data protection notice.
        /// </summary>
        /// <value>
        /// The consent data protection notice.
        /// </value>
        public ThreeStateCheckBoxViewModel ConsentDataProtectionNotice { get; private set; }

        /// <summary>
        /// Gets the consent promotions by email.
        /// </summary>
        /// <value>
        /// The consent promotions by email.
        /// </value>
        public TwoStateRadioButtonViewModel ConsentPromotionsByEmail { get; private set; }

        /// <summary>
        /// Gets the consent promotions by text.
        /// </summary>
        /// <value>
        /// The consent promotions by text.
        /// </value>
        public TwoStateRadioButtonViewModel ConsentPromotionsByText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is popup dialog.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is popup dialog; otherwise, <c>false</c>.
        /// </value>
        public bool IsPopupDialog { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is allowed to save.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is allowed to save; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllowedToSave { get; private set; }

        /// <summary>
        /// Gets the consent user model.
        /// </summary>
        /// <value>
        /// The consent user model.
        /// </value>
        public ConsentUserViewModel ConsentUserModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupConsentTermsViewModel" /> class.
        /// </summary>
        /// <param name="consentUserModel">The consent user model.</param>
        /// <param name="isAllowedToSave">if set to <c>true</c> [is allowed to save].</param>
        /// <param name="isPopupDialog">if set to <c>true</c> [is popup dialog].</param>
        public SetupConsentTermsViewModel(ConsentUserViewModel consentUserModel,
            bool isAllowedToSave = true, bool isPopupDialog = true)
        {
            ConsentUserModel = consentUserModel;
            IsAllowedToSave = isAllowedToSave;
            IsPopupDialog = isPopupDialog;
            InitConsentTerms();
        }

        /// <summary>
        /// Initializes the consent terms.
        /// </summary>
        private void InitConsentTerms()
        {
            ConsentTermsAndConditions = GetConsentViewModel(ConsentTermTypes.TermsAndConditions)
                as ThreeStateCheckBoxViewModel;

            ConsentDataProtectionNotice = GetConsentViewModel(ConsentTermTypes.DataProtectionNotice)
                as ThreeStateCheckBoxViewModel;

            ConsentPromotionsByEmail = GetConsentViewModel(ConsentTermTypes.NotifyPromotionsByEmail)
                as TwoStateRadioButtonViewModel;

            ConsentPromotionsByText = GetConsentViewModel(ConsentTermTypes.NotifyPromotionsByText)
                as TwoStateRadioButtonViewModel;
        }

        /// <summary>
        /// Gets the consent view model.
        /// </summary>
        /// <param name="consentType">Type of the consent.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual IConsentControlViewModel GetConsentViewModel(ConsentTermTypes consentType)
        {
            ErrorBase.CheckArgIsNull(ConsentUserModel, nameof(ConsentUserModel));

            var viewModel = default(IConsentControlViewModel);
            var mrConsentDate = consentType.GetMostRecentConsentDate(ConsentUserModel);
            var isConsentRequired = consentType.IsAlertUserForConsent(mrConsentDate);
            switch (consentType)
            {
                case ConsentTermTypes.TermsAndConditions:
                case ConsentTermTypes.DataProtectionNotice:
                    {
                        var isChecked = (mrConsentDate != null) ? true : default(bool?);
                        var isEnabled = (IsAllowedToSave && !isChecked.GetValueOrDefault());
                        if (isConsentRequired)
                        {
                            isChecked = default(bool?);
                            mrConsentDate = null;
                            isEnabled = IsAllowedToSave;
                        }

                        viewModel = new ThreeStateCheckBoxViewModel
                        {
                            IsChecked = isChecked,
                            ConsentDate = mrConsentDate,
                            IsVisible = true,
                            IsEnabled = isEnabled,
                            IsUserConsentRequired = isConsentRequired,
                        };
                    }
                    break;

                case ConsentTermTypes.NotifyPromotionsByEmail:
                case ConsentTermTypes.NotifyPromotionsByText:
                    {
                        var isSelected = default(bool?);
                        if (consentType == ConsentTermTypes.NotifyPromotionsByEmail)
                        {
                            isSelected = ConsentUserModel?.IsNotifyPromotionsByEmail;
                        }
                        else if (consentType == ConsentTermTypes.NotifyPromotionsByText)
                        {
                            isSelected = ConsentUserModel?.IsNotifyPromotionsByText;
                        }

                        if (isConsentRequired)
                        {
                            isSelected = default(bool?);
                            mrConsentDate = null;
                        }

                        viewModel = new TwoStateRadioButtonViewModel
                        {
                            OptionType = isSelected.ToThreeStateOptionType(),
                            ConsentDate = mrConsentDate,
                            IsVisible = true,
                            IsEnabled = IsAllowedToSave,
                            IsUserConsentRequired = isConsentRequired,
                        };
                    }
                    break;

                default:
                    {
                        var errMessage = $"The given {consentType} is not handled!";
                        throw (new NotImplementedException(errMessage));
                    }
            }

            return (viewModel);
        }
    }
}
