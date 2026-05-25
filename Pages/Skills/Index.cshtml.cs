using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Skills;

public class IndexModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];

    public void OnGet()
    {
        Skills = db.GetSkills();
    }
}
