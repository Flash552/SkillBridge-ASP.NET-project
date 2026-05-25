using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Pages.Account;

public class LogoutModel : SkillBridgePageModel
{
    public IActionResult OnGet()
    {
        SignOut();
        return RedirectToPage("/Index");
    }
}
