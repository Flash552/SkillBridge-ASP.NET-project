using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class QuizCentreModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<QuizInfo> Quizzes { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        Quizzes = db.GetQuizzes();
        return Page();
    }
}
