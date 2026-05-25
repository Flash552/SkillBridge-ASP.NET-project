using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;

namespace MyWebApp.Pages.Account;

public class LoginModel(SkillBridgeDb db) : SkillBridgePageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = db.Login(Input.Username, Input.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }

        SignIn(user.UserID, user.Username, user.Role, user.FullName);
        db.AddActivity(user.UserID, "Login", $"{user.FullName} logged in.");

        return user.Role == "Admin"
            ? RedirectToPage("/Admin/Dashboard")
            : RedirectToPage("/Member/Dashboard");
    }

    public class LoginInput
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
