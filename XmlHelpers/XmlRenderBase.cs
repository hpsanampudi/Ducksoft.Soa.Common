namespace Ducksoft.Soa.Common.XmlHelpers
{
    /// <summary>
    /// Base class to parse xml with given renderer
    /// </summary>
    public abstract class XmlRenderBase
    {
        /// <summary>
        /// Gets the XML file path.
        /// </summary>
        /// <value>The XML file path.</value>
        public string XmlFilePath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRenderBase"/> class.
        /// </summary>
        /// <param name="xmlfilePath">The xmlfile path.</param>
        public XmlRenderBase(string xmlfilePath)
        {
            XmlFilePath = xmlfilePath;
        }

        /// <summary>
        /// Deserializes this instance.
        /// </summary>
        public abstract void Deserialize();
    }
}
