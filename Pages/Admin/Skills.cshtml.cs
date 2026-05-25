using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class SkillsModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];

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

    public IActionResult OnPostSave(Skill skill)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.SaveSkill(skill);
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.DeleteSkill(id);
        return RedirectToPage();
    }

    private void Load()
    {
        Skills = db.GetSkills();
    }
}
