namespace Ducksoft.SOA.Common.XmlHelpers
{
    /// <summary>
    /// Base class to parse xml with given renderer
    /// </summary>
    public abstract class XmlRendererBase
    {
        /// <summary>
        /// Gets the XML file path.
        /// </summary>
        /// <value>The XML file path.</value>
        public string XmlFilePath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRendererBase"/> class.
        /// </summary>
        /// <param name="xmlfilePath">The xmlfile path.</param>
        public XmlRendererBase(string xmlfilePath)
        {
            XmlFilePath = xmlfilePath;
        }

        /// <summary>
        /// Deserializes this instance.
        /// </summary>
        public abstract void Deserialize();
    }
}
