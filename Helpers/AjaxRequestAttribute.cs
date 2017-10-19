using System;
using System.Reflection;
using System.Web.Mvc;

namespace Ducksoft.Soa.Common.Helpers
{
    /// <summary>
    /// Custom attribute to restrict MVC action only by Ajax call.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AjaxRequestAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// Determines whether the action method selection is valid for the specified controller context.
        /// </summary>
        /// <param name="context">The controller context.</param>
        /// <param name="methodInfo">Information about the action method.</param>
        /// <returns>
        /// true if the action method selection is valid for the specified controller context; otherwise, false.
        /// </returns>
        public override bool IsValidForRequest(ControllerContext context, MethodInfo methodInfo)
        {
            return (context.HttpContext.Request.IsAjaxRequest());
        }
    }
}
