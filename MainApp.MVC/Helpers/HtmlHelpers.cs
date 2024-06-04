using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace MainApp.MVC.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsActive(this IHtmlHelper html, string controllers = null, string actions = null, string cssClass = "active")
        {
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            var currentController = (string)html.ViewContext.RouteData.Values["controller"];

            var acceptedActions = (actions ?? currentAction).Split(',');
            var acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : string.Empty;
        }
    }
}