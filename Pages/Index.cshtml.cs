using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class IndexModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];
    public List<LearningResource> FeaturedResources { get; set; } = [];

    public void OnGet()
    {
        Skills = db.GetSkills();
        FeaturedResources = db.GetResources().Take(4).ToList();
    }
}
