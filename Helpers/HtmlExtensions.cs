using Ducksoft.Soa.Common.Utilities;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// Static class which holds the html extension methods used in razor view.
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Absolutes the route.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <returns></returns>
        public static string AbsoluteRoute(this UrlHelper url, string routeName)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.RouteUrl(routeName, null, requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the route.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteRoute(this UrlHelper url, string routeName, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.RouteUrl(routeName, new RouteValueDictionary(routeValues),
                requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the route.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteRoute(this UrlHelper url, string routeName,
            RouteValueDictionary routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.RouteUrl(routeName, routeValues, requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the route.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns></returns>
        public static string AbsoluteRoute(this UrlHelper url, string routeName,
            object routeValues, string protocol)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.RouteUrl(routeName, new RouteValueDictionary(routeValues), protocol, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, null, null, requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, null, new RouteValueDictionary(routeValues),
                requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            RouteValueDictionary routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, null, routeValues, requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            string controllerName)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, controllerName, (RouteValueDictionary)null,
                requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            string controllerName, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, controllerName, new RouteValueDictionary(routeValues),
                requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            string controllerName, RouteValueDictionary routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, controllerName, routeValues, requestUrl.Scheme, null));
        }

        /// <summary>
        /// Absolutes the action.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns></returns>
        public static string AbsoluteAction(this UrlHelper url, string actionName,
            string controllerName, object routeValues, string protocol)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return (url.Action(actionName, controllerName, new RouteValueDictionary(routeValues),
                protocol, null));
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static Uri GetBaseUrl(this UrlHelper url)
        {
            Uri contextUri = new Uri(url.RequestContext.HttpContext.Request.Url,
                url.RequestContext.HttpContext.Request.RawUrl);

            UriBuilder realmUri = new UriBuilder(contextUri)
            {
                Path = url.RequestContext.HttpContext.Request.ApplicationPath,
                Query = null,
                Fragment = null
            };

            return (realmUri.Uri);
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static string GetController(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            return ((routeValues.ContainsKey("controller")) ?
                routeValues["controller"].ToString() : string.Empty);
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static string GetAction(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            return ((routeValues.ContainsKey("action")) ?
                routeValues["action"].ToString() : string.Empty);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static string GetId(this HtmlHelper htmlHelper)
        {
            var myId = string.Empty;
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues.ContainsKey("id"))
            {
                myId = routeValues["id"].ToString();
            }
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
            {
                myId = HttpContext.Current.Request.QueryString["id"];
            }

            return (myId);
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static string GetRoutePrefix(this HtmlHelper htmlHelper)
        {
            // Get the controller type
            var controllerType = htmlHelper.ViewContext.Controller.GetType();
            return (GetRoutePrefix(controllerType));
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static string GetController(this Controller controller)
        {
            var routeValues = controller.RouteData.Values;
            return ((routeValues.ContainsKey("controller")) ?
                routeValues["controller"].ToString() : string.Empty);
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static string GetAction(this Controller controller)
        {
            var routeValues = controller.RouteData.Values;
            return ((routeValues.ContainsKey("action")) ?
                routeValues["action"].ToString() : string.Empty);
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static string GetRoutePrefix(this Controller controller)
        {
            // Get the controller type
            var controllerType = controller.GetType();
            return (GetRoutePrefix(controllerType));
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns></returns>
        public static string GetRoutePrefix(this ControllerContext controllerContext)
        {
            // Get the controller type
            var controllerType = controllerContext.Controller.GetType();
            return (GetRoutePrefix(controllerType));
        }

        /// <summary>
        /// Gets the route prefix.
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns></returns>
        private static string GetRoutePrefix(Type controllerType)
        {
            // Get the RoutePrefix Attribute
            var myAttribute = (RoutePrefixAttribute)Attribute.GetCustomAttribute(
                controllerType, typeof(RoutePrefixAttribute));

            // Return the Template that is defined
            return (((null != myAttribute) && (!string.IsNullOrWhiteSpace(myAttribute.Prefix))) ?
                myAttribute.Prefix : string.Empty);
        }

        /// <summary>
        /// Gets the route template.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static string GetRouteTemplate(this Controller controller,
            [CallerMemberName]string actionName = "")
        {
            // Get the controller type
            var controllerType = controller.GetType();

            // Return the Template that is defined
            return (GetRouteTemplate(controllerType, actionName));
        }

        /// <summary>
        /// Gets the route template.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns></returns>
        public static string GetRouteTemplate(this ControllerContext controllerContext)
        {
            // Get the controller type
            var controllerType = controllerContext.Controller.GetType();

            // Get the action name
            var actionName = ((Controller)controllerContext.Controller).GetAction();

            // Return the Template that is defined
            return (GetRouteTemplate(controllerType, actionName));
        }

        /// <summary>
        /// Gets the route template.
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        private static string GetRouteTemplate(Type controllerType, string actionName)
        {
            // Get the Route Attribute
            var myAttribute = Utility.GetMethodAttribute<RouteAttribute>(controllerType, actionName);

            // Return the Template that is defined
            return (((null != myAttribute) && (!string.IsNullOrWhiteSpace(myAttribute.Template))) ?
                myAttribute.Template : string.Empty);
        }

        /// <summary>
        /// Gets the name of the route.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        public static string GetRouteName(this Controller controller,
           [CallerMemberName]string actionName = "")
        {
            // Get the controller type
            var controllerType = controller.GetType();

            // Get the Route Attribute
            var myAttribute = Utility.GetMethodAttribute<RouteAttribute>(controllerType, actionName);

            // Return the Name that is defined
            return (((null != myAttribute) && (!string.IsNullOrWhiteSpace(myAttribute.Name))) ?
                myAttribute.Name : string.Empty);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static string GetId(this Controller controller)
        {
            var routeValues = controller.RouteData.Values;
            return ((routeValues.ContainsKey("id")) ?
                routeValues["id"].ToString() : string.Empty);
        }

        /// <summary>
        /// Attributes the specified name.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example: 
        /// Usage: <li @(Html.Attr("class", "new", () => example.isNew))>...</li>
        /// Result: <li class="new">...</li>
        /// </remarks>
        public static MvcHtmlString Attr(this HtmlHelper helper, string name, string value,
            Func<bool> condition = null)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
            {
                return (MvcHtmlString.Empty);
            }

            var render = (null != condition) ? condition() : true;
            return (render ? new MvcHtmlString(string.Format("{0}=\"{1}\"", name,
                HttpUtility.HtmlAttributeEncode(value))) : MvcHtmlString.Empty);
        }

        /// <summary>
        /// Sets the URL parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SetUrlParameter(this string url, string paramName, string value)
        {
            return (new Uri(url).SetUrlParameter(paramName, value).ToString());
        }

        /// <summary>
        /// Sets the URL parameter.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Uri SetUrlParameter(this Uri url, string paramName, string value)
        {
            var queryParts = HttpUtility.ParseQueryString(url.Query);
            queryParts[paramName] = value;
            return (new Uri(url.AbsoluteUriExcludingQuery() + '?' + queryParts.ToString()));
        }

        /// <summary>
        /// Absolutes the URI excluding query.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string AbsoluteUriExcludingQuery(this Uri url)
        {
            return (url.AbsoluteUri.Split('?').FirstOrDefault() ?? String.Empty);
        }

        /// <summary>
        /// Renders the view as string.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controller">Controller to extend</param>
        /// <param name="viewPath">(Partial) view to render</param>
        /// <param name="isPartial">if set to <c>true</c> [is partial].</param>
        /// <param name="model">Model</param>
        /// <returns></returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <remarks>
        /// Usage: return (Json(this.RenderViewAsString(...), JsonRequestBehavior.AllowGet));
        /// </remarks>
        public static string RenderViewAsString<TModel>(this Controller controller,
            string viewPath = "", bool isPartial = true, TModel model = null) where TModel : class
        {
            var view = controller.GetView(viewPath, isPartial, model);
            var result = string.Empty;
            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(controller.ControllerContext, view,
                    controller.ViewData, controller.TempData, writer);

                view.Render(viewContext, writer);
                result = writer.GetStringBuilder().ToString();
            }

            return (result);
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controller">Controller to extend</param>
        /// <param name="viewPath">(Partial) view to render</param>
        /// <param name="isPartial">if set to <c>true</c> [is partial].</param>
        /// <param name="model">Model</param>
        /// <returns>
        /// Rendered (partial) view as string
        /// </returns>
        /// <exception cref="ExceptionBase"></exception>
        /// <remarks>
        /// Usage: return (PartialView(this.ToView(...));
        /// </remarks>
        public static IView GetView<TModel>(this Controller controller, string viewPath = "",
            bool isPartial = true, TModel model = null) where TModel : class
        {
            var context = controller.ControllerContext;
            if (string.IsNullOrEmpty(viewPath))
            {
                viewPath = context.RouteData.GetRequiredString("action");
            }

            controller.ViewData.Model = model;
            ViewEngineResult viewResult = null;
            if (isPartial)
            {
                viewResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            }
            else
            {
                viewResult = ViewEngines.Engines.FindView(context, viewPath, null);
            }

            if (null == viewResult)
            {
                throw (new ExceptionBase(string.Format(CultureInfo.CurrentUICulture,
                    "View: '{0}' cannot be found.", viewPath)));
            }

            return (viewResult.View);
        }

        /// <summary>
        /// To the normal amount.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="currencyStr">The currency string.</param>
        /// <returns></returns>
        public static string ToAmount(this HtmlHelper htmlHelper, string currencyStr)
        {
            var amount = currencyStr;
            try
            {
                amount = decimal.Parse(currencyStr, NumberStyles.Currency).ToString();
            }
            catch
            {
                //Do Nothing
            }

            return (amount);
        }
    }
}