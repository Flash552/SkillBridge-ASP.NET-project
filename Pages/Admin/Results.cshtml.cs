using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class ResultsModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<QuizResultRecord> Results { get; set; } = [];
    public List<ActivityRecord> Activities { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        Results = db.GetQuizResults();
        Activities = db.GetActivities();
        return Page();
    }
}
