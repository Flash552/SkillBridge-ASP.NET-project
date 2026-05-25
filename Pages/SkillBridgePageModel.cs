using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWebApp.Pages;

public abstract class SkillBridgePageModel : PageModel
{
    protected int? CurrentUserId => HttpContext.Session.GetInt32("UserID");
    protected string? CurrentRole => HttpContext.Session.GetString("Role");
    protected string? CurrentUsername => HttpContext.Session.GetString("Username");
    protected string? CurrentFullName => HttpContext.Session.GetString("FullName");

    protected bool IsLoggedIn => CurrentUserId.HasValue;
    protected bool IsMember => CurrentRole == "Member";
    protected bool IsAdmin => CurrentRole == "Admin";

    protected IActionResult? RequireMember()
    {
        if (!IsLoggedIn)
        {
            return RedirectToPage("/Account/Login");
        }

        return IsMember ? null : RedirectToPage("/Admin/Dashboard");
    }

    protected IActionResult? RequireAdmin()
    {
        if (!IsLoggedIn)
        {
            return RedirectToPage("/Account/Login");
        }

        return IsAdmin ? null : RedirectToPage("/Member/Dashboard");
    }

    protected void SignIn(int userId, string username, string role, string fullName)
    {
        HttpContext.Session.SetInt32("UserID", userId);
        HttpContext.Session.SetString("Username", username);
        HttpContext.Session.SetString("Role", role);
        HttpContext.Session.SetString("FullName", fullName);
    }

    protected void SignOut()
    {
        HttpContext.Session.Clear();
    }
}
