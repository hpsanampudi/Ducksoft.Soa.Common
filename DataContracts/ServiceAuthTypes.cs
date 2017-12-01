using Ducksoft.Soa.Common.Utilities;

namespace Ducksoft.Soa.Common.DataContracts
{
    /// <summary>
    /// Stores the type of service authorization types
    /// </summary>
    public enum ServiceAuthTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = 0,
        /// <summary>
        /// The basic authentication
        /// </summary>
        [EnumDescription("Basic")]
        Basic,
        /// <summary>
        /// The role authentication
        /// </summary>
        [EnumDescription("Role")]
        Role,
        /// <summary>
        /// The custom authentication
        /// </summary>
        [EnumDescription("Custom")]
        Custom,
        /// <summary>
        /// The OAuth2 token base authorization
        /// </summary>
        [EnumDescription("OAuth2")]
        OAuth2
    }

    /// <summary>
    /// Stores the type of OAuth2 token grant types
    /// </summary>
    public enum OAuth2TokenGrantTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The authorization code
        /// </summary>
        [EnumDescription("authorization_code")]
        AuthorizationCode,
        /// <summary>
        /// The client credentials
        /// </summary>
        [EnumDescription("client_credentials")]
        ClientCredentials,
        /// <summary>
        /// The password
        /// </summary>
        [EnumDescription("password")]
        Password,
        /// <summary>
        /// The referesh token
        /// </summary>
        [EnumDescription("refresh_token")]
        RefreshToken,
        /// <summary>
        /// The custom
        /// </summary>
        [EnumDescription("custom")]
        Custom
    }
}
