using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Admin;

public class QuestionsModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public List<QuizInfo> Quizzes { get; set; } = [];
    public List<QuestionInfo> Questions { get; set; } = [];

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

    public IActionResult OnPostSave(QuestionInfo question)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.SaveQuestion(question);
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var guard = RequireAdmin();
        if (guard is not null)
        {
            return guard;
        }

        db.DeleteQuestion(id);
        return RedirectToPage();
    }

    private void Load()
    {
        Quizzes = db.GetQuizzes();
        Questions = db.GetQuestions();
    }
}
