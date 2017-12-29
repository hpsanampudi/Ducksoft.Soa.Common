using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Ducksoft.Soa.Common.Utilities
{
    /// <summary>
    /// Static class which is used to write error message to log file.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Stores the path of log file in the system %appdata% location.
        /// </summary>
        public static readonly string LogFilePath = Utility.GetAppLogFilePath("App_LogFilePath");

        /// <summary>
        /// Writes the message as information into log file.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteToLog(string message)
        {
            WriteToLog(message, TraceEventType.Information);
        }

        /// <summary>
        /// Writes the message with user supplied severity into log file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        public static void WriteToLog(string message, TraceEventType severity)
        {
            WriteMessageToLogFile(message, severity);
        }

        /// <summary>
        /// Writes the exception to log file.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void WriteToLog(this Exception exception) =>
            WriteExceptionToLogFile(exception);

        /// <summary>
        /// Writes the exception to log file.
        /// </summary>
        /// <param name="message">The exception.</param>
        private static void WriteExceptionToLogFile(Exception exception)
        {
            string formatMessage = string.Format(CultureInfo.CurrentUICulture, "{0}: [{1}] \t {2}",
                DateTime.Now.ToString(), TraceEventType.Error.ToString(), exception.Message);

            var logMessage = new StringBuilder();
            logMessage.Append(formatMessage);
            logMessage.Append(Environment.NewLine);
            logMessage.Append("*****Start: StackTrace*****");
            logMessage.Append(Environment.NewLine);
            logMessage.Append((null != exception.StackTrace) ?
                              exception.StackTrace : new StackTrace(true).ToString());

            logMessage.Append(Environment.NewLine);
            logMessage.Append("*****End: StackTrace*****");

            //  Write to Debug Output Window
            Debug.WriteLine(logMessage.ToString());

            //  Record to LogFile
            WriteToLogFile(logMessage.ToString());

            //Write to windows event log
            WriteToEventLog(logMessage.ToString());
        }

        /// <summary>
        /// Writes the message with user supplied severity in log file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        private static void WriteMessageToLogFile(string message, TraceEventType severity)
        {
            string formatMessage = string.Format(CultureInfo.CurrentUICulture, "{0}: [{1}] \t {2}",
                DateTime.Now.ToString(), severity.ToString(), message);

            //  Write to Debug Output Window
            Debug.WriteLine(formatMessage);

            //  Record to LogFile
            WriteToLogFile(formatMessage);
        }

        /// <summary>
        /// Writes user supplied message into a log file.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void WriteToLogFile(string message)
        {
            try
            {
                Utility.CreateDirectory(Utility.GetDirectoryPath(LogFilePath));
                //Note: Hp --> This text is always added, making the file longer over time if it is not deleted.
                using (var sw = File.AppendText(LogFilePath))
                {
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AppendLogMessage" + " " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Writes user supplied message into a windows event viewer.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void WriteToEventLog(string message)
        {
            try
            {
                // Write to Windows Event Log
                EventLog.WriteEntry(Utility.AppName, message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WriteToEventLog" + " " + ex.Message.ToString());
            }
        }
    }
}
