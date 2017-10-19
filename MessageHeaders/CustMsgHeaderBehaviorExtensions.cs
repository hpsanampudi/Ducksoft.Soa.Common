using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;

namespace Ducksoft.Soa.Common.MessageHeaders
{
    /// <summary>
    /// Class which is used to configure custom message header behavior once under WCF client config file.
    /// </summary>
    public class CustMsgHdrBehaviorExtensions : BehaviorExtensionElement
    {
        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior()
        {
            //Hp --> Logic: Store it in local variable as we are using in multiple places.
            var myCustomHeaders = CustomHeaders;
            return ((null != myCustomHeaders) ? new CustMsgHeaderBehaviorAttribute(
                myCustomHeaders.AllKeys.ToDictionary(key => key, key => myCustomHeaders[key].Value))
                : new CustMsgHeaderBehaviorAttribute());
        }

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <value>
        /// The type of the behavior.
        /// </value>
        public override Type BehaviorType
        {
            get { return (typeof(CustMsgHeaderBehaviorAttribute)); }
        }

        /// <summary>
        /// Gets the custom headers.
        /// </summary>
        /// <value>
        /// The custom headers.
        /// </value>
        [ConfigurationProperty("headers", IsRequired = true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection CustomHeaders
        {
            get
            {
                return (KeyValueConfigurationCollection)base["headers"];
            }
        }
    }
}
