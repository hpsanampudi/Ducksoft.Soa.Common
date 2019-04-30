using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.Filters;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Static class which is used to perform common functionality across the application.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Close button's code in Windows API
        /// </summary>
        private const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Disabled button status (enabled = false)
        /// </summary>
        private const int MF_GRAYED = 0x1;

        /// <summary>
        /// Event which is used to check whether multiple instances of this application is allowed (or) not.
        /// Note: When process exits or terminates, Windows will close this event, and destroy it if no open handles remain.
        /// </summary>
        private static EventWaitHandle appInstanceCheckEvent;

        /// <summary>
        /// Current UI Culture of the Main Thread 
        /// </summary>
        private static CultureInfo mainThreadUICulture = Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public static string GetFolderPath(string folderName)
        {
            ErrorBase.CheckArgIsNullOrDefault(folderName, nameof(folderName));
            return (GetCombinedPath(AppDirPath, folderName));
        }

        /// <summary>
        /// Gets the application log file path.
        /// </summary>
        /// <param name="appSettingsKey">The application settings key.</param>
        /// <returns></returns>
        public static string GetAppLogFilePath(string appSettingsKey)
        {
            string logFilePath = GetValueFromAppSettings(appSettingsKey);
            return (GetExpandedEnvVarPath(logFilePath));
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public static string AppName
        {
            get
            {
                return (GetFileNameWithoutExtension(AppFullPath));
            }
        }

        /// <summary>
        /// Gets the name of the application product.
        /// </summary>
        /// <value>
        /// The name of the application product.
        /// </value>
        public static string AppProductName
        {
            get
            {
                return (Application.ProductName);
            }
        }

        /// <summary>
        /// Gets the application product version.
        /// </summary>
        /// <value>
        /// The application product version.
        /// </value>
        public static string AppProductVersion
        {
            get
            {
                return (Application.ProductVersion);
            }
        }

        /// <summary>
        /// Gets the application build number.
        /// </summary>
        /// <value>
        /// The application build number.
        /// </value>
        public static string AppBuildNumber
        {
            get
            {
                return (Assembly.GetEntryAssembly().GetName().Version.Build
                    .ToString(CultureInfo.CurrentUICulture));
            }
        }

        /// <summary>
        /// Gets the application main form.
        /// </summary>
        /// <value>
        /// The application main form.
        /// </value>
        public static Form AppMainForm
        {
            get
            {
                return ((0 != Application.OpenForms.Count) ? Application.OpenForms[0] : null);
            }
        }

        /// <summary>
        /// Gets the application root directory path.
        /// </summary>
        /// <value>
        /// The application root directory path.
        /// </value>
        public static string AppRootDirectoryPath
        {
            get
            {
                return (Path.GetPathRoot(AppFullPath));
            }
        }

        /// <summary>
        /// Set the UI culture of the main thread 
        /// </summary>
        public static CultureInfo MainThreadUICulture
        {
            get
            {
                return mainThreadUICulture;
            }
            set
            {
                mainThreadUICulture = value;
            }
        }

        /// <summary>
        /// Shows the information message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parent">The parent.</param>
        public static DialogResult DisplayMessage(string message, IWin32Window parent)
        {
            return (DisplayMessage(message, AppName, parent));
        }

        /// <summary>
        /// Shows the information message box dialog with user provided caption.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="parent">The parent.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult DisplayMessage(string message, string caption,
            IWin32Window parent)
        {
            //ErrorBase.Require(null != parent); //Can be null
            ErrorBase.CheckArgIsNullOrDefault(message, () => message);
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(parent, message, caption, MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return (dlgResult);
        }

        /// <summary>
        /// Shows the AbortRetryIgnore confirmation message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult AskAbortRetryIgnoreConfirmation(string message)
        {
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(message, AppName, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Warning);

            return (dlgResult);
        }

        /// <summary>
        /// Shows the OKCancel confirmation message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static DialogResult AskOkCancelConfirmation(string message, IWin32Window parent)
        {
            return (AskOkCancelConfirmation(message, AppName, parent));
        }

        /// <summary>
        /// Shows the OKCancel confirmation message box dialog with user provided caption.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult AskOkCancelConfirmation(string message, string caption,
            IWin32Window parent)
        {
            //ErrorBase.Require(null != parent); //Can be null
            ErrorBase.CheckArgIsNullOrDefault(message, () => message);
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(parent, message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            // MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
            return (dlgResult);
        }

        /// <summary>
        /// Shows the YesNo confirmation message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static DialogResult AskYesNoConfirmation(string message, IWin32Window parent)
        {
            return (AskYesNoConfirmation(message, AppName, parent));
        }

        /// <summary>
        /// Shows the YesNo confirmation message box dialog with user provided caption.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult AskYesNoConfirmation(string message, string caption,
            IWin32Window parent)
        {
            //ErrorBase.Require(null != parent); //Can be null
            ErrorBase.CheckArgIsNullOrDefault(message, () => message);
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(parent,
                message,
                caption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            //MessageBoxDefaultButton.Button1,
            //MessageBoxOptions.DefaultDesktopOnly);
            return (dlgResult);
        }

        /// <summary>
        /// Shows the error message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static DialogResult ShowErrorDialog(string message, IWin32Window parent)
        {
            return (ShowErrorDialog(message, AppName, parent));
        }

        /// <summary>
        /// Shows the error message box dialog with user provided caption.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult ShowErrorDialog(string message, string caption,
            IWin32Window parent)
        {
            //ErrorBase.Require(null != parent); //Can be null
            ErrorBase.CheckArgIsNullOrDefault(message, () => message);
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(parent, message, caption, MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            //  MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
            return (dlgResult);
        }

        /// <summary>
        /// Shows the fatal error message box dialog with default caption as application name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public static DialogResult ShowFatalErrorDialog(string message, IWin32Window parent)
        {
            return (ShowFatalErrorDialog(message, (AppName + " : Fatal Error"), parent));
        }

        /// <summary>
        /// Shows the fatal error message box dialog with user provided caption.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static DialogResult ShowFatalErrorDialog(string message, string caption,
            IWin32Window parent)
        {
            //ErrorBase.Require(null != parent); //Can be null
            ErrorBase.CheckArgIsNullOrDefault(message, () => message);
            DialogResult dlgResult = DialogResult.None;
            dlgResult = MessageBox.Show(parent, message, caption,
                MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);

            //  MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
            return (dlgResult);
        }

        /// <summary>
        /// Displays popup balloon message
        /// if show balloon is false, Suppress the popup balloon message to user
        /// if show balloon is true, Displays the popup balloon message to user
        /// </summary>
        /// <param name="carlToShow">The control on which to show popup balloon message.</param>
        /// <param name="toolTipLocation">The tool tip location.</param>
        /// <param name="popupText">The popup text.</param>
        /// <param name="popupDelay">The popup delay.</param>
        /// <param name="alertType">Type of the alert.</param>
        /// <param name="showBalloon">if set to <c>true</c> [show balloon].</param>
        public static void ShowPopupMessage(Control ctrlToShow, Point toolTipLocation,
            string popupText, int popupDelay = 3000, ToolTipIcon alertType = ToolTipIcon.Info,
            bool showBalloon = true)
        {
            ErrorBase.CheckArgIsNull(ctrlToShow, () => ctrlToShow);
            ErrorBase.CheckArgIsNull(toolTipLocation, () => toolTipLocation);

            if (showBalloon)
            {
                // Create ToolTip Object, to show balloon message
                ToolTip popupHelp = new ToolTip();
                popupHelp.RemoveAll();
                popupHelp.IsBalloon = true;
                popupHelp.SetToolTip(ctrlToShow, string.Empty);
                popupHelp.ToolTipIcon = alertType;
                popupHelp.AutoPopDelay = popupDelay;

                // popupHelp.Show is called twice, to show balloon message on the editing text box.
                // else the popup message will be shown at different place in the UI.
                //This is a bug with Microsoft.
                popupHelp.Active = false;
                popupHelp.Show(popupText, ctrlToShow, toolTipLocation, popupDelay);

                popupHelp.Active = true;
                popupHelp.Show(popupText, ctrlToShow, toolTipLocation, popupDelay);
            }
        }

        /// <summary>
        /// Determines whether [is user accepted admin privileges].
        /// </summary>
        /// <param name="isCmdLineMode">if set to <c>true</c> [is CMD line mode].</param>
        /// <returns>
        ///   <c>true</c> if [is user accepted admin privileges]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUserAcceptedAdminPrivileges(bool isCmdLineMode)
        {
            bool isAcceptedAdminPrivileges = false;
            // Users with Administrator privilege can only run the application
            WindowsIdentity userIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal winPrincipalIdentity = new WindowsPrincipal(userIdentity);

            isAcceptedAdminPrivileges = winPrincipalIdentity.IsInRole(
                WindowsBuiltInRole.Administrator);

            if (!isAcceptedAdminPrivileges)
            {
                //Display the message in console if application runs on command line
                if (isCmdLineMode)
                {
                    Console.WriteLine(
                        "You need Administrator Privileges on this system to use this application.");

                    //TODO: Hp --> Needs to remove below line when we make the setup application as hybrid.
                    //Note: Hp --> In first release we are only supporting cmdline args to launch this application UI taking package scope path.
                    //So, there is no need to make this application hybrid (i.e., to support both UI and console.)
                    ShowErrorDialog(
                        "You need Administrator Privileges on this system to use this application.",
                        null);
                }
                else
                {
                    ShowErrorDialog(
                        "You need Administrator Privileges on this system to use this application.",
                        null);
                }
            }

            return (isAcceptedAdminPrivileges);
        }

        /// <summary>
        /// Determines whether [is app first instance].
        /// </summary>
        /// <param name="isCmdLineMode">if set to <c>true</c> [is CMD line mode].</param>
        /// <returns>
        ///   <c>true</c> if [is app first instance]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAppFirstInstance(bool isCmdLineMode)
        {
            bool isAppFirstInstance = true;

            // Test to see if application instance is already running.
            appInstanceCheckEvent = new EventWaitHandle(false, EventResetMode.ManualReset,
                                        Environment.UserName, out isAppFirstInstance);

            if (appInstanceCheckEvent == null) { }

            if (!isAppFirstInstance)
            {
                StringBuilder errMessage = new StringBuilder();
                errMessage.Append("An instance of the application is already running!");
                errMessage.Append(Environment.NewLine);
                errMessage.Append("Only one instance is allowed.");
                errMessage.Append(Environment.NewLine);
                errMessage.Append("Please close the running application if required.");

                //Display the message in console if application runs on command line
                if (isCmdLineMode)
                {
                    Console.WriteLine(errMessage.ToString());
                    //TODO: Hp --> Needs to remove below line when we make the setup application as hybrid.
                    //Note: Hp --> In first release we are only supporting cmdline args to launch this application UI taking package scope path.
                    //So, there is no need to make this application hybrid (i.e., to support both UI and console.)
                    ShowErrorDialog(errMessage.ToString(), null);
                }
                else
                {
                    ShowErrorDialog(errMessage.ToString(), null);
                }
            }

            return (isAppFirstInstance);
        }

        /// <summary>
        /// Returns the Application root folder path
        /// </summary>
        /// <returns></returns>
        /// Application needs Forms class so adding method here
        public static string GetAppRootPath()
        {
            return Directory.GetParent(Path.GetFullPath(Application.ExecutablePath)).FullName;
        }

        /// <summary>
        /// Writes to HKCU run once registry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void WriteToHKCURunOnceRegistry(string name, string value)
        {
            ErrorBase.CheckArgIsNullOrDefault(name, () => name);
            ErrorBase.CheckArgIsNullOrDefault(value, () => value);

            var runOnceHKCUKey = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);

            runOnceHKCUKey.SetValue(name, value);
        }

        /// <summary>
        /// Enables (or) disables the winform close button.
        /// </summary>
        /// <param name="frmHdl">The FRM HDL.</param>
        /// <param name="isEnable">if set to <c>true</c> [is enable].</param>
        public static void EnableWinCloseButton(IntPtr frmHdl, bool isEnable)
        {
            NativeApiHelper.EnableMenuItem(NativeApiHelper.GetSystemMenu(frmHdl, isEnable),
                SC_CLOSE, MF_GRAYED);
        }

        /// <summary>
        /// Check if application instance is opened/runing during installation
        /// </summary>
        /// <param name="appNameWithoutExt">The application name without extension.</param>
        /// <param name="appExeFullPath">The application executable full path.</param>
        /// <returns></returns>
        public static bool CheckIfAppInstanceRunning(string appNameWithoutExt, string appExeFullPath)
        {
            try
            {
                var process = Process.GetProcessesByName(appNameWithoutExt);
                return (process.ToList().FindAll(processObj =>
                    !string.IsNullOrEmpty(processObj.MainModule.FileName) &&
                    processObj.MainModule.FileName.IsEqualTo(appExeFullPath)).Count > 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Display Message to user if application instance is runing
        /// </summary>
        /// <param name="message">message to be displayed to user</param>
        /// <param name="isUserAbortedCurrenInstallation">Check if user aborted current installation</param>
        /// <returns>true/false if user aborted the installation or not</returns>
        /// Abort: abort the installation
        /// Ignore : continue installation even if application instance is runing
        /// Retry : Check again if user closed the runing application instance
        public static bool ShowMsgIfCBPInstanceRuning(string appNameWithoutExt,
            string appExeFullPath, string message, out bool isUserAbortedCurrenInstallation)
        {
            if (CheckIfAppInstanceRunning(appNameWithoutExt, appExeFullPath))
            {
                var dlgResult = AskAbortRetryIgnoreConfirmation(message);

                // for retry,recursive loop
                return (isUserAbortedCurrenInstallation = (dlgResult == DialogResult.Abort) ?
                    true : ((dlgResult == DialogResult.Ignore) ? false :
                ShowMsgIfCBPInstanceRuning(appNameWithoutExt, appExeFullPath, message,
                out isUserAbortedCurrenInstallation)));
            }
            else
            {
                return isUserAbortedCurrenInstallation = false;
            }
        }

        /// <summary>
        /// Bring the runing executable process to front,
        /// throws error when process is null but not in case 
        /// when process is already exited
        /// </summary>
        /// <param name="exeProcess">runing process</param>
        public static void BringRunningAppToFront(Process exeProcess)
        {
            ErrorBase.CheckArgIsNull(exeProcess, () => exeProcess);
            IntPtr handle = exeProcess.MainWindowHandle;
            if (NativeApiHelper.IsIconic(handle))
            {
                NativeApiHelper.ShowWindow(handle, NativeApiHelper.SW_RESTORE);
            }

            NativeApiHelper.SetForegroundWindow(handle);
        }

        /// <summary>
        /// Gets the decade date regular expression in yyyymmdd format based on current date.
        /// </summary>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns></returns>
        public static string GetDecadeDateRegExpression(string delimeter = "")
        {
            int startYear = DateTime.Now.AddYears(-100).Year;
            int endYear = DateTime.Now.AddYears(100).Year;

            int subStartYear = Convert.ToInt16(startYear.ToString().Substring(0, 2));
            int subEndYear = Convert.ToInt16(endYear.ToString().Substring(0, 2));

            int decadeStartYear = Convert.ToInt16(startYear.ToString().Substring(0, 2) + "00");
            int decadeEndYear = Convert.ToInt16((endYear + 100).ToString().Substring(0, 2) + "00");

            var regxStartYear = string.Join("|",
                Enumerable.Range(subStartYear, ((subEndYear - subStartYear) + 1)));

            //Hp --> Logic: Validation for date format in yyyymmdd
            StringBuilder regxDateBuilder = new StringBuilder();

            //Validation for 31 days a year (Jan,Mar,May,Jul,Oct,Dec)
            regxDateBuilder.Append(string.Join(delimeter,
                $"(((({regxStartYear})[0-9]{{2}})",
                "(0[13578]|1[02])",
                $"(0[1-9]|[12][0-9]|3[01]))"));

            regxDateBuilder.Append("|");

            //Validation for 30 days a year (Apr,Jun,Sep,Nov)
            regxDateBuilder.Append(string.Join(delimeter,
                $"((({regxStartYear})[0-9]{{2}})",
                "(0[469]|11)",
                "(0[1-9]|[12][0-9]|30))"));

            regxDateBuilder.Append("|");

            //Validation for 28 days a year (Feb)
            regxDateBuilder.Append(string.Join(delimeter,
                $"((({regxStartYear})[0-9]{{2}})",
                "(02)",
                "(0[1-9]|1[0-9]|2[0-8]))"));

            regxDateBuilder.Append("|");

            //Rules for validation of Leap year
            //1. Any year that can be evenly divided by 4 (such as 2012, 2016, etc)
            //2. If it can be evenly divided by 400, then it is (such as 2000, 2400)
            //3. If it can can be evenly divided by 100, then it isn't (such as 2100, 2200, 2300, 2500, 2600, etc)
            var misLeapYears = string.Join("|",
                Enumerable.Range(decadeStartYear, ((decadeEndYear - decadeStartYear) + 1))
                .Where(Y => ((DateTime.IsLeapYear(Y)) && (0 == (Y % 400)))));

            var regxMisLeapYears = string.IsNullOrEmpty(misLeapYears) ?
                string.Empty : string.Concat("|", misLeapYears);

            regxDateBuilder.Append(string.Join(delimeter,
                $"(((({regxStartYear})(04|08|[2468][048]|[13579][26])){regxMisLeapYears})",
                "(02)",
                "(29)))"));

            return (regxDateBuilder.ToString());
        }

        /// <summary>
        /// Determines whether [is in date time range] [the specified SRC value].
        /// </summary>
        /// <param name="srcValue">The SRC value.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="isIgnoreMilliSeconds">if set to <c>true</c> [is ignore milli seconds].
        /// </param>
        public static dynamic IsInDateTimeRange(this DateTime srcValue, DateTime minValue,
            DateTime maxValue, bool isIgnoreMilliSeconds = false)
        {
            ErrorBase.CheckArgIsNull(srcValue, () => srcValue);
            ErrorBase.CheckArgIsNull(minValue, () => minValue);
            ErrorBase.CheckArgIsNull(maxValue, () => maxValue);

            dynamic expando = new ExpandoObject();
            expando.IsOutOfMinValue = (0 > srcValue.CompareTo(minValue));

            expando.IsOutOfMaxValue = (0 > maxValue.CompareTo(srcValue));

            expando.IsEqualToMinValue = (!isIgnoreMilliSeconds) ?
                (0 == srcValue.CompareTo(minValue)) :
                (1 > Math.Abs((srcValue - minValue).TotalSeconds));

            expando.IsEqualToMaxValue = (!isIgnoreMilliSeconds) ?
                (0 == maxValue.CompareTo(srcValue)) :
                (1 > Math.Abs((maxValue - srcValue).TotalSeconds));

            expando.IsWithInRange = (((!expando.IsOutOfMinValue) && (!expando.IsOutOfMaxValue)) ||
                (expando.IsEqualToMinValue) || (expando.IsEqualToMaxValue));

            return (expando);
        }

        /// <summary>
        /// Compares the date time.
        /// IsPast: If source value is less than target value; 
        /// IsFuture: If source value is greater than target value;
        /// IsCurrent: If source value is equal to target value depending upon ignore milli secs;
        /// </summary>
        /// <param name="source">The SRC value.</param>
        /// <param name="target">The TRGT value.</param>
        /// <param name="isIgnoreMilliSeconds">if set to <c>true</c> [is ignore milli seconds].
        /// </param>
        /// <returns>
        /// IsPast: If source value is less than target value; 
        /// IsFuture: If source value is greater than target value;
        /// IsCurrent: If source value is equal to target value depending upon ignore milli secs;
        /// </returns>
        public static dynamic CompareDateTime(this DateTime source, DateTime target,
            bool isIgnoreMilliSeconds = false)
        {
            ErrorBase.CheckArgIsNull(source, () => source);
            ErrorBase.CheckArgIsNull(target, () => target);

            var result = source.CompareTo(target);
            dynamic expando = new ExpandoObject();
            expando.IsPast = (0 > result);
            expando.IsFuture = (0 < result);
            expando.IsCurrent = (!isIgnoreMilliSeconds) ? (0 == result) :
                (1 > Math.Abs((source - target).TotalSeconds));

            return (expando);
        }

        /// <summary>
        /// Extracts the date time in format yyyyMMddHHmmss from given string
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static DateTime? ExtractDateTime(this string source)
        {
            ErrorBase.CheckArgIsNullOrDefault(source, nameof(source));
            var uploadDate = default(DateTime?);
            var patternBuilder = new StringBuilder();
            patternBuilder.Append("^(?:.*)?");
            patternBuilder.Append($"(?<YearMonthDate>{GetDecadeDateRegExpression()})");
            patternBuilder.Append("(?<Hours>[01][0-9]|2[0-3])");
            patternBuilder.Append("(?<Minutes>[0-5][0-9])");
            patternBuilder.Append("(?<Seconds>[0-5][0-9])?");
            patternBuilder.Append(@"(?:\..*)?$");

            var pattern = patternBuilder.ToString();
            var result = Regex.Match(source, pattern, RegexOptions.IgnoreCase);
            if (result.Success)
            {
                var secsValue = result.Groups["Seconds"].Value;
                var seconds = string.IsNullOrWhiteSpace(secsValue) ? "00" : secsValue;

                var dateTimeStr = string.Join(string.Empty, result.Groups["YearMonthDate"],
                    result.Groups["Hours"], result.Groups["Minutes"], seconds);

                uploadDate = ConvertToDateTime(dateTimeStr, dateTimeFormats: "yyyyMMddHHmmss");
            }

            return (uploadDate);
        }

        /// <summary>
        /// Extracts the enu date time (i.e., dd MMM yyyy (or) dd MMMM yyyy).
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns></returns>
        public static DateTime? ExtractEnuDateTime(this string source, string delimeter = " ")
        {
            ErrorBase.CheckArgIsNullOrDefault(source, nameof(source));
            var uploadDate = default(DateTime?);
            var patternBuilder = new StringBuilder();
            patternBuilder.Append(@"(?<Date>\d+)");
            patternBuilder.Append($"(?:{delimeter})");
            patternBuilder.Append(@"(?<Month>Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?" +
                "|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?" +
                "|Nov(?:ember)?|Dec(?:ember)?)");

            patternBuilder.Append($"(?:{delimeter})");
            patternBuilder.Append(@"(?<Year>\d+)");
            patternBuilder.Append(@"(?: )");
            patternBuilder.Append("((?<Hours>[01][0-9]|2[0-3])");
            patternBuilder.Append(":(?<Minutes>[0-5][0-9])");
            patternBuilder.Append("(:(?<Seconds>[0-5][0-9]))?");
            patternBuilder.Append("( (?<AMPM>AM|PM) )?)?");

            var pattern = patternBuilder.ToString();
            var result = Regex.Match(source, pattern, RegexOptions.IgnoreCase);
            if (result.Success)
            {
                var dateStr = string.Join(delimeter, result.Groups["Date"],
                    result.Groups["Month"], result.Groups["Year"]);

                var timeStr = string.Empty;
                var hrsValue = result.Groups["Hours"].Value;
                var minValue = result.Groups["Minutes"].Value;
                if ((!string.IsNullOrWhiteSpace(hrsValue)) && (!string.IsNullOrWhiteSpace(minValue)))
                {
                    var secsValue = result.Groups["Seconds"].Value;
                    var seconds = string.IsNullOrWhiteSpace(secsValue) ? "00" : secsValue;

                    var amOrpmValue = result.Groups["AMPM"].Value;
                    var amOrpm = string.IsNullOrWhiteSpace(amOrpmValue) ? string.Empty : secsValue;

                    timeStr = string.Join(":", hrsValue, minValue, seconds, amOrpm).TrimEnd(':');
                }

                uploadDate = ConvertToDateTime($"{dateStr} {timeStr}".Trim(), delimeter);
            }

            return (uploadDate);
        }

        /// <summary>
        /// Converts to date time.
        /// </summary>
        /// <param name="source">The source date time as string.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <param name="isDisplayError">if set to <c>true</c> [is display error].</param>
        /// <param name="dateTimeFormats">The date time formats, if not provided then date Month Year.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static DateTime? ConvertToDateTime(string source, string delimeter = "/",
            bool isDisplayError = false, params string[] dateTimeFormats)
        {
            var output = default(DateTime?);
            var formats = dateTimeFormats?.ToList() ?? new List<string>();
            if (!formats.Any())
            {
                formats.AddRange(new List<string>
                {
                    $"dd{delimeter}MM{delimeter}yyyy HH:mm:ss",
                    $"dd{delimeter}MM{delimeter}yyyy H:mm:ss",
                    $"dd{delimeter}MM{delimeter}yyyy hh:mm:ss tt",
                    $"dd{delimeter}MM{delimeter}yyyy h:mm:ss tt",
                    $"dd{delimeter}MM{delimeter}yyyy",

                    $"dd{delimeter}M{delimeter}yyyy HH:mm:ss",
                    $"dd{delimeter}M{delimeter}yyyy H:mm:ss",
                    $"dd{delimeter}M{delimeter}yyyy hh:mm:ss tt",
                    $"dd{delimeter}M{delimeter}yyyy h:mm:ss tt",
                    $"dd{delimeter}M{delimeter}yyyy",

                    $"d{delimeter}MM{delimeter}yyyy HH:mm:ss",
                    $"d{delimeter}MM{delimeter}yyyy H:mm:ss",
                    $"d{delimeter}MM{delimeter}yyyy hh:mm:ss tt",
                    $"d{delimeter}MM{delimeter}yyyy h:mm:ss tt",
                    $"d{delimeter}MM{delimeter}yyyy",

                    $"d{delimeter}M{delimeter}yyyy HH:mm:ss",
                    $"d{delimeter}M{delimeter}yyyy H:mm:ss",
                    $"d{delimeter}M{delimeter}yyyy hh:mm:ss tt",
                    $"d{delimeter}M{delimeter}yyyy h:mm:ss tt",
                    $"d{delimeter}M{delimeter}yyyy",

                    $"dd{delimeter}MMM{delimeter}yyyy HH:mm:ss",
                    $"dd{delimeter}MMM{delimeter}yyyy H:mm:ss",
                    $"dd{delimeter}MMM{delimeter}yyyy hh:mm:ss tt",
                    $"dd{delimeter}MMM{delimeter}yyyy h:mm:ss tt",
                    $"dd{delimeter}MMM{delimeter}yyyy",

                    $"d{delimeter}MMM{delimeter}yyyy HH:mm:ss",
                    $"d{delimeter}MMM{delimeter}yyyy H:mm:ss",
                    $"d{delimeter}MMM{delimeter}yyyy hh:mm:ss tt",
                    $"d{delimeter}MMM{delimeter}yyyy h:mm:ss tt",
                    $"d{delimeter}MMM{delimeter}yyyy",

                    $"dd{delimeter}MMMM{delimeter}yyyy HH:mm:ss",
                    $"dd{delimeter}MMMM{delimeter}yyyy H:mm:ss",
                    $"dd{delimeter}MMMM{delimeter}yyyy hh:mm:ss tt",
                    $"dd{delimeter}MMMM{delimeter}yyyy h:mm:ss tt",
                    $"dd{delimeter}MMMM{delimeter}yyyy",

                    $"d{delimeter}MMMM{delimeter}yyyy HH:mm:ss",
                    $"d{delimeter}MMMM{delimeter}yyyy H:mm:ss",
                    $"d{delimeter}MMMM{delimeter}yyyy hh:mm:ss tt",
                    $"d{delimeter}MMMM{delimeter}yyyy h:mm:ss tt",
                    $"d{delimeter}MMMM{delimeter}yyyy",
                });
            }

            DateTime result;
            if (DateTime.TryParseExact(source, formats.ToArray(), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
            {
                output = result;
            }
            else if (isDisplayError)
            {
                var errMessage = $"Failed to convert string to date time {source}";
                throw (new InvalidCastException(errMessage));
            }

            return (output);
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TTarget To<TTarget>(this object source) =>
            (TTarget)Convert.ChangeType(source, typeof(TTarget));

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="defValue">The definition value.</param>
        /// <returns></returns>
        public static TTarget ConvertTo<TTarget>(this object source,
            TTarget defValue = default(TTarget))
        {
            var result = defValue;
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(TTarget));
                if (converter.CanConvertFrom(source.GetType()))
                {
                    result = (TTarget)converter.ConvertFrom(source);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to convert given source to target value");
            }

            return (result);
        }

        /// <summary>
        /// To the decimal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The definition value.</param>
        /// <returns></returns>
        public static decimal ToDecimal<T>(this T value, decimal defValue = default(decimal))
        {
            var result = defValue;
            try
            {
                result = Convert.ToDecimal(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to decimal: " + value);
            }

            return (result);
        }

        /// <summary>
        /// To the double.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The default value.</param>
        /// <returns></returns>
        public static double ToDouble<T>(this T value, double defValue = 0.0)
        {
            var result = defValue;
            try
            {
                result = Convert.ToDouble(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to double: " + value);
            }

            return (result);
        }

        /// <summary>
        /// To the int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt<T>(this T value, int defValue = 0)
        {
            var result = defValue;
            try
            {
                //Hp --> Logic: To handle if string value having scientific exponential characters.
                //First convert the string data to double and then convert double value to integer.
                result = (value is string) ?
                    Convert.ToInt32(ToDouble(value)) : Convert.ToInt32(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to int: " + value);
            }
            return (result);
        }

        /// <summary>
        /// To boolean the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public static bool Tobool<T>(this T value, bool defValue = false)
        {
            var result = defValue;
            try
            {
                result = Convert.ToBoolean(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to boolean: " + value);
            }
            return (result);
        }

        /// <summary>
        /// To the nullable bool.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The definition value.</param>
        /// <returns></returns>
        public static bool? ToNullableBool(this int value, bool? defValue = default(bool?))
        {
            var result = defValue;
            try
            {
                if ((value == 0) || (value == 1))
                {
                    result = Convert.ToBoolean(value);
                }
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to boolean: " + value);
            }

            return (result);
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The definition value.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value, DateTime defValue = default(DateTime))
        {
            var result = defValue;
            try
            {
                result = DateTime.Parse(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to datetime: " + value);
            }
            return (result);
        }

        /// <summary>
        /// To the nullable date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The definition value.</param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this string value,
            DateTime? defValue = default(DateTime?))
        {
            var result = defValue;
            try
            {
                result = DateTime.Parse(value);
            }
            catch
            {
                Debug.WriteLine("Cannot convert given value to datetime: " + value);
            }
            return (result);
        }

        /// <summary>
        /// If given value is default datetime then it reset to null otherwise returns same.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime? IsDefaultReturnNull(this DateTime value)
        {
            return ((default(DateTime) == value) ? (DateTime?)null : value);
        }

        /// <summary>
        /// If given value is null or default datetime then it reset to current datetime 
        /// otherwise returns same.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime IsNullOrDefaultReturnNow(this DateTime value)
        {
            return (value.IsNullOrDefault() ? DateTime.Now : value);
        }

        /// <summary>
        /// If given value is null or default datetime then it reset to current datetime 
        /// otherwise returns same.
        /// </summary>
        /// <param name="srcValue">The source value.</param>
        /// <returns></returns>
        public static DateTime IsNullOrDefaultReturnNow(this DateTime? srcValue)
        {
            return (srcValue.IsNullOrDefault() ? DateTime.Now : srcValue.Value);
        }

        /// <summary>
        /// Determines whether given value is null or default.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is null or default] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrDefault(this DateTime value)
        {
            return ((null == value) || (default(DateTime) == value));
        }

        /// <summary>
        /// Determines whether [is null or default].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is null or default] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrDefault(this DateTime? value)
        {
            return ((null == value) || (default(DateTime) == value));
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMemberName(expression);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Action<T>> expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Func<T>> expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid expression</exception>
        private static string GetMemberName(Expression expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            string memberName;

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                memberName = memberExpression.Member.Name;
            }
            else if (expression is MethodCallExpression)
            {
                // Reference type method
                var callExpression = (MethodCallExpression)expression;
                memberName = callExpression.Method.Name;
            }
            else if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                memberName = GetMemberName(unaryExpression);
            }
            else
            {
                throw new ArgumentException("Invalid expression");
            }

            return (memberName);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="unaryExpression">The unary expression.</param>
        /// <returns></returns>
        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (!(unaryExpression.Operand is MethodCallExpression))
                return (((MemberExpression)unaryExpression.Operand).Member.Name);

            var methodExpression = (MethodCallExpression)unaryExpression.Operand;
            return (methodExpression.Method.Name);
        }

        /// <summary>
        /// Gets the type of the declaring.
        /// </summary>
        /// <param name="unaryExpression">The unary expression.</param>
        /// <returns></returns>
        private static Type GetDeclaringType(UnaryExpression unaryExpression)
        {
            if (!(unaryExpression.Operand is MethodCallExpression))
                return (((MemberExpression)unaryExpression.Operand).Member.DeclaringType);

            var methodExpression = (MethodCallExpression)unaryExpression.Operand;
            return (methodExpression.Method.DeclaringType);
        }

        /// <summary>
        /// Gets the type of the declaring.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid expression</exception>
        public static Type GetDeclaringType(Expression expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            Type declaringType;

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                declaringType = memberExpression.Member.DeclaringType;
            }
            else if (expression is MethodCallExpression)
            {
                // Reference type method
                var callExpression = (MethodCallExpression)expression;
                declaringType = callExpression.Method.DeclaringType;
            }
            else if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                declaringType = GetDeclaringType(unaryExpression);
            }
            else
            {
                throw new ArgumentException("Invalid expression");
            }

            return (declaringType);
        }

        /// <summary>
        /// Gets the method arguments.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid expression</exception>
        public static IList<dynamic> GetMethodArguments(Expression expression)
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);
            var result = new List<dynamic>();
            if (!(expression is MethodCallExpression)) return (result);

            // Reference type method
            var callExpression = (MethodCallExpression)expression;
            foreach (var argExpr in callExpression.Arguments)
            {
                if (argExpr is MemberExpression)
                {
                    // Reference type property or field
                    var memberExpression = (MemberExpression)argExpr;
                    result.Add(GetArgumentInfo(memberExpression));
                }
                else if (argExpr is UnaryExpression)
                {
                    // Property, field of method returning value type
                    var unaryExpression = (UnaryExpression)argExpr;
                    result.Add(GetArgumentInfo((MemberExpression)unaryExpression.Operand));
                }
                else if (argExpr is ConstantExpression)
                {
                    //TODO: Hp --> Needs to handle this?
                }
                else
                {
                    throw new ArgumentException("Invalid expression");
                }
            }

            return (result);
        }

        /// <summary>
        /// Gets the argument information.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns></returns>
        private static dynamic GetArgumentInfo(MemberExpression memberExpression)
        {
            dynamic expando = new ExpandoObject();
            var constantExpr = memberExpression.Expression as ConstantExpression;
            if (null == constantExpr) return (expando);

            expando.Name = memberExpression.Member.Name;
            expando.Value = ((FieldInfo)memberExpression.Member).GetValue(constantExpr.Value);
            expando.Type = expando.Value.GetType();

            return (expando);
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="valueSelector">The value selector.</param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type,
        Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var myAttr = type.GetCustomAttributes<TAttribute>(true).SingleOrDefault();
            return ((myAttr != null) ? valueSelector(myAttr) : default(TValue));
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propInfo">The property information.</param>
        /// <param name="valueSelector">The value selector.</param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this PropertyInfo propInfo,
                Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var myAttr = propInfo.GetCustomAttributes<TAttribute>(true).SingleOrDefault();
            return ((myAttr != null) ? valueSelector(myAttr) : default(TValue));
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="propInfo">The property information.</param>
        /// <param name="attributePropFilter">The attribute property filter.</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo propInfo,
                Func<TAttribute, bool> attributePropFilter) where TAttribute : Attribute =>
            propInfo.GetCustomAttributes<TAttribute>(true).SingleOrDefault(attributePropFilter);

        /// <summary>
        /// Gets the class attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="srcType">Type of the source.</param>
        /// <param name="isInherit">if set to <c>true</c> [is inherit].</param>
        /// <returns></returns>
        public static TAttribute GetClassAttribute<TAttribute>(this Type srcType,
            bool isInherit = false) where TAttribute : Attribute
        {
            ErrorBase.CheckArgIsNull(srcType, () => srcType);
            return (srcType.GetCustomAttributes<TAttribute>(isInherit).FirstOrDefault());
        }

        /// <summary>
        /// Gets the method attribute.
        /// </summary>
        /// <typeparam name="T">The type of the class in which this method present.</typeparam>
        /// <param name="srcType">Type of the source.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isInherit">if set to <c>true</c> [is inherit].</param>
        /// <returns></returns>
        public static T GetMethodAttribute<T>(Type srcType, string methodName,
            bool isInherit = true) where T : Attribute
        {
            ErrorBase.CheckArgIsNull(srcType, () => srcType);
            ErrorBase.CheckArgIsNullOrDefault(methodName, () => methodName);

            var myAttrInfo = default(T);
            var mInfo = srcType.GetMethod(methodName, BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance);

            return ((null != mInfo) ? mInfo.GetMethodAttribute<T>(isInherit) : myAttrInfo);
        }

        /// <summary>
        /// Gets the method attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="isInherit">if set to <c>true</c> [is inherit].</param>
        /// <returns></returns>
        public static TAttribute GetMethodAttribute<TAttribute>(this MethodInfo methodInfo,
            bool isInherit = true) where TAttribute : Attribute =>
            methodInfo?.GetCustomAttributes<TAttribute>(isInherit).FirstOrDefault();

        /// <summary>
        /// Gets the enum attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public static TAttribute GetEnumAttribute<TAttribute>(this Enum enumVal)
            where TAttribute : Attribute
        {
            var typeInfo = enumVal.GetType().GetTypeInfo();
            var value = typeInfo.DeclaredMembers.SingleOrDefault(
                x => x.Name.IsEqualTo(enumVal.ToString()));

            return (value?.GetCustomAttribute<TAttribute>());
        }

        /// <summary>
        /// Gets the enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<TEnum> GetEnumValues<TEnum>()
        {
            return (Enum.GetValues(typeof(TEnum)).Cast<TEnum>());
        }

        /// <summary>
        /// Converts string to enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The default value.</param>
        /// <returns>enum value.</returns>
        public static TEnum ConvertToEnum<TEnum>(string value, TEnum defValue)
        {
            var enumValue = defValue;
            try
            {
                enumValue = (TEnum)Enum.Parse(typeof(TEnum), value, true);
            }
            catch
            {
                //Ignore any error
            }
            return enumValue;
        }

        /// <summary>
        /// Gets the enum from a given description.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="defValue">The default value.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns>enum value from its description.</returns>
        public static TEnum GetEnumFrom<TEnum>(this string description,
            TEnum defValue = default(TEnum), bool IsDisplayError = true)
        {
            var enumValue = defValue;
            try
            {
                enumValue = GetEnumValues<TEnum>().Single(item =>
                   GetEnumDescription(item as Enum, IsDisplayError).IsEqualTo(description));
            }
            catch
            {
                //Ignore any error
            }

            return enumValue;
        }

        /// <summary>
        /// Gets the enum from a given description.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <typeparam name="TAttr">The type of the attribute.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="defValue">The default value.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns>
        /// enum value from its description.
        /// </returns>
        public static TEnum GetEnumFrom<TEnum, TAttr>(this string description,
            TEnum defValue = default(TEnum), bool IsDisplayError = true) where TAttr : Attribute
        {
            var enumValue = defValue;
            try
            {
                enumValue = GetEnumValues<TEnum>().Single(item =>
                   GetEnumDescription<TAttr>(item as Enum, IsDisplayError).IsEqualTo(description));
            }
            catch
            {
                //Ignore any error
            }

            return enumValue;
        }

        /// <summary>
        /// Gets the enum from given description.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="description">The description.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns></returns>
        public static Enum GetEnumFrom(this Type enumType, string description,
            bool IsDisplayError = true)
        {
            var enumValue = default(Enum);
            try
            {
                enumValue = Enum.GetValues(enumType).Cast<Enum>().Single(item =>
                   GetEnumDescription(item, IsDisplayError).IsEqualTo(description));
            }
            catch
            {
                //Ignore any error
            }

            return enumValue;
        }

        /// <summary>
        /// Gets the enum from given description.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="description">The description.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns></returns>
        public static Enum GetEnumFrom<TAttr>(this Type enumType, string description,
            bool IsDisplayError = true) where TAttr : Attribute
        {
            var enumValue = default(Enum);
            try
            {
                enumValue = Enum.GetValues(enumType).Cast<Enum>().Single(item =>
                   GetEnumDescription<TAttr>(item, IsDisplayError).Equals(
                       description, StringComparison.CurrentCultureIgnoreCase));
            }
            catch
            {
                //Ignore any error
            }

            return enumValue;
        }

        /// <summary>
        /// Gets the description from a given enum value.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string GetEnumDescription<TAttribute>(this Enum value,
            bool IsDisplayError = true) where TAttribute : Attribute
        {
            ErrorBase.CheckArgIsNull(value, () => value);

            string output = null;
            var type = value.GetType();
            var fi = type.GetField(value.ToString());

            //Hp --> Logic: Look for our 'TAttr' in the field's custom attributes
            var myAttributes = fi.GetCustomAttributes<TAttribute>(false);

            //Hp --> Logic: Only single instance of 'EnumDescriptionAttribute' is allowed.
            var myEnumAttribute = myAttributes?.SingleOrDefault();
            if (myEnumAttribute != null)
            {
                if (typeof(EnumDescriptionAttribute) == myEnumAttribute.GetType())
                {
                    output = (myEnumAttribute as EnumDescriptionAttribute).Description;
                }
                else if (typeof(EnumMemberAttribute) == myEnumAttribute.GetType())
                {
                    output = (myEnumAttribute as EnumMemberAttribute).Value;
                }
                else if (typeof(DescriptionAttribute) == myEnumAttribute.GetType())
                {
                    output = (myEnumAttribute as DescriptionAttribute).Description;
                }
            }
            else
            {
                if (IsDisplayError)
                {
                    ErrorBase.Require(false,
                        $"The given enum value {value} does not have DescriptionAttribute!");
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the description from a given enum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="IsDisplayError">if set to <c>true</c> [is display error].</param>
        /// <returns>
        /// System.String.
        /// </returns>        
        public static string GetEnumDescription(this Enum value, bool IsDisplayError = true)
        {
            ErrorBase.CheckArgIsNull(value, () => value);

            string description = null;
            var enumType = value.GetType();
            var fi = enumType.GetField(value.ToString());
            var customAttributeTypes = new List<Type>
                {
                    typeof(EnumDescriptionAttribute),
                    typeof(EnumMemberAttribute),
                    typeof(DescriptionAttribute),
                };

            var myEnumAttribute = fi.GetCustomAttributes(true)
                .FirstOrDefault(A => customAttributeTypes.Contains(A.GetType()));

            if (null == myEnumAttribute)
            {
                if (IsDisplayError)
                {
                    ErrorBase.Require(false,
                        $"The given enum value {value} does not have DescriptionAttribute!");
                }

                return (description);
            }

            if (typeof(EnumDescriptionAttribute) == myEnumAttribute.GetType())
            {
                description = ((EnumDescriptionAttribute)myEnumAttribute).Description;
            }
            else if (typeof(EnumMemberAttribute) == myEnumAttribute.GetType())
            {
                description = ((EnumMemberAttribute)myEnumAttribute).Value;
            }
            else if (typeof(DescriptionAttribute) == myEnumAttribute.GetType())
            {
                description = ((DescriptionAttribute)myEnumAttribute).Description;
            }

            return (description);
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <param name="xmlRdrSettings">The XML RDR settings.</param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string xmlNamespace, string xmlData,
            XmlReaderSettings xmlRdrSettings = null) where T : class
        {
            ErrorBase.CheckArgIsNullOrDefault(xmlNamespace, () => xmlNamespace);
            ErrorBase.CheckArgIsNullOrDefault(xmlData, () => xmlData);
            T result;

            // Create an XML reader for this file.
            using (var reader = XmlReader.Create(new StringReader(xmlData), xmlRdrSettings))
            {
                var serializer = new XmlSerializer(typeof(T), xmlNamespace);
                result = (T)serializer.Deserialize(reader);
            }

            return (result);
        }

        /// <summary>
        /// De serializes from XML.
        /// </summary>
        /// <typeparam name="T">Serializable class object</typeparam>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <param name="xmlRdrSettings">The XML reader settings.</param>
        /// <returns>
        /// Serializable class object
        /// </returns>
        public static T DeserializeFromXml<T>(string xmlFilePath,
            XmlReaderSettings xmlRdrSettings = null) where T : class
        {
            ErrorBase.CheckArgIsNullOrDefault(xmlFilePath, () => xmlFilePath);
            T result;

            if (!IsFileExists(xmlFilePath))
            {
                var errMessage = $"The given xml \"{xmlFilePath}\" file path doesn't exists!";
                throw (new FileNotFoundException(errMessage));
            }

            // Create an XML reader for this file.
            using (var reader = XmlReader.Create(xmlFilePath, xmlRdrSettings))
            {
                var serializer = new XmlSerializer(typeof(T));
                result = (T)serializer.Deserialize(reader);
            }

            return (result);
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <param name="xsdFilePath">The XSD file path.</param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string xmlFilePath, string xsdFilePath)
            where T : class
        {
            var xmlRdrSettings = GetXmlReaderSettings(xsdFilePath);
            return (DeserializeFromXml<T>(xmlFilePath, xmlRdrSettings));
        }

        /// <summary>
        /// Gets the XML reader settings.
        /// </summary>
        /// <param name="xsdFilePath">The XSD file path.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static XmlReaderSettings GetXmlReaderSettings(string xsdFilePath)
        {
            if (!IsFileExists(xsdFilePath))
            {
                var errMessage = $"The given xsd \"{xsdFilePath}\" file path doesn't exists!";
                throw (new FileNotFoundException(errMessage));
            }

            //Hp --> Logic: Validate the given xml file against its corresponsding schema.
            var xmlRdrSettings = new XmlReaderSettings();
            xmlRdrSettings.Schemas.Add(null, xsdFilePath);
            xmlRdrSettings.ValidationType = ValidationType.Schema;
            xmlRdrSettings.ValidationEventHandler += (sender, e) =>
            {
                throw (new ExceptionBase(e.Message, e.Exception));
            };

            return (xmlRdrSettings);
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <typeparam name="T">Serializable class object</typeparam>
        /// <param name="obj">The serializable class object.</param>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <param name="xmlWtrSettings">The XML writer settings.</param>
        public static void SerializeToXml<T>(T obj, string xmlFilePath,
            XmlWriterSettings xmlWtrSettings = null) where T : class
        {
            ErrorBase.Require(default(T));
            ErrorBase.CheckArgIsNullOrDefault(xmlFilePath, () => xmlFilePath);

            // Create an XML writer for this file.
            using (var writer = XmlWriter.Create(xmlFilePath, xmlWtrSettings))
            {
                var serializer = new XmlSerializer(typeof(T));
                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Writes the XSL transformed XML file.
        /// </summary>
        /// <param name="xslFileFullPath">The XSL file full path.</param>
        /// <param name="inputXmlFileFullPath">The input XML file full path.</param>
        /// <param name="outputXmlFileFullPath">The input XML file full path.</param>
        public static void WriteXslTransformedXmlFile(string xslFileFullPath,
            string inputXmlFileFullPath, string outputXmlFileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(xslFileFullPath, () => xslFileFullPath);
            ErrorBase.CheckArgIsNullOrDefault(inputXmlFileFullPath, () => inputXmlFileFullPath);
            ErrorBase.CheckArgIsNullOrDefault(outputXmlFileFullPath, () => outputXmlFileFullPath);

            var myXslTransform = new XslCompiledTransform();
            myXslTransform.Load(xslFileFullPath);
            myXslTransform.Transform(inputXmlFileFullPath, outputXmlFileFullPath);
        }

        /// <summary>
        /// Gets the XSL transformed output data.
        /// </summary>
        /// <param name="inputXmlData">The input XML data.</param>
        /// <param name="xslFileFullPath">The XSL file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetXslTransformedOutputData(string inputXmlData,
            string xslFileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(inputXmlData, () => inputXmlData);
            ErrorBase.CheckArgIsNullOrDefault(xslFileFullPath, () => xslFileFullPath);

            string outputData;
            XPathDocument xPathDoc;
            using (var strReader = new StringReader(inputXmlData))
            {
                xPathDoc = new XPathDocument(strReader);
            }

            var myXslTransform = new XslCompiledTransform();
            myXslTransform.Load(xslFileFullPath);
            using (var strWriter = new StringWriter())
            {
                myXslTransform.Transform(xPathDoc, null, strWriter);
                outputData = strWriter.ToString();
            }

            return (outputData);
        }

        /// <summary>
        /// Gets the duplicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source,
            bool isDistinct = true)
        {
            ErrorBase.CheckArgIsNull(source, () => source);

            var duplicates = source.GroupBy(item => item).SelectMany(item => item.Skip(1));
            if (isDistinct)
            {
                duplicates = duplicates.Distinct();
            }

            return (duplicates);
        }

        /// <summary>
        /// Determines whether [is valid filename] [the specified filename without ext].
        /// </summary>
        /// <param name="filenameWithoutExt">The filename without ext.</param>
        /// <returns><c>true</c> if [is valid filename] [the specified filename without ext];
        /// otherwise, <c>false</c>.</returns>
        public static bool IsValidFilename(string filenameWithoutExt)
        {
            //ErrorBase.Require(!string.IsNullOrEmpty(filenameWithoutExt)); //Hp --> Not required, 
            //down we are handling

            //Hp --> Note: From MSDN documentation,the array returned from this method is not 
            //guaranteed to contain the complete set of characters that are invalid in file and
            //directory names. The full set of invalid characters can vary by file system. 
            //For example, on Windows-based desktop platforms, invalid path characters might 
            //include ASCII/Unicode characters 1 through 31, as well as quote ("), less than (<), 
            //greater than (>), pipe (|), backspace (\b), null (\0) and tab (\t).
            var inValidFileNameCharSet = new string(Path.GetInvalidFileNameChars());
            var regToValidate = new Regex("[" + Regex.Escape(inValidFileNameCharSet) + "]");

            //Hp --> Logic: May be file name contains all empty spaces (or) empty string.
            //so, this behavior also we need to treat it as invalid.
            return ((!string.IsNullOrEmpty(filenameWithoutExt.Trim())) &&
                (!regToValidate.IsMatch(filenameWithoutExt.Trim())));
        }

        /// <summary>
        /// Gets the valid filename by removing invalid file name characters.
        /// </summary>
        /// <param name="filenameWithoutExt">The filename without ext.</param>
        /// <returns></returns>
        public static string GetValidFilename(string filenameWithoutExt)
        {
            ErrorBase.Require(!string.IsNullOrEmpty(filenameWithoutExt));
            var validFilename = filenameWithoutExt.Trim();

            //Hp --> Note: From MSDN documentation,the array returned from this method is not 
            //guaranteed to contain the complete set of characters that are invalid in file and
            //directory names. The full set of invalid characters can vary by file system. 
            //For example, on Windows-based desktop platforms, invalid path characters might 
            //include ASCII/Unicode characters 1 through 31, as well as quote ("), less than (<), 
            //greater than (>), pipe (|), backspace (\b), null (\0) and tab (\t).
            var inValidFileNameCharSet = new string(Path.GetInvalidFileNameChars());
            var regToValidate = new Regex("[" + Regex.Escape(inValidFileNameCharSet) + "]");
            if (regToValidate.IsMatch(validFilename))
            {
                validFilename = regToValidate.Replace(validFilename, string.Empty);
            }

            return (validFilename);
        }

        /// <summary>
        /// Converts the UTF16 string to UTF8.
        /// </summary>
        /// <param name="utf16Str">The UTF16 string.</param>
        /// <returns>System.String.</returns>
        public static string ConvertUtf16StrToUtf8(string utf16Str)
        {
            ErrorBase.CheckArgIsNullOrDefault(utf16Str, () => utf16Str);

            //Get UTF16 bytes by reading each byte using unicode encoding.
            var utf16Bytes = Encoding.Unicode.GetBytes(utf16Str);

            //Convert UTF16 bytes to UTF8 bytes.
            var utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            //Return UTF8 bytes as ANSII string.
            return (Encoding.Default.GetString(utf8Bytes));
        }

        /// <summary>
        /// Converts the UTF8 string to UTF16.
        /// </summary>
        /// <param name="utf8Str">The UTF8 string.</param>
        /// <returns>System.String.</returns>
        public static string ConvertUtf8StrToUtf16(string utf8Str)
        {
            ErrorBase.CheckArgIsNullOrDefault(utf8Str, () => utf8Str);

            //Get UTF8 bytes by reading each byte using ANSII encoding.
            var utf8Bytes = Encoding.Default.GetBytes(utf8Str);

            //Convert UTF8 bytes to UTF16 bytes.
            var utf16Bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, utf8Bytes);

            //Return UTF16 bytes as UTF16 string.
            return (Encoding.Unicode.GetString(utf16Bytes));
        }

        /// <summary>
        /// Reads the content of the file.
        /// </summary>
        /// <param name="srcFilePath">The source file full path.</param>
        /// <returns>Returns file content in bytes.</returns>
        public static byte[] ReadFromFile(string srcFilePath)
        {
            if (!IsFileExists(srcFilePath)) return (null);

            byte[] fileContent;
            using (var currentFileStream = File.OpenRead(srcFilePath))
            {
                fileContent = new byte[currentFileStream.Length];
                currentFileStream.Read(fileContent, 0, fileContent.Length);
            }

            return (fileContent);
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dstFilePath">The destination file full path.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns>
        /// dynamic.
        /// </returns>
        public static dynamic WriteToFile(string data, string dstFilePath,
            CancellationToken cancelToken = default(CancellationToken))
        {
            dynamic expando = new ExpandoObject();
            expando.IsSuccess = false;
            expando.IsLocked = IsFileLocked(dstFilePath, showErrorMsg: true);
            if (expando.IsLocked)
            {
                return (expando);
            }

            //Write data to user specified location.
            using (var writer = new StreamWriter(dstFilePath))
            {
                if ((default(CancellationToken) != cancelToken) &&
                (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                writer.Write(data);
            }

            expando.IsSuccess = true;
            return (expando);
        }

        /// <summary>
        /// Writes the stream to file.
        /// </summary>
        /// <param name="srcFileStream">The source file stream.</param>
        /// <param name="dstFilePath">The destination file full path.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public static void WriteToFile(Stream srcFileStream, string dstFilePath,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(srcFileStream, () => srcFileStream);

            //If destination file already exists then delete.
            DeleteFile(dstFilePath);

            //If corresponding file directory does not exists then create it.
            var dirPath = GetDirectoryPath(dstFilePath);
            CreateDirectory(dirPath);

            //Hp --> Always remove ReadOnly file attribute, since we are forcibly writing data.
            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.Attributes &= ~FileAttributes.ReadOnly;

            //Create destination file and write source content into it.
            using (Stream destination = File.OpenWrite(dstFilePath))
            {
                if ((default(CancellationToken) != cancelToken) &&
                (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                CopyStreamFrom(srcFileStream, destination);
            }
        }

        /// <summary>
        /// Writes the content to file.
        /// </summary>
        /// <param name="srcContent">Content of the source.</param>
        /// <param name="dstFilePath">The destination file full path.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public static void WriteToFile(byte[] srcContent, string dstFilePath,
            CancellationToken cancelToken = default(CancellationToken))
        {
            ErrorBase.CheckArgIsNull(srcContent, () => srcContent);

            //If destination file already exists then delete.
            DeleteFile(dstFilePath);

            //If corresponding file directory does not exists then create it.
            var dirPath = GetDirectoryPath(dstFilePath);
            CreateDirectory(dirPath);

            //Hp --> Always remove ReadOnly file attribute, since we are forcibly writing data.
            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.Attributes &= ~FileAttributes.ReadOnly;

            //Create destination file and write source content into it.
            using (var writer = File.Create(dstFilePath))
            {
                if ((default(CancellationToken) != cancelToken) &&
                (cancelToken.IsCancellationRequested))
                {
                    cancelToken.ThrowIfCancellationRequested();
                }

                writer.Write(srcContent, 0, srcContent.Length);
            }
        }

        /// <summary>
        /// Copies the stream from given source file to destination.
        /// </summary>
        /// <param name="srcFilePath">The source file path.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyStreamFrom(string srcFilePath, Stream destination)
        {
            ErrorBase.CheckArgIsNull(destination, () => destination);

            var fileContent = ReadFromFile(srcFilePath);
            if (null == fileContent)
            {
                return;
            }

            var source = new MemoryStream(fileContent);
            CopyStreamFrom(source, destination);
        }

        /// <summary>
        /// Copies the stream from source to destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyStreamFrom(Stream source, Stream destination)
        {
            ErrorBase.CheckArgIsNull(source, () => source);
            ErrorBase.CheckArgIsNull(destination, () => destination);

            int ReadBytesSize = 0x1000;
            var buffer = new byte[ReadBytesSize];
            int read = 0;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="dirPath">The directory path.</param>
        /// <param name="directorySecurity">The directory access control list (ACL) entries for a specified directory.</param>
        public static void CreateDirectory(string dirPath, DirectorySecurity directorySecurity = null)
        {
            if (IsDirectoryExists(dirPath))
            {
                return;
            }

            if (directorySecurity == null)
            {
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                Directory.CreateDirectory(dirPath, directorySecurity);
            }
        }

        /// <summary>
        /// Gets the directory access control list (ACL) entries for a specified directory.
        /// </summary>
        /// <param name="dirPath">The directory path.</param>
        /// <returns></returns>
        public static DirectorySecurity GetDirAccessControlList(string dirPath)
        {
            if (!IsDirectoryExists(dirPath))
            {
                return default(DirectorySecurity);
            }

            return Directory.GetAccessControl(dirPath);
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="isRecursive">if set to <c>true</c> [is recursive].</param>
        public static void DeleteFolder(string folderPath, bool isRecursive = true)
        {
            //Hp --> Logic: Trigger recursive call only when IOException occurs.
            if (!IsDirectoryExists(folderPath)) return;

            try
            {
                Directory.Delete(folderPath, isRecursive);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.Message);
                //If we get IOException, while deleting directories that are open in Explorer, 
                //add a sleep(0) to give Explorer a chance to release the directory handle.
                Thread.Sleep(0);
                DeleteFolder(folderPath, isRecursive);
            }
        }

        /// <summary>
        /// Renames the folder.
        /// </summary>
        /// <param name="srcFolderPath">The source folder path.</param>
        /// <param name="dstFolderPath">The destination folder path.</param>
        public static void RenameFolder(string srcFolderPath, string dstFolderPath)
        {
            if ((IsDirectoryExists(srcFolderPath)) &&
                (!IsDirectoryExists(dstFolderPath)))
            {
                Directory.Move(srcFolderPath, dstFolderPath);
            }
        }

        /// <summary>
        /// Deletes the file if exists.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        public static void DeleteFile(string fileFullPath)
        {
            if (IsFileExists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }
        }

        /// <summary>
        /// Renames the file if exists.
        /// </summary>
        /// <param name="srcFileFullPath">Source file full path.</param>
        /// <param name="dstFileNameWithoutExt">Destination filename without extension.</param>
        /// <returns>If success returns renamed file full path otherwise empty string</returns>
        public static string RenameFile(string srcFileFullPath, string dstFileNameWithoutExt)
        {
            ErrorBase.CheckArgIsNullOrDefault(dstFileNameWithoutExt, () => dstFileNameWithoutExt);

            var dstFileFullPath = string.Empty;
            if (!IsFileExists(srcFileFullPath)) return (dstFileFullPath);

            dstFileFullPath = GetCombinedPath(GetDirectoryPath(srcFileFullPath),
                (dstFileNameWithoutExt + GetFileExtension(srcFileFullPath)));

            //Rename the src filename with destination filename
            File.Move(srcFileFullPath, dstFileFullPath);

            return (dstFileFullPath);
        }

        /// <summary>
        /// Takes a copy of source file with user provided filename in the location where source
        /// file exists.
        /// </summary>
        /// <param name="srcFileFullPath">Source file full path.</param>
        /// <param name="dstFileNameWithoutExt">Destination filename without extension.</param>
        /// <param name="isOverWrite">True, to overwrite if destination file exists</param>
        public static void CopyFileToSameLocation(string srcFileFullPath,
            string dstFileNameWithoutExt, bool isOverWrite = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(dstFileNameWithoutExt, () => dstFileNameWithoutExt);

            if (!IsFileExists(srcFileFullPath)) return;

            var dstFileFullPath = GetCombinedPath(GetDirectoryPath(srcFileFullPath),
                (dstFileNameWithoutExt + GetFileExtension(srcFileFullPath)));

            //Copy the src filename with destination filename
            File.Copy(srcFileFullPath, dstFileFullPath, isOverWrite);
        }

        /// <summary>
        /// Copies the existing file.
        /// </summary>
        /// <param name="srcFileFullPath">The source file full path.</param>
        /// <param name="dstDirPath">The Destination dir path.</param>
        /// <param name="dstFileNameWithoutExt">The Destination file name without extension.</param>
        /// <param name="isOverWrite">if set to <c>true</c> [is over write].</param>
        public static void CopyExistingFile(string srcFileFullPath, string dstDirPath,
            string dstFileNameWithoutExt, bool isOverWrite = false)
        {
            ErrorBase.CheckArgIsNullOrDefault(dstFileNameWithoutExt, () => dstFileNameWithoutExt);

            if (!IsFileExists(srcFileFullPath)) return;

            if (!IsDirectoryExists(dstDirPath))
            {
                CreateDirectory(dstDirPath);
            }

            var dstFileFullPath = GetCombinedPath(dstDirPath,
                (dstFileNameWithoutExt + GetFileExtension(srcFileFullPath)));

            //Copy the src filename with destination filename
            File.Copy(srcFileFullPath, dstFileFullPath, isOverWrite);
        }

        /// <summary>
        /// Moves the existing file.
        /// </summary>
        /// <param name="srcFileFullPath">The source file full path.</param>
        /// <param name="dstDirPath">The destination directory path.</param>
        public static void MoveExistingFile(string srcFileFullPath, string dstDirPath)
        {
            if (!IsFileExists(srcFileFullPath)) return;

            if (!IsDirectoryExists(dstDirPath))
            {
                CreateDirectory(dstDirPath);
            }

            var dstFileFullPath = GetCombinedPath(
                dstDirPath, GetFileName(srcFileFullPath));

            //Moves the src file to user specified destination.
            File.Move(srcFileFullPath, dstFileFullPath);
        }

        /// <summary>
        /// Gets the application full path.
        /// </summary>
        /// <value>The application full path.</value>
        public static string AppFullPath
        {
            get { return (Assembly.GetEntryAssembly().Location); }
        }

        /// <summary>
        /// Gets the application directory path.
        /// </summary>
        /// <value>The application directory path.</value>
        public static string AppDirPath
        {
            get { return (GetDirectoryPath(AppFullPath)); }
        }

        /// <summary>
        /// Gets the calling assembly full path.
        /// </summary>
        /// <value>
        /// The application full path.
        /// </value>
        public static string CallingAssemblyFullPath
        {
            get { return (Assembly.GetCallingAssembly().Location); }
        }

        /// <summary>
        /// Gets the calling assembly directory path.
        /// </summary>
        /// <value>
        /// The application directory path.
        /// </value>
        public static string CallingAssemblyDirPath
        {
            get { return (GetDirectoryPath(CallingAssemblyFullPath)); }
        }

        /// <summary>
        /// Gets the system application data path.
        /// </summary>
        /// <value>The system application data path.</value>
        public static string SystemAppDataPath
        {
            get { return (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); }
        }

        /// <summary>
        /// Gets the local application data path.
        /// </summary>
        /// <value>
        /// The local application data path.
        /// </value>
        public static string LocalAppDataPath
        {
            get
            {
                return (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }
        }

        /// <summary>
        /// Gets the combined path.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="childPath">The child path.</param>
        /// <returns>System.String.</returns>
        public static string GetCombinedPath(string parentPath, string childPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(parentPath, () => parentPath);
            ErrorBase.CheckArgIsNullOrDefault(childPath, () => childPath);
            return Path.Combine(parentPath, childPath.GetValidChildPath());
        }

        /// <summary>
        /// Gets the combined path.
        /// </summary>
        /// <param name="paths">The list of paths to combine.</param>
        /// <returns>System.String.</returns>
        public static string GetCombinedPath(string[] paths)
        {
            ErrorBase.CheckArgIsNullOrDefault(paths, () => paths);
            var filePaths = paths.ToList();
            if (filePaths.Count > 1)
            {
                var childPaths = filePaths.Skip(1).Select(P => P.GetValidChildPath());
                filePaths = filePaths.Take(1).Concat(childPaths).ToList();
            }
            
            return Path.Combine(filePaths.ToArray());
        }

        /// <summary>
        /// Gets the valid child path.
        /// </summary>
        /// <param name="childPath">The child path.</param>
        /// <returns></returns>
        public static string GetValidChildPath(this string childPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(childPath, () => childPath);
            //Hp --> BugFix: Path.Combine fails to return file full path if child path starts with
            //directory separator.
            if (Path.IsPathRooted(childPath))
            {
                var delimeters = new char[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                };

                childPath = childPath.TrimStart(delimeters);
            }

            return childPath;
        }

        /// <summary>
        /// Gets the directory path.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetDirectoryPath(string fileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileFullPath, () => fileFullPath);
            return (Path.GetDirectoryName(fileFullPath));
        }

        /// <summary>
        /// Gets the name of the current directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>System.String.</returns>
        public static string GetCurrentDirName(string dirPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(dirPath, () => dirPath);
            return (new DirectoryInfo(dirPath).Name);
        }

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetFileNameWithoutExtension(string fileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileFullPath, () => fileFullPath);
            return (Path.GetFileNameWithoutExtension(fileFullPath));
        }

        /// <summary>
        /// Gets the name of the file with its extension.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetFileName(string fileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileFullPath, () => fileFullPath);
            return (Path.GetFileName(fileFullPath));
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetFileExtension(string fileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileFullPath, () => fileFullPath);
            return (Path.GetExtension(fileFullPath));
        }

        /// <summary>
        /// Determines whether [is directory exists] [the specified dir path].
        /// </summary>
        /// <param name="dirPath">The directory path.</param>
        /// <returns><c>true</c> if [is directory exists] [the specified dir path]; otherwise,
        /// <c>false</c>.</returns>
        public static bool IsDirectoryExists(string dirPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(dirPath, () => dirPath);
            return (Directory.Exists(dirPath));
        }

        /// <summary>
        /// Determines whether [is file exists] [the specified file path].
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns><c>true</c> if [is file exists] [the specified file path]; 
        /// otherwise, <c>false</c>.</returns>
        public static bool IsFileExists(string fileFullPath)
        {
            ErrorBase.CheckArgIsNullOrDefault(fileFullPath, () => fileFullPath);
            return (File.Exists(fileFullPath));
        }

        /// <summary>
        /// Determines whether given path is directory (or) file path.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>
        ///   <c>true</c> if given path is directory path; otherwise, given path is file path <c>false</c>.
        /// </returns>
        public static bool IsDirectory(string fileFullPath)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(fileFullPath));
            var fileSystemInfo = new DirectoryInfo(fileFullPath);
            if (fileSystemInfo == null)
            {
                return (false);
            }

            if ((int)fileSystemInfo.Attributes != -1)
            {
                // if attributes are initialized check the directory flag
                return (fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory));
            }

            // If we get here the file probably doesn't exist yet.  The best we can do is 
            // try to judge intent.  Because directories can have extensions and files
            // can lack them, we can't rely on filename.
            var fileExt = GetFileExtension(fileFullPath);
            if (!string.IsNullOrEmpty(fileExt))
            {
                return (false);
            }

            // We can reasonably assume that if the path doesn't exist yet and 
            // FileSystemInfo is a DirectoryInfo, a directory is intended.  FileInfo can 
            // make a directory, but it would be a bizarre code path.
            return (fileSystemInfo is DirectoryInfo);
        }

        /// <summary>
        /// Determines whether [is folder locked] [the specified directory full path].
        /// </summary>
        /// <param name="dirFullPath">The directory full path.</param>
        /// <param name="showErrorMsg">if set to <c>true</c> [show error MSG].</param>
        /// <returns><c>true</c> if specified folder is locked; otherwise, <c>false</c>.</returns>
        public static bool IsFolderLocked(string dirFullPath, bool showErrorMsg = false)
        {
            if (!IsDirectoryExists(dirFullPath)) return (false);

            var isLocked = false;
            var tempDirName = "Temp_" + GetCurrentDirName(dirFullPath);
            var tempDirFullPath = GetCombinedPath(
                GetDirectoryPath(dirFullPath), tempDirName);

            try
            {
                //Hp --> Logic: Try to rename the folder. If any instance is opened then 
                //rename operation fails. 
                RenameFolder(dirFullPath, tempDirFullPath);

                //If rename operation success, then revert to its original name. 
                RenameFolder(tempDirFullPath, dirFullPath);
            }
            catch (IOException ex)
            {
                isLocked = true;
                var errorMsg = string.Format(CultureInfo.CurrentUICulture, ex.Message);
                ErrorBase.Require(!showErrorMsg, errorMsg);
            }

            return (isLocked);
        }

        /// <summary>
        /// Determines whether [is file locked] [the specified file full path].
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <param name="showErrorMsg">if set to <c>true</c> [show error MSG].</param>
        /// <returns><c>true</c> if specified file is locked; otherwise, <c>false</c>.</returns>
        public static bool IsFileLocked(string fileFullPath, bool showErrorMsg = false)
        {
            if (!IsFileExists(fileFullPath)) return (false);

            var isLocked = false;
            var srcFileNameWithoutExt = GetFileNameWithoutExtension(fileFullPath);
            var tempFileNameWithoutExt = "Temp_" + srcFileNameWithoutExt;

            try
            {
                //Hp --> Step 1: Try to rename the file. If any instance is opened then rename
                //operation fails. 
                var tempFileFullPath =
                    RenameFile(fileFullPath, tempFileNameWithoutExt);

                //If rename operation success, then revert to its original name. 
                RenameFile(tempFileFullPath, srcFileNameWithoutExt);

                //Hp --> Step 2: Try to open the file. If any instance is opened then open 
                //operation fails. 
                using (new FileStream(fileFullPath, FileMode.Open))
                {
                    //Hp --> Do Nothing, since we are just checking whether it is opened by 
                    //other process (or) not.
                }
            }
            catch (IOException ex)
            {
                isLocked = true;
                var errorMsg = string.Format(CultureInfo.CurrentUICulture, ex.Message);
                ErrorBase.Require(!showErrorMsg, errorMsg);
            }

            return (isLocked);
        }

        /// <summary>
        /// Gets the expanded environment variable full path
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public static string GetExpandedEnvVarPath(string filePath)
        {
            var envVarPath = (string.IsNullOrEmpty(filePath)) ? string.Empty :
                                (!filePath.Contains("%")) ? filePath :
                                Environment.ExpandEnvironmentVariables(filePath);

            return ((!string.IsNullOrEmpty(envVarPath)) ?
                    Path.GetFullPath(envVarPath) : string.Empty);
        }

        /// <summary>
        /// Gets the corresponding app settings key related value defined inside app.config file.
        /// </summary>
        /// <param name="appSettingKey">App settings key defined inside app.config file.</param>
        /// <returns>System.String.</returns>
        public static string GetValueFromAppSettings(string appSettingKey)
        {
            ErrorBase.CheckArgIsNullOrDefault(appSettingKey, () => appSettingKey);
            var appSettingValue = string.Empty;

            var myExeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if ((null != myExeConfig.AppSettings.Settings) &&
                (myExeConfig.AppSettings.Settings.AllKeys.Contains(appSettingKey)))
            {
                appSettingValue = myExeConfig.AppSettings.Settings[appSettingKey].Value;
            }

            return (appSettingValue);
        }

        /// <summary>
        /// Converts to CSV format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The data.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <param name="isShowHeader">if set to <c>true</c> [show header].</param>
        /// <param name="isParseEnumValue">if set to <c>true</c> [is parse enum value].</param>
        /// <param name="dateFormat">The date format.</param>
        /// <returns></returns>
        public static string ToCsvFormat<T>(this IList<T> source, string delimeter = ",",
            bool isShowHeader = true, bool isParseEnumValue = false, string dateFormat = "")
            where T : class
        {
            ErrorBase.CheckArgIsNull(source, () => source);
            Func<object, string> ToCellValue = (col) =>
            {
                var cellData = (null == col) ? string.Empty : col.ToString().Trim();
                Func<string, string> cellDataFormat = (cell) => $"\"{cell}\"";
                if (string.IsNullOrWhiteSpace(dateFormat))
                {
                    return (cellDataFormat(cellData));
                }

                DateTime myDateTime;
                if (DateTime.TryParse(cellData, out myDateTime))
                {
                    //Hp --> Logic: Write cell date value with user provided format.
                    return (cellDataFormat(myDateTime.ToString(dateFormat)));
                }

                return (cellDataFormat(cellData));
            };

            //Hp --> BugFix: If column data contains delimeter charater then shows wrong results.
            //Comment: While writing sorrounded column data with double quotes.
            var csvHeader = string.Join(delimeter, source.FirstOrDefault()
                .AsDictionary()
                .Keys
                .Select(col => $"\"{col}\""));

            var csvRows = source.Select(item => string.Join(delimeter, item
                .AsDictionary(isParseEnumValue: isParseEnumValue)
                .Values
                .Select(col => ToCellValue(col))));

            return (string.Join(Environment.NewLine, (isShowHeader) ?
                new List<string> { csvHeader }.Concat(csvRows) : csvRows));
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static string GetPropertyName(this PropertyInfo propertyInfo) =>
            propertyInfo?.Name?.Trim() ?? string.Empty;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcObject">The source object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T srcObject, string propertyName)
        {
            ErrorBase.CheckArgIsNull(srcObject, () => srcObject);
            ErrorBase.CheckArgIsNullOrDefault(propertyName, () => propertyName);

            //Hp --> Logic: Using reflection get given property value by its name.
            return (srcObject.GetType().GetProperty(propertyName).GetValue(srcObject, null));
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcObject">The source object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        public static void SetPropertyValue<T>(this T srcObject, string propertyName,
            object propertyValue)
        {
            ErrorBase.CheckArgIsNull(srcObject, () => srcObject);
            ErrorBase.CheckArgIsNullOrDefault(propertyName, () => propertyName);

            var srcObjType = srcObject.GetType();
            var propInfo = srcObjType.GetProperty(propertyName);

            //Hp --> Note: Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = propInfo.PropertyType.GetGenericInnerType();

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyValue = Convert.ChangeType(propertyValue, targetType);

            //Hp --> Logic: Using reflection set given property value by its name.
            propInfo.SetValue(srcObject, propertyValue, null);
        }

        /// <summary>
        /// Determines whether [is nullable type] [the specified type].
        /// </summary>
        /// <param name="srcType">Type of the source.</param>
        /// <returns>
        ///   <c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type srcType)
        {
            return ((srcType.IsGenericType) &&
                (srcType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        /// <summary>
        /// Determines whether [is generic list type].
        /// </summary>
        /// <param name="srcType">Type of the source.</param>
        /// <returns>
        ///   <c>true</c> if [is list type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGenericListType(this Type srcType)
        {
            return ((srcType.IsGenericType) &&
                (srcType.GetGenericTypeDefinition().Equals(typeof(IList<>))));
        }

        /// <summary>
        /// Gets the type of the generic.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetGenericInnerType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return (type);
            }

            //Hp --> Assumption: We are only considering list, concreate class with one type only.
            var innerType = default(Type);
            if (type.IsNullableType())
            {
                innerType = Nullable.GetUnderlyingType(type);
            }
            else if ((type.IsGenericListType()) || (type.GetGenericArguments().Count() == 1))
            {
                innerType = type.GetGenericArguments()[0];
            }
            else
            {
                innerType = type;
            }

            return (innerType);
        }

        /// <summary>
        /// Converts the given string value to desired target type string.
        /// </summary>
        /// <param name="valueAsStr">The value as string.</param>
        /// <param name="targetTypeAsStr">The type as string.</param>
        /// <returns></returns>
        public static object ConvertTo(string valueAsStr, string targetTypeAsStr)
        {
            if (string.IsNullOrWhiteSpace(targetTypeAsStr))
            {
                return (valueAsStr);
            }

            //Hp --> Note: Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = Type.GetType(targetTypeAsStr);
            return (ConvertTo(valueAsStr, targetType));
        }

        /// <summary>
        /// Converts the given string value to desired target type.
        /// </summary>
        /// <param name="valueAsStr">The value as string.</param>
        /// <param name="targetType">The type as string.</param>
        /// <returns></returns>
        public static object ConvertTo(string valueAsStr, Type targetType)
        {
            return (Convert.ChangeType(valueAsStr, targetType));
        }

        /// <summary>
        /// Determines whether [is simple type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSimpleType(this Type type)
        {
            var myTypes = new Type[]
            {
                typeof(Enum),
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            };

            return ((type.IsPrimitive) || (myTypes.Contains(type)) ||
                (Convert.GetTypeCode(type) != TypeCode.Object) ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                IsSimpleType(type.GetGenericArguments()[0])));
        }

        /// <summary>
        /// Gets all complex (or) navigation properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllComplexProperties<T>()
        {
            return (GetAllProperties(typeof(T)).Where(P => !P.PropertyType.IsSimpleType()));
        }

        /// <summary>
        /// Convert dictionary data to given object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
        {
            var myObject = new T();
            var myObjectType = myObject.GetType();

            foreach (var item in source)
            {
                myObject.SetPropertyValue(item.Key, item.Value);
            }

            return (myObject);
        }

        /// <summary>
        /// Converts given dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <param name="isParseEnumValue">if set to <c>true</c> [is parse enum value].</param>
        /// <returns></returns>
        public static IDictionary<string, object> AsDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public |
            BindingFlags.Instance, bool isParseEnumValue = false)
        {
            return (source.GetAllProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, isParseEnumValue)
            ));
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="source">The source.</param>
        /// <param name="isParseEnumValue">if set to <c>true</c> [is parse enum value].</param>
        /// <returns>System.Object.</returns>
        private static object GetValue(this PropertyInfo propertyInfo, object source,
            bool isParseEnumValue = false)
        {
            ErrorBase.CheckArgIsNull(source, () => source);
            var value = propertyInfo.GetValue(source, null);

            var propType = propertyInfo.PropertyType;
            if ((isParseEnumValue) && (propType.IsEnum))
            {
                var description = GetEnumDescription(value as Enum, IsDisplayError: false);
                if (null != description)
                {
                    value = description;
                }
            }

            return (value);
        }

        /// <summary>
        /// Gets all object properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public |
            BindingFlags.Instance) => GetAllProperties(source.GetType(), bindingAttr);

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type sourceType,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public |
            BindingFlags.Instance) => sourceType.GetProperties(bindingAttr).AsEnumerable();

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="attributePropFilter">The attribute property filter.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties<TAttribute>(this object source,
            Func<TAttribute, bool> attributePropFilter, BindingFlags bindingAttr =
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            where TAttribute : Attribute =>
            GetAllProperties(source.GetType(), attributePropFilter, bindingAttr);

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="attributePropFilter">The attribute property filter.</param>
        /// <param name="bindingAttr">The binding attribute.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties<TAttribute>(this Type sourceType,
            Func<TAttribute, bool> attributePropFilter, BindingFlags bindingAttr =
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
            where TAttribute : Attribute =>
            sourceType.GetProperties(bindingAttr).Where(
                P => P.GetAttribute(attributePropFilter) != null);

        /// <summary>
        /// Deserializes from json string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr">The json string.</param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string jsonStr) where T : class
        {
            return (JsonConvert.DeserializeObject<T>(jsonStr,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                }));
        }

        /// <summary>
        /// Serializes to json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">The json object.</param>
        /// <returns></returns>
        public static string SerializeToJson<T>(T jsonObject) where T : class
        {
            return (JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                }));
        }

        /// <summary>
        /// The compiled camel case regex1
        /// </summary>
        private static readonly Regex CompiledCamelCaseRegex1 =
            new Regex(@"(\P{Ll})(\P{Ll}\p{Ll})", RegexOptions.Compiled);

        /// <summary>
        /// The compiled camel case regex2
        /// </summary>
        private static readonly Regex CompiledCamelCaseRegex2 =
            new Regex(@"(\p{Ll})(\P{Ll})", RegexOptions.Compiled);

        /// <summary>
        /// Splits the camel case.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string propertyName)
        {
            var result = CompiledCamelCaseRegex1.Replace(propertyName, "$1 $2");
            return (CompiledCamelCaseRegex2.Replace(result, "$1 $2"));
        }

        /// <summary>
        /// Invokes as impersonate.
        /// </summary>
        /// <param name="codeBlock">The code block.</param>
        public static void InvokeAsImpersonate(Action codeBlock)
        {
            using (var context = WindowsIdentity.Impersonate(IntPtr.Zero))
            {
                try
                {
                    codeBlock?.Invoke();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    context?.Undo();
                }
            }
        }

        /// <summary>
        /// Invokes as impersonate.
        /// </summary>
        /// <param name="codeBlock">The code block.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public static void InvokeAsImpersonate(Action codeBlock, string domain, string username,
            string password)
        {
            using (new Impersonation(domain, username, password))
            {
                codeBlock?.Invoke();
            }
        }

        /// <summary>
        /// Invokes as impersonate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeBlock">The code block.</param>
        /// <returns></returns>
        public static T InvokeAsImpersonate<T>(Func<T> codeBlock)
        {
            using (var context = WindowsIdentity.Impersonate(IntPtr.Zero))
            {
                try
                {
                    return ((codeBlock != null) ? codeBlock.Invoke() : default(T));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    context?.Undo();
                }
            }
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wcfObject">The WCF object.</param>
        /// <returns>System.String.</returns>
        public static string SerializeToXml<T>(this T wcfObject) where T : class
        {
            var strBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(strBuilder))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(writer, wcfObject);
                writer.Flush();
            }

            return (strBuilder.ToString());
        }

        /// <summary>
        /// Deserializes from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wcfRawXml">The WCF raw XML.</param>
        /// <returns>T.</returns>
        public static T DeserializeFromXml<T>(string wcfRawXml) where T : class
        {
            var wcfObject = default(T);
            using (var reader = XmlReader.Create(new StringReader(wcfRawXml)))
            {
                //Hp --> Note: DataContractSerializer expects things to be in alphabetical order.
                //So, We need to add Order to our Data Members for this to work correctly.
                var serializer = new DataContractSerializer(typeof(T));
                wcfObject = (T)serializer.ReadObject(reader);
            }

            return (wcfObject);
        }

        /// <summary>
        /// Tries validating the object by data annotation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectWithDataAnnotations">The object with data annotations.</param>
        /// <param name="results">The results.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="items">The items.</param>
        /// <param name="validateAllProperties">if set to <c>false</c> validate only properties with a[Required] attribute.</param>
        /// <returns>
        ///   <c>true</c> if given object satisfies data annotation rules, <c>false</c> otherwise.
        /// </returns>
        public static bool TryValidateObject<T>(this T objectWithDataAnnotations,
            out IList<ValidationResult> results, IServiceProvider serviceProvider = null,
            IDictionary<object, object> items = null, bool validateAllProperties = true)
            where T : class
        {
            var context = new ValidationContext(objectWithDataAnnotations, serviceProvider, items);
            results = new List<ValidationResult>();

            //Hp -->  Note: When validateAllProperties is false, 
            //only properties with a[Required] attribute are checked. 
            return (Validator.TryValidateObject(
                context.ObjectInstance, context, results, validateAllProperties));
        }

        /// <summary>
        /// Determines whether this instance is json string (or) not.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if the specified input is json; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJson(this string input)
        {
            input = input.Trim();
            Func<bool> isJsonStr = () =>
            {
                try
                {
                    JToken.Parse(input);
                }
                catch
                {
                    return false;
                }

                return true;
            };

            //Hp --> Note: The reason to add checks for { or [ etc was based on the fact that JToken.
            //Parse would parse the values such as "1234" or "'a string'" as a valid token. 
            //The other option could be to use both JObject.Parse and JArray.Parse in parsing and 
            //see if anyone of them succeeds, but I believe checking for {} and [] should be easier.
            return ((input.StartsWith("{") && input.EndsWith("}") //For object
                    || input.StartsWith("[") && input.EndsWith("]")) //For array
                    && isJsonStr());
        }

        /// <summary>
        /// Determines whether the specified target is equal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if the specified target is equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqualTo(this string source, string target,
            StringComparison comparer = StringComparison.CurrentCultureIgnoreCase)
        {
            Func<string, string> trimData = (data) => (null != data) ? data.Trim() : data;
            return (string.Equals(trimData(source), trimData(target), comparer));
        }

        /// <summary>
        /// Determines whether the specified source value is equal to target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcValue">The source value.</param>
        /// <param name="trgtValue">The target value.</param>
        /// <returns>
        ///   <c>true</c> if the specified source value is equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqual<T>(T srcValue, T trgtValue)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(srcValue, trgtValue))
            {
                return (true);
            }

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(srcValue, null) || ReferenceEquals(trgtValue, null))
            {
                return (false);
            }

            //Check whether the source value equal to target.
            return (srcValue.Equals(trgtValue));
        }

        /// <summary>
        /// Gets the equality comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEqualityComparer<T> GetEqualityComparer<T>()
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            if (typeof(T).IsValueType)
            {
                comparer = new CustomEqualityComparer<T>(item => item);
            }
            else if (typeof(string) == typeof(T))
            {
                comparer = (IEqualityComparer<T>)StringComparer.Create(
                    CultureInfo.CurrentUICulture, true);
            }
            else if (typeof(T).Implements<IEqualityComparer<T>>())
            {
                comparer = (IEqualityComparer<T>)Activator.CreateInstance<T>();
            }

            return (comparer);
        }

        /// <summary>
        /// Determines whether [contains] [the specified string to check].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="toCheck">To check.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified string to check]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, string toCheck, StringComparison comparer)
        {
            return (source != null && toCheck != null && source.IndexOf(toCheck, comparer) >= 0);
        }

        /// <summary>
        /// Enumerates for each item in collection by performing given action and yeild results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action?.Invoke(item);
                yield return (item);
            }
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection,
            Action<T, int> action)
        {
            var index = -1;
            foreach (var item in collection)
            {
                action?.Invoke(item, ++index);
                yield return (item);
            }
        }

        /// <summary>
        /// Sorts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortColumns">The sort property.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> source,
            IDictionary<string, ListSortDirection> sortColumns)
        {
            ErrorBase.CheckArgIsNull(sortColumns, () => sortColumns);

            var srcType = typeof(T);
            var index = 0;
            var query = source;

            foreach (var item in sortColumns)
            {
                var propInfo = srcType.GetProperties().SingleOrDefault(P => P.Name.IsEqualTo(item.Key));
                if (null == propInfo)
                {
                    //Hp --> If given column does not exists then skip.
                    continue;
                }

                var parameter = Expression.Parameter(srcType, "p");
                var propAccess = Expression.MakeMemberAccess(parameter, propInfo);
                var expression = Expression.Lambda(propAccess, parameter);
                var typeArguments = new Type[] { srcType, propInfo.PropertyType };
                var methodName = string.Empty;

                if (0 == index)
                {
                    methodName = (ListSortDirection.Ascending == item.Value) ?
                    nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
                }
                else
                {
                    methodName = (ListSortDirection.Ascending == item.Value) ?
                    nameof(Queryable.ThenBy) : nameof(Queryable.ThenByDescending);
                }

                var method = Expression.Call(typeof(Queryable), methodName, typeArguments,
                        query.Expression, Expression.Quote(expression));

                query = source.Provider.CreateQuery<T>(method);
                index++;
            }

            return ((IOrderedQueryable<T>)query);
        }

        /// <summary>
        /// Sorts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="sortColumns">The sort columns.</param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source,
            ListSortDirection sortOrder, params string[] sortColumns)
        {
            ErrorBase.CheckArgIsNull(sortColumns, () => sortColumns);
            return (source.SortBy(sortColumns.ToDictionary(C => C, C => sortOrder)));
        }

        /// <summary>
        /// Sorts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortNestedProperty">The sort nested property.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T, TResult>(this IQueryable<T> source,
            string sortNestedProperty,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");
            var sortKeys = sortNestedProperty.Split('.');

            var expression = sortKeys.Aggregate<string, Expression>(parameter, Expression.Property);
            var conversion = Expression.Convert(expression, typeof(object));

            var tryExpression = Expression.TryCatch(Expression.Block(typeof(object), conversion),
                Expression.Catch(typeof(object), Expression.Constant(null)));

            var predicate = Expression.Lambda<Func<T, TResult>>(tryExpression, parameter);
            return ((sortDirection == ListSortDirection.Ascending) ?
                source.OrderBy(predicate) : source.OrderByDescending(predicate));
        }

        /// <summary>
        /// Sorts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortNestedProperty">The sort nested property.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string sortNestedProperty,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            return (source.SortBy<T, object>(sortNestedProperty, sortDirection));
        }

        /// <summary>
        /// Filters the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, FilterGroup group)
            where T : class
        {
            ErrorBase.CheckArgIsNull(group, () => group);

            // Hp --> Logic: Convert given filter group to expreesion
            var expression = group.GetFilterExpression<T>();
            return (source.Where(expression));
        }

        /// <summary>
        /// Filters the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source,
            Expression<Func<T, bool>> expression) where T : class
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);

            // Hp --> Logic: Convert given filter group to expreesion
            return (source.Where(expression));
        }

        /// <summary>
        /// Removes by range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="group">The group.</param>
        /// <exception cref="System.NullReferenceException">body</exception>
        public static int RemoveAll<T>(this List<T> source, FilterGroup group) where T : class
        {
            ErrorBase.CheckArgIsNull(group, () => group);

            // Hp --> Logic: Convert given filter group to expreesion
            var expression = group.GetFilterExpression<T>();
            var predicate = expression.GetPredicate();
            return (source.RemoveAll(predicate));
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static int RemoveAll<T>(this List<T> source, Expression<Func<T, bool>> expression)
            where T : class
        {
            ErrorBase.CheckArgIsNull(expression, () => expression);

            // Hp --> Logic: Convert given filter group to expreesion
            return (source.RemoveAll(expression.GetPredicate()));
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static int RemoveAll<T>(this List<T> source, Predicate<T> predicate) where T : class
        {
            ErrorBase.CheckArgIsNull(predicate, () => predicate);

            // Hp --> Logic: Convert given filter group to expreesion
            return (source.RemoveAll(predicate));
        }

        /// <summary>
        /// Properties the descriptor.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static PropertyDescriptor PropertyDescriptor(this PropertyInfo propertyInfo)
        {
            return TypeDescriptor.GetProperties(propertyInfo.DeclaringType)[propertyInfo.Name];
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public static Expression<Func<T, object>> GetExpression<T>(string propName)
        {
            var srcType = typeof(T);

            //Hp --> Logic: Create E => portion of lambda expression
            var parameter = Expression.Parameter(srcType, "E");
            var propInfo = srcType.GetProperties()
                .SingleOrDefault(P => P.Name.IsEqualTo(propName));

            if (propInfo == null)
            {
                //Hp --> If given property name does not exists then throw exception.
                throw (new NullReferenceException(string.Join(Environment.NewLine,
                    $"{nameof(propInfo)} instance is null.",
                    $"[Reason]: Cannot find property with name \"{propName}\".")));
            }

            //Hp --> Logic: Create E.Id portion of lambda expression (right)
            var body = Expression.Property(parameter, propInfo.Name);
            var expression = Expression.Lambda<Func<T, object>>(body, new[] { parameter });
            return (expression);
        }

        /// <summary>
        /// Converts given expression to predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Predicate<T> GetPredicate<T>(this Expression<Func<T, bool>> expression)
        {
            return (new Predicate<T>(expression?.Compile() ?? null));
        }

        /// <summary>
        /// Determines whether [is having audit columns] [the specified audit column type].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcObject">The source.</param>
        /// <param name="auditColumnType">Type of the audit column.</param>
        /// <returns>
        ///   <c>true</c> if [is having audit columns] [the specified audit column type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHavingAuditColumns(this object srcObject,
            AuditColumnTypes auditColumnType = AuditColumnTypes.All)
        {
            return (IsHavingAuditColumns(srcObject.GetType(), auditColumnType));
        }

        /// <summary>
        /// Determines whether [is having audit columns] [the specified source object type].
        /// </summary>
        /// <param name="srcObjectType">Type of the source object.</param>
        /// <param name="auditColumnType">Type of the audit column.</param>
        /// <returns>
        ///   <c>true</c> if [is having audit columns] [the specified source object type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHavingAuditColumns(this Type srcObjectType,
            AuditColumnTypes auditColumnType = AuditColumnTypes.All)
        {
            ErrorBase.CheckArgIsNull(srcObjectType, () => srcObjectType);
            ErrorBase.CheckArgIsValid(auditColumnType, () => auditColumnType,
                T => (AuditColumnTypes.None != T));

            var comparer = new CustomEqualityComparer<string>(A => A.ToLower().Trim());
            var delimeter = new string[] { "," };
            var srcObjColumns = srcObjectType.GetAllProperties().Select(P => P.Name).ToList();

            return (GetEnumDescription(auditColumnType)
                .Split(delimeter, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .TrueForAll(A => srcObjColumns.Contains(A, comparer)));
        }

        /// <summary>
        /// Gets the months difference.
        /// </summary>
        /// <param name="srcDate">The source date.</param>
        /// <param name="trgtDate">The target date.</param>
        /// <returns></returns>
        public static int GetMonthsDiff(this DateTime srcDate, DateTime trgtDate)
        {
            return (Math.Abs(((srcDate.Year - trgtDate.Year) * 12) +
                (srcDate.Month - trgtDate.Month)));
        }

        /// <summary>
        /// To the amount.
        /// </summary>
        /// <param name="currencyStr">The currency string.</param>
        /// <returns></returns>
        public static decimal ToAmount(this string currencyStr) =>
            decimal.Parse(currencyStr ?? string.Empty, NumberStyles.Currency);

        /// <summary>
        /// To the curreny "£".
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static string ToCurreny(this decimal amount) => string.Format("{0:C}", amount);

        /// <summary>
        /// To the currency precision "£".
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static string ToCurrencyPrecision(this decimal amount, int precision = 2) =>
            ToCurreny(ToPrecision(amount, precision));

        /// <summary>
        /// To the precision2 "F2".
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static string ToPrecision2(this decimal amount) => amount.ToString("F2");

        /// <summary>
        /// To the precision.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static decimal ToPrecision(this decimal amount, int precision = 2) =>
            Math.Round(amount, precision, MidpointRounding.AwayFromZero);

        /// To the SQL short date "yyyyMMdd".
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns></returns>
        public static string ToSQLShortDate(this DateTime date, string delimeter = "") =>
            date.ToString($"yyyy{delimeter}MM{delimeter}dd");

        /// <summary>
        /// To the UK short date "dd/MM/yyyy".
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns></returns>
        public static string ToUKShortDate(this DateTime date, string delimeter = "/") =>
            date.ToString($"dd{delimeter}MM{delimeter}yyyy");

        /// <summary>
        /// To the UK full english date "dd MMMM yyyy".
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string ToUKFullEnuDate(this DateTime date, string delimeter = " ") =>
            date.ToString($"dd{delimeter}MMMM{delimeter}yyyy");

        /// <summary>
        /// To the UK short english date "dd-MMM-yyyy".
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns></returns>
        public static string ToUKShortEnuDate(this DateTime date, string delimeter = "-") =>
            date.ToString($"dd{delimeter}MMM{delimeter}yyyy");

        /// <summary>
        /// To the short year "yy".
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string ToShortYear(this DateTime date) => date.ToString("yy");

        /// <summary>
        /// To the long year.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string ToLongYear(this DateTime date) => date.ToString("yyyy");

        /// <summary>
        /// Converts To the Odata date in url.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string ToODataDate(this DateTime date) => $"datetime'{date.ToString("s")}'";

        /// <summary>
        /// To the OData string in url.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ToODataStr(this string source) => $"'{source}'";

        /// <summary>
        /// To the OData boolean value in url.
        /// </summary>
        /// <param name="source">if set to <c>true</c> [source].</param>
        /// <returns></returns>
        public static string ToODataBool(this bool source) => $"{source.ToString().ToLower()}";

        /// <summary>
        /// To the OData boolean value in url.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int ToODataBool(this bool? source) =>
            (source == null) ? -1 : ToInt(source.Value);

        /// <summary>
        /// Gets the json date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ToJsonDate(string dateTime)
        {
            ErrorBase.CheckArgIsNullOrDefault(dateTime, () => dateTime);

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentDate = DateTime.Parse(dateTime);
            return ((currentDate - unixEpoch).Ticks / TimeSpan.TicksPerMillisecond);
        }

        /// <summary>
        /// To the Odata URL encode.
        /// </summary>
        /// <param name="srcData">The source data.</param>
        /// <returns></returns>
        public static string ToODataUrlEncode(this string srcData) =>
            HttpUtility.UrlEncode((srcData?.Replace("'", "''") ?? string.Empty).Trim());

        /// <summary>
        /// Implementses the specified type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Only interfaces can be implemented.</exception>
        public static bool Implements<TInterface>(this Type type) where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
            {
                throw (new InvalidOperationException(
                    "The given type does not implements provided interface."));
            }

            return (interfaceType.IsAssignableFrom(type));
        }

        /// <summary>
        /// Nexts the day of the week.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <param name="occurenceOfWeek">The occurence of a week.</param>
        /// <returns></returns>
        public static DateTime NextDay(this DateTime from, DayOfWeek dayOfWeek, int occurenceOfWeek = 1)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
            {
                target += 7;
            }

            //Hp --> Logic: Get number of days to add based on next occurence of given day of a week
            var daysToAdd = target - start + (7 * (occurenceOfWeek <= 1 ? 0 : (occurenceOfWeek - 1)));
            return from.AddDays(daysToAdd);
        }

        /// <summary>
        /// To the title case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ToTitleCase(this string source) =>
            CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(source);

        /// <summary>
        /// To the camel case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ToCamelCase(this string source)
        {
            if ((string.IsNullOrWhiteSpace(source)) || (char.IsLower(source, 0)))
            {
                return (source);
            }

            var subStr = (source.Length <= 1) ? string.Empty : source.Substring(1);
            return (string.Concat(source[0].ToString().ToLowerInvariant(), subStr).Trim());
        }

        /// <summary>
        /// To the snake case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ToSnakeCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return (source);
            }

            //PascalCase to snake_case
            return (string.Concat(source.Select((x, i) => i > 0 && char.IsUpper(x) ?
            "_" + char.ToLower(x).ToString() : x.ToString())).Trim());
        }

        /// <summary>
        /// Gets a value indicating whether this instance is design mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDesignMode
        {
            get
            {
                bool modeCheck = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
                bool procCheck = false;

                var designerProcessNames = new[] { "xdesproc", "devenv" };
                using (var process = Process.GetCurrentProcess())
                {
                    procCheck = designerProcessNames.Contains(process.ProcessName.ToLower());
                }

                return (procCheck || modeCheck);
            }
        }

        /// <summary>
        /// To the page data base on given dynamic linq filter expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static PaginationData<T> ToPageData<T>(this ICollection<T> list, int skip, int take,
            DynamicLinqFilter filter = null) where T : class
        {
            ErrorBase.CheckArgIsValid(skip, nameof(skip), index => (0 <= index));
            ErrorBase.CheckArgIsValid(take, nameof(take), size => (0 < size));

            var data = list.AsQueryable();
            var filterData = (null == filter) ? data : data.Where(filter.Expression,
                filter.Parameters ?? new object[] { });

            return (new PaginationData<T>
            {
                TotalItems = filterData.Count(),
                PageData = filterData.Skip(skip).Take(take).ToList()
            });
        }

        /// <summary>
        /// Executes the given function block asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeBlock">The code block.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <param name="errMessage">The error message.</param>
        /// <returns></returns>
        public static Task<T> ExecuteAsync<T>(this Func<T> codeBlock, bool isIgnoreError = false,
            string errMessage = "Failed to execute given function block asynchronously!")
        {
            return (Task.Run(codeBlock)
            .ContinueWith((antecedent) =>
            {
                if ((!isIgnoreError) && (antecedent.Status != TaskStatus.RanToCompletion))
                {
                    var customMessage = $"{errMessage} in method {nameof(ExecuteAsync)}";
                    var innerExceptions =
                    antecedent.Exception?.InnerExceptions?.ToList() ?? new List<Exception>();

                    throw (new AggregateException(customMessage, innerExceptions));
                }

                return (antecedent.Result);
            }, CancellationToken.None));
        }

        /// <summary>
        /// Executes the given action block asynchronously.
        /// </summary>
        /// <param name="codeBlock">The code block.</param>
        /// <param name="isIgnoreError">if set to <c>true</c> [is ignore error].</param>
        /// <param name="errMessage">The error message.</param>
        /// <returns></returns>
        public static Task ExecuteAsync(this Action codeBlock, bool isIgnoreError = false,
            string errMessage = "Failed to execute given action block asynchronously!")
        {
            return (Task.Run(codeBlock)
            .ContinueWith((antecedent) =>
            {
                if ((!isIgnoreError) && (antecedent.Status != TaskStatus.RanToCompletion))
                {
                    var customMessage = $"{errMessage} in method {nameof(ExecuteAsync)}";
                    var innerExceptions =
                    antecedent.Exception?.InnerExceptions?.ToList() ?? new List<Exception>();

                    throw (new AggregateException(customMessage, innerExceptions));
                }

                return (Task.CompletedTask);
            }, CancellationToken.None));
        }

        /// <summary>
        /// Gets the completed task.
        /// </summary>
        /// <value>
        /// The completed task.
        /// </value>
        /// <remarks>
        /// Workaround for Task.CompletedTask in .Net 4.6
        /// </remarks>
        public static Task CompletedTask { get; } = Task.FromResult(false);

        /// <summary>
        /// To the bool.
        /// </summary>
        /// <param name="sourceEnumType">Type of the source enum.</param>
        /// <returns></returns>
        public static bool? ToBool(this ThreeStateOptionTypes sourceEnumType)
        {
            var value = default(bool?);
            switch (sourceEnumType)
            {
                case ThreeStateOptionTypes.None:
                    {
                        value = null;
                    }
                    break;

                case ThreeStateOptionTypes.No:
                    {
                        value = false;
                    }
                    break;

                case ThreeStateOptionTypes.Yes:
                    {
                        value = true;
                    }
                    break;

                default:
                    {
                        var errMessage = $"The given {sourceEnumType} is not handled!";
                        throw (new NotImplementedException(errMessage));
                    }
            }

            return (value);
        }

        /// <summary>
        /// To the type of the three state option.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <returns></returns>
        public static ThreeStateOptionTypes ToThreeStateOptionType(this bool? sourceValue)
        {
            var optionType = ThreeStateOptionTypes.None;
            switch (sourceValue)
            {
                case null:
                    {
                        optionType = ThreeStateOptionTypes.None;
                    }
                    break;

                case false:
                    {
                        optionType = ThreeStateOptionTypes.No;
                    }
                    break;

                case true:
                    {
                        optionType = ThreeStateOptionTypes.Yes;
                    }
                    break;

                default:
                    {
                        //Hp --> Do nothing
                    }
                    break;
            }

            return (optionType);
        }

        /// <summary>
        /// Gets the base URL path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetBaseUrlPath(this HttpRequestBase request) =>
            $"{request.Url.Scheme}://{request.Url.Authority}";

        /// <summary>
        /// Gets all URL query parms.
        /// </summary>
        /// <param name="queryUrlPath">The query URL path.</param>
        /// <returns></returns>
        /// <remarks>Usage: Request.Url.Query.GetAllUrlQueryParms();</remarks>
        public static IDictionary<string, string> GetAllUrlQueryParms(this string queryUrlPath) =>
            HttpUtility.ParseQueryString(queryUrlPath).ToDictionary();

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection) =>
            collection.AllKeys.ToDictionary(K => K, K => collection[K]);
    }
}
