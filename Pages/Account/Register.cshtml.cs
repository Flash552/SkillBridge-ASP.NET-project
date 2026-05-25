using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Account;

public class RegisterModel(SkillBridgeDb db) : SkillBridgePageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (db.EmailExists(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "This email is already registered.");
        }

        if (db.UsernameExists(Input.Username))
        {
            ModelState.AddModelError("Input.Username", "This username is already taken.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new UserAccount
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Email = Input.Email,
            Username = Input.Username,
            Role = "Member"
        };

        db.AddUser(user, Input.Password);
        var registeredUser = db.Login(Input.Username, Input.Password);
        if (registeredUser is not null)
        {
            SignIn(registeredUser.UserID, registeredUser.Username, registeredUser.Role, registeredUser.FullName);
            db.AddActivity(registeredUser.UserID, "Registered Account", "Created a new SkillBridge member account.");
        }

        return RedirectToPage("/Member/Dashboard");
    }

    public class RegisterInput
    {
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(30, MinimumLength = 4)]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
