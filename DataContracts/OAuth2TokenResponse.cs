using Ducksoft.Soa.Common.Utilities;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which stores the rest OAuth2 token response related information
    /// </summary>
    [DataContract(Name = "OAuth2TokenResponse",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class OAuth2TokenResponse
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        [DataMember(Name = "expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets the issued date time.
        /// </summary>
        /// <value>
        /// The issued date time.
        /// </value>
        [DataMember(Name = "IssuedDateTime")]
        public DateTime IssuedDateTime { get; private set; }

        /// <summary>
        /// Gets the expiry date time.
        /// </summary>
        /// <value>
        /// The expiry date time.
        /// </value>
        public DateTime ExpiryDateTime { get { return (IssuedDateTime.AddSeconds(ExpiresIn)); } }

        /// <summary>
        /// Gets a value indicating whether this instance is token expired.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is token expired; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "IsTokenExpired")]
        public bool IsTokenExpired
        {
            get
            {
                var result = Utility.CompareDateTime(ExpiryDateTime, DateTime.Now);
                return ((IsError) || (!result.IsPast));
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [DataMember(Name = "error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        [DataMember(Name = "error_message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        [DataMember(Name = "StatusCode")]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "IsError")]
        public bool IsError { get { return (StatusCode != HttpStatusCode.OK); } }

        /// <summary>
        /// Gets or sets the raw response.
        /// </summary>
        /// <value>
        /// The raw response.
        /// </value>
        [DataMember(Name = "Raw")]
        public string Raw { get; set; }

        /// <summary>
        /// Gets the token request.
        /// </summary>
        /// <value>
        /// The token request.
        /// </value>
        [DataMember(Name = "TokenRequest")]
        public OAuth2TokenRequest TokenRequest { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2TokenResponse" /> class.
        /// </summary>
        public OAuth2TokenResponse()
        {
            IssuedDateTime = DateTime.Now;
        }
    }
}
