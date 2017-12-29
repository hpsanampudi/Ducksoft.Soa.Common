using Ducksoft.Soa.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Class which stores the rest OAuth2 token request related information
    /// </summary>
    [DataContract(Name = "OAuth2TokenRequest",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class OAuth2TokenRequest : IEqualityComparer<OAuth2TokenRequest>
    {
        /// <summary>
        /// Gets the token URL.
        /// </summary>
        /// <value>
        /// The token URL.
        /// </value>
        [DataMember]
        public string TokenUrl { get; set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        [DataMember]
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets the body parameters.
        /// </summary>
        /// <value>
        /// The body parameters.
        /// </value>
        [DataMember]
        public Dictionary<string, string> BodyParameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2TokenRequest"/> class.
        /// </summary>
        public OAuth2TokenRequest()
        {
            Headers = new Dictionary<string, string>();
            Headers.Add("cache-control", "no-cache");
            Headers.Add("accept", "application/json");
            Headers.Add("content-type", "application/x-www-form-urlencoded");

            BodyParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2TokenRequest" /> class.
        /// </summary>
        /// <param name="tokenUrl">The token URL.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="grantType">Type of the grant.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="redirectUrls">The redirect urls.</param>
        /// <param name="authCode">The  authorization code.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        public OAuth2TokenRequest(string tokenUrl, string clientId, string clientSecret,
            OAuth2TokenGrantTypes grantType = OAuth2TokenGrantTypes.ClientCredentials,
            string scope = "", List<string> redirectUrls = null, string authCode = "",
            string userName = "", string password = "") : this()
        {
            Func<string, string> GetValidStr =
                (source) => string.IsNullOrWhiteSpace(source) ? string.Empty : source.Trim();

            TokenUrl = GetValidStr(tokenUrl);
            clientId = GetValidStr(clientId);
            clientSecret = GetValidStr(clientSecret);
            scope = GetValidStr(scope);
            redirectUrls = redirectUrls ?? new List<string>();
            authCode = GetValidStr(authCode);
            userName = GetValidStr(userName);
            password = GetValidStr(password);

            BodyParameters.Add("client_id", clientId);
            BodyParameters.Add("client_secret", clientSecret);
            if (grantType != OAuth2TokenGrantTypes.None)
            {
                BodyParameters.Add("grant_type", grantType.GetEnumDescription());
            }

            var errMessage = string.Empty;
            switch (grantType)
            {
                case OAuth2TokenGrantTypes.AuthorizationCode:
                    {
                        BodyParameters.Add("code", authCode);
                        BodyParameters.Add("redirect_uri", string.Join(",", redirectUrls));
                        BodyParameters.Add("scope", scope);
                    }
                    break;

                case OAuth2TokenGrantTypes.ClientCredentials:
                    {
                        if (!string.IsNullOrWhiteSpace(scope))
                        {
                            BodyParameters.Add("scope", scope);
                        }
                    }
                    break;

                case OAuth2TokenGrantTypes.Password:
                    {
                        BodyParameters.Add("username", userName);
                        BodyParameters.Add("password", password);
                        BodyParameters.Add("scope", scope);
                    }
                    break;

                case OAuth2TokenGrantTypes.None:
                case OAuth2TokenGrantTypes.RefreshToken:
                case OAuth2TokenGrantTypes.Custom:
                default:
                    {
                        errMessage = $"The given grant type {grantType} is not handled!";
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(errMessage))
            {
                throw (new ExceptionBase(errMessage));
            }
        }

        #region Interface: IEqualityComparer implementation
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(OAuth2TokenRequest x, OAuth2TokenRequest y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            //Check whether the properties are equal.
            return (x.Equals(y));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(OAuth2TokenRequest obj)
        {
            //Check whether the object is null
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return (GetHashCode());
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var targetObject = obj as OAuth2TokenRequest;
            if (null == targetObject)
            {
                return (false);
            }

            var comparer = new DictionaryComparer<string, string>(true);
            var isEqual = (TokenUrl.IsEqualTo(targetObject.TokenUrl) &&
                comparer.Equals(Headers, targetObject.Headers) &&
                comparer.Equals(BodyParameters, targetObject.BodyParameters));

            return (isEqual);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (TokenUrl?.GetHashCode() ?? 0 ^
                Headers?.GetHashCode() ?? 0 ^
                BodyParameters?.GetHashCode() ?? 0);
        }
        #endregion
    }
}
