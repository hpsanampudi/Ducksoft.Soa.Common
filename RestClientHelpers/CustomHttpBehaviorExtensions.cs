using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel.Web;

namespace Ducksoft.SOA.Common.RestClientHelpers
{
    /// <summary>
    /// Class which is used to enable CustomWebHttpBehavior for an endpoint through configuration.
    /// Note: Since the CustomWebHttpBehavior is derived of the WebHttpBehavior we wanted to have 
    /// the exact same configuration. However during the coding we've realized that the 
    /// WebHttpElement is sealed so we've grabbed its code using reflector and modified it.
    /// </summary>
    /// <seealso cref="System.ServiceModel.Configuration.BehaviorExtensionElement" />
    public sealed class CustomHttpBehaviorExtensions : BehaviorExtensionElement
    {
        /// <summary>
        /// The web http properties
        /// </summary>
        private ConfigurationPropertyCollection webHttpProperties;

        /// <summary>
        /// Gets or sets a value that indicates whether help is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [help enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("helpEnabled")]
        public bool HelpEnabled
        {
            get
            {
                return ((bool)base["helpEnabled"]);
            }
            set
            {
                base["helpEnabled"] = value;
            }
        }

        /// <summary>
        /// Gets and sets the default message body style.
        /// </summary>
        /// <value>
        /// The default body style.
        /// </value>
        [ConfigurationProperty("defaultBodyStyle")]
        public WebMessageBodyStyle DefaultBodyStyle
        {
            get
            {
                return ((WebMessageBodyStyle)base["defaultBodyStyle"]);
            }
            set
            {
                base["defaultBodyStyle"] = value;
            }
        }

        /// <summary>
        /// Gets and sets the default outgoing response format.
        /// </summary>
        /// <value>
        /// The default outgoing response format.
        /// </value>
        [ConfigurationProperty("defaultOutgoingResponseFormat")]
        public WebMessageFormat DefaultOutgoingResponseFormat
        {
            get
            {
                return ((WebMessageFormat)base["defaultOutgoingResponseFormat"]);
            }
            set
            {
                base["defaultOutgoingResponseFormat"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the message format can be automatically selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if [automatic format selection enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("automaticFormatSelectionEnabled")]
        public bool AutomaticFormatSelectionEnabled
        {
            get
            {
                return ((bool)base["automaticFormatSelectionEnabled"]);
            }
            set
            {
                base["automaticFormatSelectionEnabled"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag that specifies whether a FaultException is generated when an internal server error (HTTP status code: 500) occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> if [fault exception enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("faultExceptionEnabled")]
        public bool FaultExceptionEnabled
        {
            get
            {
                return ((bool)base["faultExceptionEnabled"]);
            }
            set
            {
                base["faultExceptionEnabled"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (null == webHttpProperties)
                {
                    webHttpProperties = new ConfigurationPropertyCollection
                    {
                        new ConfigurationProperty("helpEnabled", typeof(bool), false, null, null,
                        ConfigurationPropertyOptions.None),

                        new ConfigurationProperty("defaultBodyStyle", typeof(WebMessageBodyStyle),
                        WebMessageBodyStyle.Bare, null, null, ConfigurationPropertyOptions.None),

                        new ConfigurationProperty("defaultOutgoingResponseFormat",
                        typeof(WebMessageFormat), WebMessageFormat.Xml, null, null,
                        ConfigurationPropertyOptions.None),

                        new ConfigurationProperty("automaticFormatSelectionEnabled", typeof(bool),
                        false, null, null, ConfigurationPropertyOptions.None),

                        new ConfigurationProperty("faultExceptionEnabled", typeof(bool), false,
                        null, null, ConfigurationPropertyOptions.None)
                    };
                }

                return (webHttpProperties);
            }
        }

        /// <summary>
        /// Gets the type of the behavior enabled by this configuration element.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return (typeof(CustomWebHttpBehavior));
            }
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior()
        {
            return (new CustomWebHttpBehavior
            {
                HelpEnabled = HelpEnabled,
                DefaultBodyStyle = DefaultBodyStyle,
                DefaultOutgoingResponseFormat = DefaultOutgoingResponseFormat,
                AutomaticFormatSelectionEnabled = AutomaticFormatSelectionEnabled,
                FaultExceptionEnabled = FaultExceptionEnabled
            });
        }
    }
}
