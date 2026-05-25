using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Member;

public class DashboardModel(SkillBridgeDb db) : SkillBridgePageModel
{
    public string FullName { get; set; } = string.Empty;
    public int CompletedCount { get; set; }
    public int QuizAttempts { get; set; }
    public string LatestScore { get; set; } = "N/A";
    public List<ActivityRecord> RecentActivities { get; set; } = [];

    public IActionResult OnGet()
    {
        var guard = RequireMember();
        if (guard is not null)
        {
            return guard;
        }

        FullName = CurrentFullName ?? "Member";
        var progress = db.GetProgress(CurrentUserId!.Value);
        var results = db.GetQuizResults(CurrentUserId.Value);
        CompletedCount = progress.Count(p => p.Status == "Completed");
        QuizAttempts = results.Count;
        LatestScore = results.FirstOrDefault() is { } latest ? $"{latest.Score}/{latest.TotalMarks}" : "N/A";
        RecentActivities = db.GetActivities(CurrentUserId.Value).Take(5).ToList();
        return Page();
    }
}
