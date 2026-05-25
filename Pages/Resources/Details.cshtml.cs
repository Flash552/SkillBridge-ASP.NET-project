using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Resources;

public class DetailsModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public LearningResource? Resource { get; set; }
    public bool IsMemberUser => IsMember;

    public void OnGet(int id)
    {
        Resource = db.GetResource(id);
    }

    public IActionResult OnPost(int resourceId, string status)
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        db.SaveProgress(CurrentUserId!.Value, resourceId, status);
        return RedirectToPage("/Member/MyProgress");
    }
}
