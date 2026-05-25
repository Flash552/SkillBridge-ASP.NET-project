using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class LearningHistoryModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<ActivityRecord> Activities { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        Activities = db.GetActivities(CurrentUserId!.Value);
        return Page();
    }
}
