using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Static class which is used to validate a method arguments are valid (or) not.
    /// </summary>
    public static class ErrorBase
    {
        /// <summary>
        /// Checks the argument is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argName">Name of the argument.</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void CheckArgIsNull<T>(T methodArgument, string argName,
            string errMessage = "The parameter value is null!")
        {
            Predicate<T> validationCriteria = objArg => (objArg != null);
            if (!validationCriteria?.Invoke(methodArgument) ?? false)
            {
                throw (new ArgumentNullException(argName, errMessage));
            }
        }

        /// <summary>
        /// Checks the argument is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argExpression">The argument expression.(i.e., () =&gt; methodArgument)</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException">If given method argument is null.</exception>
        public static void CheckArgIsNull<T>(T methodArgument, Expression<Func<T>> argExpression,
            string errMessage = "The parameter value is null!") =>
            CheckArgIsNull(methodArgument, GetArgumentName(argExpression), errMessage);

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argExpression">The argument expression.</param>
        /// <returns></returns>
        private static string GetArgumentName<T>(Expression<Func<T>> argExpression)
        {
            var expression = (MemberExpression)argExpression?.Body ?? null;
            return ((expression == null) ? string.Empty : expression.Member.Name);
        }

        /// <summary>
        /// Checks the argument is null or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argName">Name of the argument.</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void CheckArgIsNullOrDefault<T>(T methodArgument, string argName,
            string errMessage = "The parameter value is either null (or) contains default value(s)!")
        {
            Predicate<T> validationCriteria = null;
            if (methodArgument == null)
            {
                validationCriteria = objArg => (objArg != null);
            }
            else if (methodArgument.GetType().IsValueType)
            {
                //Note: Hp --> Ignore, if argument is value type (eg., int, double, float, datetime, ...)
                //validationCriteria = new Predicate<T>((objArg) =>
                //    (!EqualityComparer<T>.Default.Equals(objArg, default(T))));
            }
            else if (methodArgument is string)
            {
                validationCriteria = strArg => (!string.IsNullOrWhiteSpace(strArg as string));
            }
            else if (methodArgument is IList)
            {
                validationCriteria = listArg =>
                {
                    var listObj = listArg as IList;

                    return (((listObj?.Count ?? -1) > 0) &&
                    (listObj.Cast<object>().All(item => (item is string) ?
                    (!string.IsNullOrWhiteSpace((string)item)) : (item != null))));
                };
            }
            else
            {
                validationCriteria = objArg => (objArg != null);
            }

            if (!validationCriteria?.Invoke(methodArgument) ?? false)
            {
                throw (new ArgumentNullException(argName, errMessage));
            }
        }

        /// <summary>
        /// Checks the argument is null or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argExpression">The argument expression.(i.e., () =&gt; methodArgument)</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException">If given method argument is null (or) contains default value(s).</exception>
        public static void CheckArgIsNullOrDefault<T>(T methodArgument,
            Expression<Func<T>> argExpression,
            string errMessage = "The parameter value is either null (or) contains default value(s)!")
            => CheckArgIsNullOrDefault(methodArgument, GetArgumentName(argExpression), errMessage);

        /// <summary>
        /// Checks the argument is valid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argName">Name of the argument.</param>
        /// <param name="validationCriteria">The validation criteria.</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void CheckArgIsValid<T>(T methodArgument, string argName,
            Predicate<T> validationCriteria,
            string errMessage = "The given parameter fails against validation criteria!")
        {
            //Hp --> Note: First always check whether method argument is null (or) not because
            //we don't know in validation criteria whether user is checking for null (or) not.
            bool isCheckForNull = false;
            if ((methodArgument == null) || (validationCriteria == null))
            {
                //Hp --> Logic: If method argument is null, then overwrite validation criteria with check for null.
                //Similarly if user don't provide validation criteria then by default check for null.
                isCheckForNull = true;
                errMessage = "The parameter value is null!";
                validationCriteria = objArg => (objArg != null);
            }

            if (!validationCriteria?.Invoke(methodArgument) ?? false)
            {
                throw ((isCheckForNull) ?
                    new ArgumentNullException(argName, errMessage) :
                    new ArgumentException(errMessage, argName));
            }
        }

        /// <summary>
        /// Checks the argument is valid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodArgument">The method argument.</param>
        /// <param name="argExpression">The argument expression.(i.e., () =&gt; methodArgument)</param>
        /// <param name="validationCriteria">The validation criteria.</param>
        /// <param name="errMessage">The error message.</param>
        /// <exception cref="System.ArgumentNullException">If given method argument is null.</exception>
        /// <exception cref="System.ArgumentException">If given method argument, fails against validation criteria.</exception>
        public static void CheckArgIsValid<T>(T methodArgument, Expression<Func<T>> argExpression,
            Predicate<T> validationCriteria,
            string errMessage = "The given parameter fails against validation criteria!") =>
            CheckArgIsValid(methodArgument, GetArgumentName(argExpression),
                validationCriteria, errMessage);

        /// <summary>
        /// Function checks the given argument for null
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTest">The arg to test.</param>
        public static void Require(object argToTest) => Require((argToTest != null));

        /// <summary>
        /// Function checks the given argument for null or empty
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTest">The arg to test.</param>
        public static void Require(string argToTest) =>
            Require(!string.IsNullOrWhiteSpace(argToTest));

        /// <summary>
        /// Function checks the given argument condition
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTestCondition">if set to <c>true</c> [arg to test condition].</param>
        public static void Require(bool argToTestCondition) =>
            Require(argToTestCondition, "Assert validation failed!");

        /// <summary>
        /// Function checks the given argument is null
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTest">The arg to test.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void Require(object argToTest, string errorMessage) =>
            Require((argToTest != null), errorMessage);

        /// <summary>
        /// Function checks the given argument for null or empty
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTest">The arg to test.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void Require(string argToTest, string errorMessage) =>
            Require((!string.IsNullOrWhiteSpace(argToTest)), errorMessage);

        /// <summary>
        /// Function checks the given argument condition
        /// In DebugMode:
        /// If there is a debugger attached then automatically executes debug break else assert will be shown
        /// In Release mode:
        /// Logs the current stack throws ArgumentException exception
        /// </summary>
        /// <param name="argToTestCondition">if set to <c>true</c> [arg to test condition].</param>
        /// <param name="errorMessage">The error message.</param>
        public static void Require(bool argToTestCondition, string errorMessage)
        {
            if (argToTestCondition)
            {
                return;
            }

            var argException = new ArgumentException(errorMessage);
            //Dump the stack trace
            //WriteExceptionToLogFile(argException);
#if DEBUG
            if (Debugger.IsAttached)
            {
                //Debug.Assert(argToTestCondition, errorMessage);
                Debugger.Break();
            }
            else
            {
                Debug.Assert(argToTestCondition, argException.Message);
            }
#else
            //Hp --> If we have implementation for catching unhandled exceptions at top level then 
            //we should just throw exception.                
            throw (argException);
#endif
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void HandleException(Exception exception)
        {
            HandleException(string.Empty, exception, false);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="skipUIMessageNotification">if set to <c>true</c> [skip UI message notification].</param>
        public static void HandleException(Exception exception, bool skipUIMessageNotification)
        {
            HandleException(string.Empty, exception, skipUIMessageNotification);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="defaultMessage">The default message.</param>
        /// <param name="exception">The exception.</param>
        public static void HandleException(string defaultMessage, Exception exception)
        {
            HandleException(defaultMessage, exception, false);
        }

        /// <summary>
        /// Updates the error exception to log file and displays a messagebox with user specified error message
        /// </summary>
        /// <param name="defaultMessage">The default message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="skipUIMessageNotification">if set to <c>true</c> [skip UI message notification].</param>
        public static void HandleException(string defaultMessage, Exception exception,
            bool skipUIMessageNotification)
        {
            try
            {
                //Ask user to restart Application
                bool seriousError = exception is AccessViolationException;

                //Break if we are debugging the code
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                Exception actualException = exception;

                actualException.WriteToLog();
                while (null != exception.InnerException)
                {
                    exception = exception.InnerException;
                    seriousError |= (exception is AccessViolationException);
                }

                var appName = Utility.AppName;
                var myErrMessage = new StringBuilder();
                myErrMessage.Append(defaultMessage);
                myErrMessage.Append(Environment.NewLine);

                if (typeof(IOException).IsInstanceOfType(exception)) //IO errors
                {
                    myErrMessage.Append(exception.Message + Environment.NewLine + defaultMessage);
                }
                else if (typeof(OutOfMemoryException).IsInstanceOfType(exception) ||
                    (exception.Message.IndexOf("Out of memory") > 0))
                {
                    myErrMessage.Append("System is running on very less virtual memory." +
                        Environment.NewLine + "Please close all unused programs, restart " +
                        appName + " and try again.");
                }
                else if (exception is ExceptionBase)
                {
                    myErrMessage.Append(exception.Message);
                }
                else if (typeof(IOException).IsInstanceOfType(actualException)) //IO errors
                {
                    myErrMessage.Append(actualException.Message + Environment.NewLine +
                        defaultMessage);
                }
                else if (typeof(OutOfMemoryException).IsInstanceOfType(actualException) ||
                    (actualException.Message.IndexOf("Out of memory") > 0))
                {
                    myErrMessage.Append("System is running on very less virtual memory." +
                        Environment.NewLine + "Please close all unused programs, restart " +
                        appName + " and try again.");
                }
                else if (actualException is ExceptionBase)
                {
                    myErrMessage.Append(actualException.Message);
                }
                else
                {
                    myErrMessage.Append(exception.Message);
                }

                if (seriousError)
                {
                    myErrMessage.Append(Environment.NewLine + appName +
                        " has recovered from a serious error." + Environment.NewLine +
                        "Please save all your work and restart the application.");
                }
#if DEBUG
                myErrMessage.Append(Environment.NewLine + exception.Message + Environment.NewLine +
                    actualException.ToString());
#endif

                if (!skipUIMessageNotification)
                {
                    //Display message to user
#if DEBUG
                    Utility.ShowErrorDialog((myErrMessage.ToString() + Environment.NewLine +
                        exception.ToString()), null);
#else
                    Utility.ShowErrorDialog(myErrMessage.ToString(), null);
#endif
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("HandleException:" + " " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handling an unhandled exception that propagates all the way to the top of your application.
        /// </summary>
        /// <param name="unHandledException">The unhandled exception.</param>
        public static void UnhandledException(Exception unHandledException)
        {
            //Require(null != unHandledException); //Not required down we are handling
            var result = DialogResult.Abort;
            try
            {
                if (null == unHandledException)
                {
                    unHandledException = new Exception("UnhandledException");
                }

                unHandledException.WriteToLog();

                string ErrorMessage = "UHE occured!" + Environment.NewLine
                  + "Log file is placed in following location: " + Environment.NewLine
                  + LogHelper.LogFilePath + Environment.NewLine
                  + "Select Abort --> To close the application " + Environment.NewLine
                  + "Select Retry/Ignore --> To allow the application to continue" +
                  Environment.NewLine;
#if DEBUG
                ErrorMessage += Environment.NewLine + unHandledException.Message +
                    Environment.NewLine + unHandledException.StackTrace;
#endif
                result = Utility.ShowFatalErrorDialog(ErrorMessage, null);
            }
            finally
            {
                if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }
#if DEBUG
                else if (result == DialogResult.Retry)
                {
                    Debugger.Break();
                }
#endif
            }
        }
    }
}
