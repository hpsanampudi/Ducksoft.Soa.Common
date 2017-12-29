using Ducksoft.Soa.Common.Utilities;
using System.Xml;

namespace Ducksoft.Soa.Common.XmlHelpers
{
    /// <summary>
    /// Abstract class for xml schema renderer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Ducksoft.Soa.Bl.JobSponsorRender.Xml.XmlRenderBase" />
    public abstract class XmlSchemaRender<T> : XmlRenderBase where T : class
    {
        /// <summary>
        /// Gets the XSD file path.
        /// </summary>
        /// <value>The XSD file path.</value>
        public string XsdFilePath { get; private set; }

        /// <summary>
        /// Gets the XML raw data.
        /// </summary>
        /// <value>The XML raw data.</value>
        public T XmlRawData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSchemaRender{T}"/> class.
        /// </summary>
        /// <param name="xmlfilePath">The xmlfile path.</param>
        /// <param name="xsdFilePath">The XSD file path.</param>
        public XmlSchemaRender(string xmlfilePath, string xsdFilePath) : base(xmlfilePath)
        {
            XsdFilePath = xsdFilePath;
        }

        /// <summary>
        /// Deserializes this instance.
        /// </summary>
        public override void Deserialize()
        {
            //Hp --> Logic: Deserialize the given xml file data to C# object.
            XmlRawData = DeserializeFromXml();
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <param name="xmlRdrSettings">The XML RDR settings.</param>
        /// <returns></returns>
        protected virtual T DeserializeFromXml(XmlReaderSettings xmlRdrSettings = null)
        {
            xmlRdrSettings = xmlRdrSettings ?? Utility.GetXmlReaderSettings(XsdFilePath);
            return (Utility.DeserializeFromXml<T>(XmlFilePath, xmlRdrSettings));
        }
    }
}
