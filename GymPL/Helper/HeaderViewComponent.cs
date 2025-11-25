public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return View("~/Views/Shared/_Header.cshtml");
        }

        // Check user role from claims
        var role = User.IsInRole("Trainer") ? "Trainer" : User.IsInRole("Member") ? "Member" : "Public";

        return role switch
        {
            "Trainer" => View("~/Views/Shared/_TrainerHeader.cshtml"),
            "Member" => View("~/Views/Shared/_MemberHeader.cshtml"),
            "Public"=> View("~/Views/Shared/_Header.cshtml"),
            _ => View("~/Views/Shared/_Header.cshtml")
        };
    }
}