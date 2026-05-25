using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;

namespace MyWebApp.Pages.Admin;

public class DashboardModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public int UserCount { get; set; }
    public int SkillCount { get; set; }
    public int ResourceCount { get; set; }
    public int QuizCount { get; set; }

    public IActionResult OnGet()
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        UserCount = db.CountRows("Users");
        SkillCount = db.CountRows("Skills");
        ResourceCount = db.CountRows("Resources");
        QuizCount = db.CountRows("Quiz");
        return Page();
    }
}
