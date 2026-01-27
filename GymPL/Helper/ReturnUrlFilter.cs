using Microsoft.AspNetCore.Mvc.Filters;

public class ReturnUrlFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Don't store returnUrl for these actions (to avoid loops)
        var excludedActions = new[] { "Login", "Logout", "Error" };
        var actionName = context.RouteData.Values["action"]?.ToString();
        
        if (!excludedActions.Contains(actionName))
        {
            // Store current URL as the "previous page"
            var currentUrl = $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
            context.HttpContext.Session.SetString("LastVisitedUrl", currentUrl);
        }
        
        base.OnActionExecuting(context);
    }
}
