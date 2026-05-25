using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class ProfileModel(SkillBridgeDb db) : SkillBridgePageModel
{
    [BindProperty]
    public UserAccount Profile { get; set; } = new();

    [BindProperty, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
    public string? NewPassword { get; set; }

    public string? Message { get; set; }

    public IActionResult OnGet()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        Profile = db.GetUser(CurrentUserId!.Value) ?? new UserAccount();
        return Page();
    }

    public IActionResult OnPost()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        Profile.UserID = CurrentUserId!.Value;
        Profile.Role = "Member";

        if (db.EmailExists(Profile.Email, Profile.UserID))
        {
            ModelState.AddModelError("Profile.Email", "This email is already used by another account.");
        }

        if (db.UsernameExists(Profile.Username, Profile.UserID))
        {
            ModelState.AddModelError("Profile.Username", "This username is already used by another account.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        db.UpdateUser(Profile, NewPassword);
        SignIn(Profile.UserID, Profile.Username, Profile.Role, Profile.FullName);
        db.AddActivity(Profile.UserID, "Updated Profile", "Updated member profile details.");
        Message = "Profile updated successfully.";
        return Page();
    }
}
