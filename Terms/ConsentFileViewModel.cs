using Ducksoft.Soa.Common.Utilities;
using System;

namespace Ducksoft.Soa.Common.Terms
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsentFileViewModel
    {
        /// <summary>
        /// Gets the type of the consent.
        /// </summary>
        /// <value>
        /// The type of the consent.
        /// </value>
        public ConsentTermTypes ConsentType { get; private set; }

        /// <summary>
        /// Gets the type of the consent file.
        /// </summary>
        /// <value>
        /// The type of the consent file.
        /// </value>
        public string FileType { get; private set; }

        /// <summary>
        /// Gets the consent file full path.
        /// </summary>
        /// <value>
        /// The consent file full path.
        /// </value>
        public string FullPath { get; private set; }

        /// <summary>
        /// Gets the uploaded date.
        /// </summary>
        /// <value>
        /// The uploaded date.
        /// </value>
        public DateTime? UploadedDate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentFileViewModel" /> class.
        /// </summary>
        /// <param name="consentType">Type of the consent.</param>
        /// <param name="fileType">Type of the file.</param>
        public ConsentFileViewModel(ConsentTermTypes consentType, string fileType = "pdf")
        {
            ConsentType = consentType;
            FileType = fileType;

            FullPath = ConsentFileHelper.GetConsentFileFullPath(consentType, fileType);
            UploadedDate = Utility.GetFileName(FullPath).ExtractDateTime();
        }
    }
}
