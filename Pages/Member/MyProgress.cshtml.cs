using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class MyProgressModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<ProgressRecord> Progress { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        Progress = db.GetProgress(CurrentUserId!.Value);
        return Page();
    }
}
