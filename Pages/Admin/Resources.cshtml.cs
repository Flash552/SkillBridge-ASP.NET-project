using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class ResourcesModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];
    public List<LearningResource> Resources { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        Load();
        return Page();
    }

    public IActionResult OnPostSave(LearningResource resource)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.SaveResource(resource);
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.DeleteResource(id);
        return RedirectToPage();
    }

    private void Load()
    {
        Skills = db.GetSkills();
        Resources = db.GetResources();
    }
}
