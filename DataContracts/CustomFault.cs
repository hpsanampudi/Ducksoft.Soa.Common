using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Ducksoft.SOA.Common.DataContracts
{
    /// <summary>
    /// Class which is used to store user provided log message related information.
    /// </summary>
    [DataContract(Name = "CustomFault",
        Namespace = "http://ducksoftware.co.uk/SOA/WCF/DataContracts")]
    public class CustomFault
    {
        /// <summary>
        /// Gets or sets the ticket number.
        /// </summary>
        /// <value>
        /// The ticket number.
        /// </value>
        [DataMember]
        public Guid TicketNumber { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the type of the layer.
        /// </summary>
        /// <value>
        /// The type of the layer.
        /// </value>
        [DataMember]
        public AppLayerTypes LayerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is notify user.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is notify user; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsNotifyUser { get; set; }

        /// <summary>
        /// Gets or sets the type of the log.
        /// </summary>
        /// <value>
        /// The type of the log.
        /// </value>
        [DataMember]
        public LogMessageTypes LogType { get; set; }

        /// <summary>
        /// Gets or sets the name of the source call method.
        /// </summary>
        /// <value>
        /// The name of the source call method.
        /// </value>
        [DataMember]
        public string SourceMethodName { get; set; }

        /// <summary>
        /// Gets or sets the source file path.
        /// </summary>
        /// <value>
        /// The source file path.
        /// </value>
        [DataMember]
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Gets or sets the source line number.
        /// </summary>
        /// <value>
        /// The source line number.
        /// </value>
        [DataMember]
        public int SourceLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the help link.
        /// </summary>
        /// <value>
        /// The help link.
        /// </value>
        [DataMember]
        public string HelpLink { get; set; }

        /// <summary>
        /// Gets or sets the call stack details.
        /// </summary>
        /// <value>
        /// The call stack details.
        /// </value>
        [DataMember]
        public string CallStack { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFault"/> class.
        /// </summary>
        public CustomFault()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFault" /> class.
        /// </summary>
        /// <param name="customMessage">The custom message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="isNotifyUser">if set to <c>true</c> [is notify user].</param>
        /// <param name="srcMethodName">Name of the source method.</param>
        /// <param name="srcFilePath">The source file path.</param>
        /// <param name="srcLineNumber">The source line number.</param>
        public CustomFault(string customMessage = "", Exception exception = null,
            bool isNotifyUser = false, [CallerMemberName] string srcMethodName = "",
             [CallerFilePath] string srcFilePath = "", [CallerLineNumber] int srcLineNumber = 0)
        {
            var callerAssembly = Assembly.GetCallingAssembly();
            var errMessage = string.Join(Environment.NewLine,
                exception?.Messages() ?? new List<string>());

            var errorInfo = string.IsNullOrWhiteSpace(customMessage) ? errMessage :
                string.Join(Environment.NewLine, customMessage, errMessage);

            TicketNumber = Guid.NewGuid();
            Reason = errorInfo;
            UserName = string.Empty;
            AppName = callerAssembly.GetName().Name;
            LayerType = GetAppLayerType(callerAssembly);
            IsNotifyUser = isNotifyUser;
            SourceMethodName = srcMethodName;
            SourceFilePath = srcFilePath;
            SourceLineNumber = srcLineNumber;
            Message = errorInfo;
            HelpLink = string.Empty;
            CallStack = (null == exception) ? string.Empty :
                $"{Environment.NewLine}{exception.StackTrace}";
        }

        /// <summary>
        /// Gets the type of the application layer.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static AppLayerTypes GetAppLayerType(Assembly assembly)
        {
            var layerType = AppLayerTypes.None;
            var delimeter = new[] { "." };
            var result = assembly.GetName().Name.Split(delimeter,
                StringSplitOptions.RemoveEmptyEntries);

            Func<AppLayerTypes, bool> IsAppLayer = (appLayerType) =>
            result.Any(name => Utility.GetEnumDescription(appLayerType).IsEqualTo(name));

            if (IsAppLayer(AppLayerTypes.UI))
            {
                layerType = AppLayerTypes.UI;
            }
            else if (IsAppLayer(AppLayerTypes.Adaptor))
            {
                layerType = AppLayerTypes.Adaptor;
            }
            else if (IsAppLayer(AppLayerTypes.BL))
            {
                layerType = AppLayerTypes.BL;
            }
            else if (IsAppLayer(AppLayerTypes.DAL))
            {
                layerType = AppLayerTypes.DAL;
            }
            else if (IsAppLayer(AppLayerTypes.Common))
            {
                //TODO: Hp -->  Needs to handle if each layer has common component apart from genric?
                layerType = AppLayerTypes.Common;
            }

            return (layerType);
        }
    }
}
