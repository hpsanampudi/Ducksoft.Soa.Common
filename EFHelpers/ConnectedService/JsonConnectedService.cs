namespace Ducksoft.Soa.Common.EFHelpers.ConnectedService
{
    /// <summary>
    /// Class which deserializes connected Odata service json file.
    /// </summary>
    /// <remarks>
    /// Auto generated code for ConnectedService.json using https://jsonutils.com/
    /// </remarks>
    public class JsonConnectedService
    {
        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public string ProviderId { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the getting started document.
        /// </summary>
        /// <value>
        /// The getting started document.
        /// </value>
        public GettingStartedDocument GettingStartedDocument { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        /// <value>
        /// The extended data.
        /// </value>
        public ExtendedData ExtendedData { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GettingStartedDocument
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EdmxVersion
    {
        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        /// <value>
        /// The major.
        /// </value>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        /// <value>
        /// The minor.
        /// </value>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        /// <value>
        /// The build.
        /// </value>
        public int Build { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>
        /// The revision.
        /// </value>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the major revision.
        /// </summary>
        /// <value>
        /// The major revision.
        /// </value>
        public int MajorRevision { get; set; }

        /// <summary>
        /// Gets or sets the minor revision.
        /// </summary>
        /// <value>
        /// The minor revision.
        /// </value>
        public int MinorRevision { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExtendedData
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the edmx version.
        /// </summary>
        /// <value>
        /// The edmx version.
        /// </value>
        public EdmxVersion EdmxVersion { get; set; }

        /// <summary>
        /// Gets or sets the generated file name prefix.
        /// </summary>
        /// <value>
        /// The generated file name prefix.
        /// </value>
        public string GeneratedFileNamePrefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use name space prefix].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use name space prefix]; otherwise, <c>false</c>.
        /// </value>
        public bool UseNameSpacePrefix { get; set; }

        /// <summary>
        /// Gets or sets the namespace prefix.
        /// </summary>
        /// <value>
        /// The namespace prefix.
        /// </value>
        public string NamespacePrefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use data service collection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use data service collection]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDataServiceCollection { get; set; }
    }
}
