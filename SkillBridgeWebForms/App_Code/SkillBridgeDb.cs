using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public static class SkillBridgeDb
{
    private static string ConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["SkillBridgeConnection"].ConnectionString; }
    }

    public static void Initialize()
    {
        Execute(SchemaSql);

        if (Count("Users") == 0)
        {
            Seed();
        }
    }

    public static List<Skill> GetSkills()
    {
        List<Skill> items = new List<Skill>();
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand("SELECT SkillID, SkillName, Description FROM Skills ORDER BY SkillName", connection))
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                items.Add(ReadSkill(reader));
            }
        }

        return items;
    }

    public static Skill GetSkill(int id)
    {
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand("SELECT SkillID, SkillName, Description FROM Skills WHERE SkillID = @id", connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read() ? ReadSkill(reader) : null;
            }
        }
    }

    public static void AddSkill(string skillName, string description)
    {
        Execute("INSERT INTO Skills (SkillName, Description) VALUES (@name, @description)",
            Param("@name", skillName), Param("@description", description));
    }

    public static void UpdateSkill(int id, string skillName, string description)
    {
        Execute("UPDATE Skills SET SkillName = @name, Description = @description WHERE SkillID = @id",
            Param("@id", id), Param("@name", skillName), Param("@description", description));
    }

    public static void DeleteSkill(int id)
    {
        Execute("DELETE FROM Skills WHERE SkillID = @id", Param("@id", id));
    }

    public static List<LearningResource> GetResources(int? skillId)
    {
        List<LearningResource> items = new List<LearningResource>();
        string sql = @"SELECT r.ResourceID, r.SkillID, s.SkillName, r.Title, r.Type, r.URL, r.Description
                       FROM Resources r
                       INNER JOIN Skills s ON s.SkillID = r.SkillID
                       WHERE (@skillId IS NULL OR r.SkillID = @skillId)
                       ORDER BY s.SkillName, r.Title";

        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@skillId", (object)skillId ?? DBNull.Value);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(ReadResource(reader));
                }
            }
        }

        return items;
    }

    public static LearningResource GetResource(int id)
    {
        string sql = @"SELECT r.ResourceID, r.SkillID, s.SkillName, r.Title, r.Type, r.URL, r.Description
                       FROM Resources r
                       INNER JOIN Skills s ON s.SkillID = r.SkillID
                       WHERE r.ResourceID = @id";

        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read() ? ReadResource(reader) : null;
            }
        }
    }

    public static void AddResource(int skillId, string title, string type, string url, string description)
    {
        Execute(@"INSERT INTO Resources (SkillID, Title, Type, URL, Description)
                  VALUES (@skillId, @title, @type, @url, @description)",
            Param("@skillId", skillId), Param("@title", title), Param("@type", type),
            Param("@url", url), Param("@description", description));
    }

    public static void UpdateResource(int id, int skillId, string title, string type, string url, string description)
    {
        Execute(@"UPDATE Resources SET SkillID = @skillId, Title = @title, Type = @type, URL = @url, Description = @description
                  WHERE ResourceID = @id",
            Param("@id", id), Param("@skillId", skillId), Param("@title", title), Param("@type", type),
            Param("@url", url), Param("@description", description));
    }

    public static void DeleteResource(int id)
    {
        Execute("DELETE FROM Resources WHERE ResourceID = @id", Param("@id", id));
    }

    public static UserAccount Login(string username, string password)
    {
        string sql = @"SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered
                       FROM Users
                       WHERE Username = @username AND Password = @password";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read() ? ReadUser(reader) : null;
            }
        }
    }

    public static bool UsernameExists(string username, int? excludingUserId)
    {
        return Exists("SELECT COUNT(*) FROM Users WHERE Username = @value AND (@id IS NULL OR UserID <> @id)",
            Param("@value", username), Param("@id", (object)excludingUserId ?? DBNull.Value));
    }

    public static bool EmailExists(string email, int? excludingUserId)
    {
        return Exists("SELECT COUNT(*) FROM Users WHERE Email = @value AND (@id IS NULL OR UserID <> @id)",
            Param("@value", email), Param("@id", (object)excludingUserId ?? DBNull.Value));
    }

    public static void AddUser(string firstName, string lastName, string email, string username, string password, string role)
    {
        Execute(@"INSERT INTO Users (FirstName, LastName, Email, Username, Password, Role, DateRegistered)
                  VALUES (@firstName, @lastName, @email, @username, @password, @role, @dateRegistered)",
            Param("@firstName", firstName), Param("@lastName", lastName), Param("@email", email),
            Param("@username", username), Param("@password", PasswordHasher.Hash(password)),
            Param("@role", role), Param("@dateRegistered", DateTime.Now));
    }

    public static UserAccount GetUser(int id)
    {
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand("SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered FROM Users WHERE UserID = @id", connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read() ? ReadUser(reader) : null;
            }
        }
    }

    public static List<UserAccount> GetUsers()
    {
        List<UserAccount> users = new List<UserAccount>();
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand("SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered FROM Users ORDER BY DateRegistered DESC", connection))
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                users.Add(ReadUser(reader));
            }
        }

        return users;
    }

    public static void UpdateUser(int id, string firstName, string lastName, string email, string username, string role)
    {
        Execute(@"UPDATE Users SET FirstName = @firstName, LastName = @lastName, Email = @email, Username = @username, Role = @role
                  WHERE UserID = @id",
            Param("@id", id), Param("@firstName", firstName), Param("@lastName", lastName),
            Param("@email", email), Param("@username", username), Param("@role", role));
    }

    public static void UpdateUserProfile(int id, string firstName, string lastName, string email, string username)
    {
        Execute(@"UPDATE Users SET FirstName = @firstName, LastName = @lastName, Email = @email, Username = @username
                  WHERE UserID = @id",
            Param("@id", id), Param("@firstName", firstName), Param("@lastName", lastName),
            Param("@email", email), Param("@username", username));
    }

    public static void DeleteUser(int id)
    {
        Execute("DELETE FROM Users WHERE UserID = @id", Param("@id", id));
    }

    public static List<QuizInfo> GetQuizzes()
    {
        List<QuizInfo> quizzes = new List<QuizInfo>();
        string sql = @"SELECT q.QuizID, q.SkillID, s.SkillName, q.QuizTitle, q.TotalMarks
                       FROM Quiz q INNER JOIN Skills s ON s.SkillID = q.SkillID
                       ORDER BY s.SkillName, q.QuizTitle";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                quizzes.Add(ReadQuiz(reader));
            }
        }

        return quizzes;
    }

    public static QuizInfo GetQuiz(int id)
    {
        string sql = @"SELECT q.QuizID, q.SkillID, s.SkillName, q.QuizTitle, q.TotalMarks
                       FROM Quiz q INNER JOIN Skills s ON s.SkillID = q.SkillID
                       WHERE q.QuizID = @id";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return reader.Read() ? ReadQuiz(reader) : null;
            }
        }
    }

    public static void AddQuiz(int skillId, string title, int totalMarks)
    {
        Execute("INSERT INTO Quiz (SkillID, QuizTitle, TotalMarks) VALUES (@skillId, @title, @marks)",
            Param("@skillId", skillId), Param("@title", title), Param("@marks", totalMarks));
    }

    public static void UpdateQuiz(int id, int skillId, string title, int totalMarks)
    {
        Execute("UPDATE Quiz SET SkillID = @skillId, QuizTitle = @title, TotalMarks = @marks WHERE QuizID = @id",
            Param("@id", id), Param("@skillId", skillId), Param("@title", title), Param("@marks", totalMarks));
    }

    public static void DeleteQuiz(int id)
    {
        Execute("DELETE FROM Quiz WHERE QuizID = @id", Param("@id", id));
    }

    public static List<QuestionInfo> GetQuestions(int? quizId)
    {
        List<QuestionInfo> questions = new List<QuestionInfo>();
        string sql = @"SELECT q.QuestionID, q.QuizID, z.QuizTitle, q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.CorrectAnswer
                       FROM Questions q INNER JOIN Quiz z ON z.QuizID = q.QuizID
                       WHERE (@quizId IS NULL OR q.QuizID = @quizId)
                       ORDER BY z.QuizTitle, q.QuestionID";

        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@quizId", (object)quizId ?? DBNull.Value);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    questions.Add(ReadQuestion(reader));
                }
            }
        }

        return questions;
    }

    public static void AddQuestion(int quizId, string text, string optionA, string optionB, string optionC, string correctAnswer)
    {
        Execute(@"INSERT INTO Questions (QuizID, QuestionText, OptionA, OptionB, OptionC, CorrectAnswer)
                  VALUES (@quizId, @text, @a, @b, @c, @correct)",
            Param("@quizId", quizId), Param("@text", text), Param("@a", optionA), Param("@b", optionB),
            Param("@c", optionC), Param("@correct", correctAnswer));
    }

    public static void UpdateQuestion(int id, int quizId, string text, string optionA, string optionB, string optionC, string correctAnswer)
    {
        Execute(@"UPDATE Questions SET QuizID = @quizId, QuestionText = @text, OptionA = @a, OptionB = @b, OptionC = @c, CorrectAnswer = @correct
                  WHERE QuestionID = @id",
            Param("@id", id), Param("@quizId", quizId), Param("@text", text), Param("@a", optionA),
            Param("@b", optionB), Param("@c", optionC), Param("@correct", correctAnswer));
    }

    public static void DeleteQuestion(int id)
    {
        Execute("DELETE FROM Questions WHERE QuestionID = @id", Param("@id", id));
    }

    public static void SaveProgress(int userId, int resourceId, string status)
    {
        string sql = @"IF EXISTS (SELECT 1 FROM Progress WHERE UserID = @userId AND ResourceID = @resourceId)
                           UPDATE Progress SET Status = @status, DateCompleted = @completed WHERE UserID = @userId AND ResourceID = @resourceId
                       ELSE
                           INSERT INTO Progress (UserID, ResourceID, Status, DateCompleted) VALUES (@userId, @resourceId, @status, @completed)";
        object completed = status == "Completed" ? (object)DateTime.Now : DBNull.Value;
        Execute(sql, Param("@userId", userId), Param("@resourceId", resourceId), Param("@status", status), Param("@completed", completed));
        AddActivity(userId, "Progress", "Marked resource #" + resourceId + " as " + status);
    }

    public static List<ProgressRecord> GetProgressForUser(int userId)
    {
        List<ProgressRecord> items = new List<ProgressRecord>();
        string sql = @"SELECT p.ProgressID, p.UserID, p.ResourceID, r.Title, s.SkillName, p.Status, p.DateCompleted
                       FROM Progress p
                       INNER JOIN Resources r ON r.ResourceID = p.ResourceID
                       INNER JOIN Skills s ON s.SkillID = r.SkillID
                       WHERE p.UserID = @userId
                       ORDER BY p.ProgressID DESC";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(ReadProgress(reader));
                }
            }
        }

        return items;
    }

    public static void AddActivity(int userId, string activityType, string description)
    {
        Execute(@"INSERT INTO ActivityHistory (UserID, ActivityType, Description, ActivityDate)
                  VALUES (@userId, @type, @description, @date)",
            Param("@userId", userId), Param("@type", activityType), Param("@description", description), Param("@date", DateTime.Now));
    }

    public static List<ActivityRecord> GetActivities(int userId)
    {
        List<ActivityRecord> items = new List<ActivityRecord>();
        string sql = @"SELECT ActivityID, UserID, ActivityType, Description, ActivityDate
                       FROM ActivityHistory
                       WHERE UserID = @userId
                       ORDER BY ActivityDate DESC";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(ReadActivity(reader));
                }
            }
        }

        return items;
    }

    public static void SaveQuizResult(int userId, int quizId, int score)
    {
        Execute(@"INSERT INTO QuizResults (UserID, QuizID, Score, DateAttempted)
                  VALUES (@userId, @quizId, @score, @date)",
            Param("@userId", userId), Param("@quizId", quizId), Param("@score", score), Param("@date", DateTime.Now));
        AddActivity(userId, "Quiz", "Completed quiz #" + quizId + " with score " + score);
    }

    public static List<QuizResultRecord> GetQuizResults(int? userId)
    {
        List<QuizResultRecord> items = new List<QuizResultRecord>();
        string sql = @"SELECT qr.ResultID, qr.UserID, u.FirstName + ' ' + u.LastName AS FullName, qr.QuizID, q.QuizTitle, qr.Score, q.TotalMarks, qr.DateAttempted
                       FROM QuizResults qr
                       INNER JOIN Users u ON u.UserID = qr.UserID
                       INNER JOIN Quiz q ON q.QuizID = qr.QuizID
                       WHERE (@userId IS NULL OR qr.UserID = @userId)
                       ORDER BY qr.DateAttempted DESC";
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@userId", (object)userId ?? DBNull.Value);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(ReadQuizResult(reader));
                }
            }
        }

        return items;
    }

    public static int Count(string tableName)
    {
        return ScalarInt("SELECT COUNT(*) FROM " + tableName);
    }

    public static int CountForUser(string tableName, int userId)
    {
        return ScalarInt("SELECT COUNT(*) FROM " + tableName + " WHERE UserID = @userId", Param("@userId", userId));
    }

    private static SqlConnection OpenConnection()
    {
        SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private static bool Exists(string sql, params SqlParameter[] parameters)
    {
        return ScalarInt(sql, parameters) > 0;
    }

    private static int ScalarInt(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddRange(parameters);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    private static void Execute(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = OpenConnection())
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }
    }

    private static SqlParameter Param(string name, object value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }

    private static Skill ReadSkill(SqlDataReader reader)
    {
        return new Skill
        {
            SkillID = Convert.ToInt32(reader["SkillID"]),
            SkillName = Convert.ToString(reader["SkillName"]),
            Description = Convert.ToString(reader["Description"])
        };
    }

    private static LearningResource ReadResource(SqlDataReader reader)
    {
        return new LearningResource
        {
            ResourceID = Convert.ToInt32(reader["ResourceID"]),
            SkillID = Convert.ToInt32(reader["SkillID"]),
            SkillName = Convert.ToString(reader["SkillName"]),
            Title = Convert.ToString(reader["Title"]),
            Type = Convert.ToString(reader["Type"]),
            Url = Convert.ToString(reader["URL"]),
            Description = Convert.ToString(reader["Description"])
        };
    }

    private static UserAccount ReadUser(SqlDataReader reader)
    {
        return new UserAccount
        {
            UserID = Convert.ToInt32(reader["UserID"]),
            FirstName = Convert.ToString(reader["FirstName"]),
            LastName = Convert.ToString(reader["LastName"]),
            Email = Convert.ToString(reader["Email"]),
            Username = Convert.ToString(reader["Username"]),
            Password = Convert.ToString(reader["Password"]),
            Role = Convert.ToString(reader["Role"]),
            DateRegistered = Convert.ToDateTime(reader["DateRegistered"])
        };
    }

    private static QuizInfo ReadQuiz(SqlDataReader reader)
    {
        return new QuizInfo
        {
            QuizID = Convert.ToInt32(reader["QuizID"]),
            SkillID = Convert.ToInt32(reader["SkillID"]),
            SkillName = Convert.ToString(reader["SkillName"]),
            QuizTitle = Convert.ToString(reader["QuizTitle"]),
            TotalMarks = Convert.ToInt32(reader["TotalMarks"])
        };
    }

    private static QuestionInfo ReadQuestion(SqlDataReader reader)
    {
        return new QuestionInfo
        {
            QuestionID = Convert.ToInt32(reader["QuestionID"]),
            QuizID = Convert.ToInt32(reader["QuizID"]),
            QuizTitle = Convert.ToString(reader["QuizTitle"]),
            QuestionText = Convert.ToString(reader["QuestionText"]),
            OptionA = Convert.ToString(reader["OptionA"]),
            OptionB = Convert.ToString(reader["OptionB"]),
            OptionC = Convert.ToString(reader["OptionC"]),
            CorrectAnswer = Convert.ToString(reader["CorrectAnswer"])
        };
    }

    private static ProgressRecord ReadProgress(SqlDataReader reader)
    {
        return new ProgressRecord
        {
            ProgressID = Convert.ToInt32(reader["ProgressID"]),
            UserID = Convert.ToInt32(reader["UserID"]),
            ResourceID = Convert.ToInt32(reader["ResourceID"]),
            ResourceTitle = Convert.ToString(reader["Title"]),
            SkillName = Convert.ToString(reader["SkillName"]),
            Status = Convert.ToString(reader["Status"]),
            DateCompleted = reader["DateCompleted"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateCompleted"])
        };
    }

    private static ActivityRecord ReadActivity(SqlDataReader reader)
    {
        return new ActivityRecord
        {
            ActivityID = Convert.ToInt32(reader["ActivityID"]),
            UserID = Convert.ToInt32(reader["UserID"]),
            ActivityType = Convert.ToString(reader["ActivityType"]),
            Description = Convert.ToString(reader["Description"]),
            ActivityDate = Convert.ToDateTime(reader["ActivityDate"])
        };
    }

    private static QuizResultRecord ReadQuizResult(SqlDataReader reader)
    {
        return new QuizResultRecord
        {
            ResultID = Convert.ToInt32(reader["ResultID"]),
            UserID = Convert.ToInt32(reader["UserID"]),
            FullName = Convert.ToString(reader["FullName"]),
            QuizID = Convert.ToInt32(reader["QuizID"]),
            QuizTitle = Convert.ToString(reader["QuizTitle"]),
            Score = Convert.ToInt32(reader["Score"]),
            TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
            DateAttempted = Convert.ToDateTime(reader["DateAttempted"])
        };
    }

    private static void Seed()
    {
        AddUser("System", "Admin", "admin@skillbridge.local", "admin", "Admin@123", "Admin");
        AddUser("John", "Smith", "johnsmith99@example.com", "johnsmith99", "Member@123", "Member");

        AddSkill("Web Development", "Build interactive websites with HTML5, CSS, JavaScript, and server-side programming.");
        AddSkill("Database Design", "Understand relational database design, SQL queries, and data-driven applications.");
        AddSkill("Programming Basics", "Practice problem solving, variables, control flow, and reusable code.");
        AddSkill("Digital Productivity", "Use digital tools to organize tasks, communicate clearly, and learn independently.");

        AddResource(1, "HTML5 Semantic Layout Video", "Video", "https://www.youtube.com/embed/UB1O30fR-EE", "An embedded video introduction to semantic HTML5 page structure.");
        AddResource(1, "CSS Layout Reference", "Link", "https://developer.mozilla.org/en-US/docs/Learn/CSS/CSS_layout", "MDN guide for modern CSS layout.");
        AddResource(2, "SQL Server SELECT Tutorial", "Video", "https://www.youtube.com/embed/HXV3zeQKqGY", "Video lesson for writing SELECT queries.");
        AddResource(2, "Database Normalization Notes", "PDF", "https://www3.ntu.edu.sg/home/ehchua/programming/sql/Relational_Database_Design.pdf", "PDF notes for relational database normalization.");
        AddResource(3, "C# Basics Guide", "Link", "https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/", "Microsoft Learn introduction to C#.");
        AddResource(4, "Study Planning Image", "Link", "Images/learning-dashboard.svg", "A local image resource used to demonstrate multimedia in the website.");

        AddQuiz(1, "HTML5 Fundamentals", 3);
        AddQuiz(2, "SQL Basics", 3);
        AddQuiz(3, "C# Basics", 3);

        AddQuestion(1, "Which element represents a standalone article?", "section", "article", "div", "B");
        AddQuestion(1, "Which attribute provides alternative text for images?", "alt", "title", "src", "A");
        AddQuestion(1, "Which element is used for page navigation links?", "nav", "aside", "footer", "A");
        AddQuestion(2, "Which SQL command reads data?", "SELECT", "INSERT", "DELETE", "A");
        AddQuestion(2, "Which key uniquely identifies a row?", "Foreign key", "Primary key", "Candidate text", "B");
        AddQuestion(2, "Which clause filters records?", "WHERE", "ORDER BY", "GROUP BY", "A");
        AddQuestion(3, "Which keyword declares a variable with explicit type?", "int", "loop", "return", "A");
        AddQuestion(3, "Which statement repeats while a condition is true?", "if", "while", "class", "B");
        AddQuestion(3, "Which type stores true or false?", "bool", "string", "decimal", "A");
    }

    private const string SchemaSql = @"
IF OBJECT_ID('QuizResults', 'U') IS NULL
BEGIN
    CREATE TABLE QuizResults (
        ResultID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT NOT NULL,
        QuizID INT NOT NULL,
        Score INT NOT NULL,
        DateAttempted DATETIME NOT NULL
    );
END;

IF OBJECT_ID('Questions', 'U') IS NULL
BEGIN
    CREATE TABLE Questions (
        QuestionID INT IDENTITY(1,1) PRIMARY KEY,
        QuizID INT NOT NULL,
        QuestionText NVARCHAR(500) NOT NULL,
        OptionA NVARCHAR(255) NOT NULL,
        OptionB NVARCHAR(255) NOT NULL,
        OptionC NVARCHAR(255) NOT NULL,
        CorrectAnswer NVARCHAR(1) NOT NULL CHECK (CorrectAnswer IN ('A', 'B', 'C'))
    );
END;

IF OBJECT_ID('ActivityHistory', 'U') IS NULL
BEGIN
    CREATE TABLE ActivityHistory (
        ActivityID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT NOT NULL,
        ActivityType NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        ActivityDate DATETIME NOT NULL
    );
END;

IF OBJECT_ID('Progress', 'U') IS NULL
BEGIN
    CREATE TABLE Progress (
        ProgressID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT NOT NULL,
        ResourceID INT NOT NULL,
        Status NVARCHAR(20) NOT NULL CHECK (Status IN ('In Progress', 'Completed')),
        DateCompleted DATETIME NULL,
        CONSTRAINT UQ_Progress_UserResource UNIQUE (UserID, ResourceID)
    );
END;

IF OBJECT_ID('Resources', 'U') IS NULL
BEGIN
    CREATE TABLE Resources (
        ResourceID INT IDENTITY(1,1) PRIMARY KEY,
        SkillID INT NOT NULL,
        Title NVARCHAR(150) NOT NULL,
        Type NVARCHAR(20) NOT NULL CHECK (Type IN ('Video', 'PDF', 'Link')),
        URL NVARCHAR(500) NOT NULL,
        Description NVARCHAR(1000) NOT NULL
    );
END;

IF OBJECT_ID('Quiz', 'U') IS NULL
BEGIN
    CREATE TABLE Quiz (
        QuizID INT IDENTITY(1,1) PRIMARY KEY,
        SkillID INT NOT NULL,
        QuizTitle NVARCHAR(150) NOT NULL,
        TotalMarks INT NOT NULL
    );
END;

IF OBJECT_ID('Skills', 'U') IS NULL
BEGIN
    CREATE TABLE Skills (
        SkillID INT IDENTITY(1,1) PRIMARY KEY,
        SkillName NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(1000) NOT NULL
    );
END;

IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        UserID INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(80) NOT NULL,
        LastName NVARCHAR(80) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        Username NVARCHAR(80) NOT NULL UNIQUE,
        Password NVARCHAR(128) NOT NULL,
        Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Member', 'Admin')),
        DateRegistered DATETIME NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Resources_Skills')
    ALTER TABLE Resources ADD CONSTRAINT FK_Resources_Skills FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Progress_Users')
    ALTER TABLE Progress ADD CONSTRAINT FK_Progress_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Progress_Resources')
    ALTER TABLE Progress ADD CONSTRAINT FK_Progress_Resources FOREIGN KEY (ResourceID) REFERENCES Resources(ResourceID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Activity_Users')
    ALTER TABLE ActivityHistory ADD CONSTRAINT FK_Activity_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Quiz_Skills')
    ALTER TABLE Quiz ADD CONSTRAINT FK_Quiz_Skills FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Questions_Quiz')
    ALTER TABLE Questions ADD CONSTRAINT FK_Questions_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_QuizResults_Users')
    ALTER TABLE QuizResults ADD CONSTRAINT FK_QuizResults_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_QuizResults_Quiz')
    ALTER TABLE QuizResults ADD CONSTRAINT FK_QuizResults_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE;
";
}
