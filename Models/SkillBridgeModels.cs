using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

public class UserAccount
{
    public int UserID { get; set; }

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Member";

    public string DateRegistered { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}

public class Skill
{
    public int SkillID { get; set; }

    [Required, Display(Name = "Skill Name")]
    public string SkillName { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}

public class LearningResource
{
    public int ResourceID { get; set; }

    [Required, Display(Name = "Skill")]
    public int SkillID { get; set; }

    public string SkillName { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "Link";

    [Required, Url]
    public string URL { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}

public class ProgressRecord
{
    public int ProgressID { get; set; }
    public int UserID { get; set; }
    public int ResourceID { get; set; }
    public string ResourceTitle { get; set; } = string.Empty;
    public string SkillName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? DateCompleted { get; set; }
}

public class ActivityRecord
{
    public int ActivityID { get; set; }
    public int UserID { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityDate { get; set; } = string.Empty;
}

public class QuizInfo
{
    public int QuizID { get; set; }
    public int SkillID { get; set; }
    public string SkillName { get; set; } = string.Empty;

    [Required, Display(Name = "Quiz Title")]
    public string QuizTitle { get; set; } = string.Empty;

    [Required, Range(1, 100)]
    public int TotalMarks { get; set; }
}

public class QuestionInfo
{
    public int QuestionID { get; set; }
    public int QuizID { get; set; }
    public string QuizTitle { get; set; } = string.Empty;

    [Required, Display(Name = "Question")]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public string OptionA { get; set; } = string.Empty;

    [Required]
    public string OptionB { get; set; } = string.Empty;

    [Required]
    public string OptionC { get; set; } = string.Empty;

    [Required]
    public string CorrectAnswer { get; set; } = "A";
}

public class QuizResultRecord
{
    public int ResultID { get; set; }
    public int UserID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int QuizID { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalMarks { get; set; }
    public string DateAttempted { get; set; } = string.Empty;
}
