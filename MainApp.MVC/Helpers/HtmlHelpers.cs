using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace MainApp.MVC.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsActive(this IHtmlHelper html, string? controllers = null, string? actions = null, string cssClass = "active")
        {
            RouteData? routeData = html.ViewContext.RouteData;
            string? currentController = (routeData.Values["controller"]?.ToString()) ?? string.Empty;
            string? currentAction = (routeData.Values["action"]?.ToString()) ?? string.Empty;

            string[]? acceptedActions = (actions ?? currentAction).Split(',');
            string[]? acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : string.Empty;
        }
    }
}