using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class AttemptQuizModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public QuizInfo? Quiz { get; set; }
    public List<QuestionInfo> Questions { get; set; } = [];
    public string? ScoreMessage { get; set; }

    [BindProperty]
    public Dictionary<int, string> Answers { get; set; } = [];

    public IActionResult OnGet(int id)
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        LoadQuiz(id);
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        LoadQuiz(id);
        if (Questions.Count == 0)
        {
            return Page();
        }

        if (Answers.Count < Questions.Count)
        {
            ModelState.AddModelError(string.Empty, "Please answer all questions before submitting.");
            return Page();
        }

        var score = db.SaveQuizResult(CurrentUserId!.Value, id, Answers);
        ScoreMessage = $"Your score is {score}/{Questions.Count}. The result has been saved to your learning history.";
        return Page();
    }

    private void LoadQuiz(int id)
    {
        Quiz = db.GetQuiz(id);
        Questions = db.GetQuestions(id);
    }
}
