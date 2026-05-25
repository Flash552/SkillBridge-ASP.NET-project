using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class UsersModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<UserAccount> Users { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        Users = db.GetUsers();
        return Page();
    }

    public IActionResult OnPostSave(UserAccount user, string? password)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        if (user.UserID == 0)
        {
            db.AddUser(user, string.IsNullOrWhiteSpace(password) ? "Password@123" : password);
        }
        else
        {
            db.UpdateUser(user, password);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        if (id != CurrentUserId)
        {
            db.DeleteUser(id);
        }

        return RedirectToPage();
    }
}
