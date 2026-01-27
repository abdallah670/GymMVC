public static class UrlHelperExtensions
{
    public static string GetReturnUrl(this IUrlHelper urlHelper, HttpContext context)
    {
        // Try to get from query string first
        var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(returnUrl))
        {
            // Try to get from session (last visited page)
            returnUrl = context.Session.GetString("LastVisitedUrl");
        }
        
        if (string.IsNullOrEmpty(returnUrl))
        {
            // Default to home
            returnUrl = urlHelper.Action("Index", "Home");
        }
        
        return returnUrl;
    }
    
    public static string GetBackUrl(this IUrlHelper urlHelper, HttpContext context)
    {
        var returnUrl = urlHelper.GetReturnUrl(context);
        
        // If current page is the same as returnUrl, go to Index
        var currentUrl = $"{context.Request.Path}{context.Request.QueryString}";
        if (currentUrl == returnUrl)
        {
            returnUrl = urlHelper.Action("Index", "Home");
        }
        
        return returnUrl;
    }
}
