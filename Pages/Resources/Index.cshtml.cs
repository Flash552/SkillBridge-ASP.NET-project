using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Resources;

public class IndexModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];
    public List<LearningResource> Resources { get; set; } = [];
    public int? SkillId { get; set; }

    public void OnGet(int? skillId)
    {
        SkillId = skillId;
        Skills = db.GetSkills();
        Resources = db.GetResources(skillId);
    }
}
