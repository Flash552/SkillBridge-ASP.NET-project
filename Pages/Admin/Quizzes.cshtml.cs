using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class QuizzesModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<Skill> Skills { get; set; } = [];
    public List<QuizInfo> Quizzes { get; set; } = [];

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

    public IActionResult OnPostSave(QuizInfo quiz)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.SaveQuiz(quiz);
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.DeleteQuiz(id);
        return RedirectToPage();
    }

    private void Load()
    {
        Skills = db.GetSkills();
        Quizzes = db.GetQuizzes();
    }
}
