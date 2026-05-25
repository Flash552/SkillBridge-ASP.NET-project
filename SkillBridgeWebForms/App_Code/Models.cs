using System;

public class UserAccount
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public DateTime DateRegistered { get; set; }

    public string FullName
    {
        get { return (FirstName + " " + LastName).Trim(); }
    }
}

public class Skill
{
    public int SkillID { get; set; }
    public string SkillName { get; set; }
    public string Description { get; set; }
}

public class LearningResource
{
    public int ResourceID { get; set; }
    public int SkillID { get; set; }
    public string SkillName { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

public class ProgressRecord
{
    public int ProgressID { get; set; }
    public int UserID { get; set; }
    public int ResourceID { get; set; }
    public string ResourceTitle { get; set; }
    public string SkillName { get; set; }
    public string Status { get; set; }
    public DateTime? DateCompleted { get; set; }
}

public class ActivityRecord
{
    public int ActivityID { get; set; }
    public int UserID { get; set; }
    public string ActivityType { get; set; }
    public string Description { get; set; }
    public DateTime ActivityDate { get; set; }
}

public class QuizInfo
{
    public int QuizID { get; set; }
    public int SkillID { get; set; }
    public string SkillName { get; set; }
    public string QuizTitle { get; set; }
    public int TotalMarks { get; set; }
}

public class QuestionInfo
{
    public int QuestionID { get; set; }
    public int QuizID { get; set; }
    public string QuizTitle { get; set; }
    public string QuestionText { get; set; }
    public string OptionA { get; set; }
    public string OptionB { get; set; }
    public string OptionC { get; set; }
    public string CorrectAnswer { get; set; }
}

public class QuizResultRecord
{
    public int ResultID { get; set; }
    public int UserID { get; set; }
    public string FullName { get; set; }
    public int QuizID { get; set; }
    public string QuizTitle { get; set; }
    public int Score { get; set; }
    public int TotalMarks { get; set; }
    public DateTime DateAttempted { get; set; }
}
