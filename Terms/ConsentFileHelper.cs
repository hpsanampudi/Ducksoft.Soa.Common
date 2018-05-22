using Ducksoft.Soa.Common.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConsentFileHelper
    {
        /// <summary>
        /// Gets the root folder path.
        /// </summary>
        /// <value>
        /// The root folder path.
        /// </value>
        public static readonly string RootFolderPath = @"~/App_Data";

        /// <summary>
        /// Gets the consent file full path.
        /// </summary>
        /// <param name="consentType">Type of the consent.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static string GetConsentFileFullPath(ConsentTermTypes consentType, string fileType)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileType, nameof(fileType));
            var fileFullPath = string.Empty;
            var consentName = consentType.ToString();

            try
            {
                var consentServerPath = string.Empty;
                switch (consentType)
                {
                    case ConsentTermTypes.TermsAndConditions:
                    case ConsentTermTypes.DataProtectionNotice:
                        {
                            consentServerPath = $@"{RootFolderPath}/{consentName}";
                        }
                        break;

                    default:
                        {
                            var errMessage = $"The given {consentType} is not handled!";
                            throw (new NotImplementedException(errMessage));
                        }
                }

                if (string.IsNullOrWhiteSpace(consentServerPath))
                {
                    return (fileFullPath);
                }

                //Hp --> Assumption: Always single latest consent file will be kept under consent folder.
                //If we want to take backup of existing file while placing new/modified terms then 
                //create sub folder with name "_backup" and move the file into it.
                var consentDirPath = HostingEnvironment.MapPath(consentServerPath);
                fileFullPath = Directory
                    .EnumerateFiles(consentDirPath, $"*.{fileType}", SearchOption.TopDirectoryOnly)
                    .SingleOrDefault();
            }
            catch
            {
                fileFullPath = string.Empty;
            }

            return (fileFullPath);
        }

        /// <summary>
        /// Gets the most recent consent date.
        /// </summary>
        /// <param name="consentType">Type of the consent.</param>
        /// <param name="consentUserModel">The consent user model.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static DateTime? GetMostRecentConsentDate(this ConsentTermTypes consentType,
            ConsentUserViewModel consentUserModel)
        {
            ErrorBase.CheckArgIsNull(consentUserModel, nameof(consentUserModel));

            var lastConsentDate = default(DateTime?);
            switch (consentType)
            {
                case ConsentTermTypes.TermsAndConditions:
                    {
                        lastConsentDate = consentUserModel.TermsConditionsAgreedDate;
                    }
                    break;

                case ConsentTermTypes.DataProtectionNotice:
                    {
                        lastConsentDate = consentUserModel.DataProtectionAgreedDate;
                    }
                    break;

                case ConsentTermTypes.NotifyPromotionsByEmail:
                    {
                        lastConsentDate = consentUserModel.PromotionsByEmailConsentDate;
                    }
                    break;

                case ConsentTermTypes.NotifyPromotionsByText:
                    {
                        lastConsentDate = consentUserModel.PromotionsByTextConsentDate;
                    }
                    break;

                default:
                    {
                        var errMessage = $"The given {consentType} is not handled!";
                        throw (new NotImplementedException(errMessage));
                    }
            }

            return (lastConsentDate);
        }

        /// <summary>
        /// Determines whether [is alert user for consent] [the specified last consent date].
        /// </summary>
        /// <param name="consentType">Type of the consent.</param>
        /// <param name="lastConsentDate">The last (or) most recent consent date.</param>
        /// <returns>
        ///   <c>true</c> if [is alert user for consent] [the specified last consent date]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlertUserForConsent(this ConsentTermTypes consentType,
            DateTime? lastConsentDate)
        {
            var consentDate = lastConsentDate ?? default(DateTime);
            var consentFileInfo = new ConsentFileViewModel(consentType);
            var consentUploadDate = consentFileInfo.UploadedDate ?? default(DateTime);
            var result = consentDate.CompareDateTime(consentUploadDate);
            return (result.IsPast);
        }
    }
}
