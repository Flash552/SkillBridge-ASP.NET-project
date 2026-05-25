using Microsoft.Data.Sqlite;
using MyWebApp.Models;

namespace MyWebApp.Data;

public class SkillBridgeDb
{
    private readonly string _connectionString;

    public SkillBridgeDb(IWebHostEnvironment environment)
    {
        var dbPath = Path.Combine(environment.ContentRootPath, "skillbridge.db");
        _connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
    }

    public void Initialize()
    {
        using var connection = OpenConnection();
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Users (
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL CHECK (Role IN ('Member', 'Admin')),
                DateRegistered TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Skills (
                SkillID INTEGER PRIMARY KEY AUTOINCREMENT,
                SkillName TEXT NOT NULL UNIQUE,
                Description TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Resources (
                ResourceID INTEGER PRIMARY KEY AUTOINCREMENT,
                SkillID INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Type TEXT NOT NULL CHECK (Type IN ('Video', 'PDF', 'Link')),
                URL TEXT NOT NULL,
                Description TEXT NOT NULL,
                FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Progress (
                ProgressID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                ResourceID INTEGER NOT NULL,
                Status TEXT NOT NULL CHECK (Status IN ('In Progress', 'Completed')),
                DateCompleted TEXT,
                UNIQUE(UserID, ResourceID),
                FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
                FOREIGN KEY (ResourceID) REFERENCES Resources(ResourceID) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS ActivityHistory (
                ActivityID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                ActivityType TEXT NOT NULL,
                Description TEXT NOT NULL,
                ActivityDate TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Quiz (
                QuizID INTEGER PRIMARY KEY AUTOINCREMENT,
                SkillID INTEGER NOT NULL,
                QuizTitle TEXT NOT NULL,
                TotalMarks INTEGER NOT NULL,
                FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Questions (
                QuestionID INTEGER PRIMARY KEY AUTOINCREMENT,
                QuizID INTEGER NOT NULL,
                QuestionText TEXT NOT NULL,
                OptionA TEXT NOT NULL,
                OptionB TEXT NOT NULL,
                OptionC TEXT NOT NULL,
                CorrectAnswer TEXT NOT NULL CHECK (CorrectAnswer IN ('A', 'B', 'C')),
                FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS QuizResults (
                ResultID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                QuizID INTEGER NOT NULL,
                Score INTEGER NOT NULL,
                DateAttempted TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
                FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE
            );
            """);

        if (Count(connection, "Users") == 0)
        {
            Seed(connection);
        }
    }

    public List<Skill> GetSkills()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT SkillID, SkillName, Description FROM Skills ORDER BY SkillName";
        using var reader = command.ExecuteReader();
        var skills = new List<Skill>();
        while (reader.Read())
        {
            skills.Add(new Skill
            {
                SkillID = reader.GetInt32(0),
                SkillName = reader.GetString(1),
                Description = reader.GetString(2)
            });
        }

        return skills;
    }

    public List<LearningResource> GetResources(int? skillId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT r.ResourceID, r.SkillID, s.SkillName, r.Title, r.Type, r.URL, r.Description
            FROM Resources r
            INNER JOIN Skills s ON s.SkillID = r.SkillID
            WHERE $skillId IS NULL OR r.SkillID = $skillId
            ORDER BY s.SkillName, r.Title
            """;
        command.Parameters.AddWithValue("$skillId", skillId.HasValue ? skillId.Value : DBNull.Value);

        using var reader = command.ExecuteReader();
        var resources = new List<LearningResource>();
        while (reader.Read())
        {
            resources.Add(ReadResource(reader));
        }

        return resources;
    }

    public LearningResource? GetResource(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT r.ResourceID, r.SkillID, s.SkillName, r.Title, r.Type, r.URL, r.Description
            FROM Resources r
            INNER JOIN Skills s ON s.SkillID = r.SkillID
            WHERE r.ResourceID = $id
            """;
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadResource(reader) : null;
    }

    public UserAccount? Login(string username, string password)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered
            FROM Users
            WHERE Username = $username AND Password = $password
            """;
        command.Parameters.AddWithValue("$username", username);
        command.Parameters.AddWithValue("$password", PasswordHasher.Hash(password));
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public bool UsernameExists(string username, int? excludingUserId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $username AND ($id IS NULL OR UserID <> $id)";
        command.Parameters.AddWithValue("$username", username);
        command.Parameters.AddWithValue("$id", excludingUserId.HasValue ? excludingUserId.Value : DBNull.Value);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool EmailExists(string email, int? excludingUserId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = $email AND ($id IS NULL OR UserID <> $id)";
        command.Parameters.AddWithValue("$email", email);
        command.Parameters.AddWithValue("$id", excludingUserId.HasValue ? excludingUserId.Value : DBNull.Value);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void AddUser(UserAccount user, string password)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users (FirstName, LastName, Email, Username, Password, Role, DateRegistered)
            VALUES ($firstName, $lastName, $email, $username, $password, $role, $dateRegistered)
            """;
        command.Parameters.AddWithValue("$firstName", user.FirstName);
        command.Parameters.AddWithValue("$lastName", user.LastName);
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$username", user.Username);
        command.Parameters.AddWithValue("$password", PasswordHasher.Hash(password));
        command.Parameters.AddWithValue("$role", user.Role);
        command.Parameters.AddWithValue("$dateRegistered", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        command.ExecuteNonQuery();
    }

    public UserAccount? GetUser(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered FROM Users WHERE UserID = $id";
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public List<UserAccount> GetUsers()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT UserID, FirstName, LastName, Email, Username, Password, Role, DateRegistered FROM Users ORDER BY Role, FirstName";
        using var reader = command.ExecuteReader();
        var users = new List<UserAccount>();
        while (reader.Read())
        {
            users.Add(ReadUser(reader));
        }

        return users;
    }

    public void UpdateUser(UserAccount user, string? newPassword = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = string.IsNullOrWhiteSpace(newPassword)
            ? """
                UPDATE Users
                SET FirstName = $firstName, LastName = $lastName, Email = $email, Username = $username, Role = $role
                WHERE UserID = $id
                """
            : """
                UPDATE Users
                SET FirstName = $firstName, LastName = $lastName, Email = $email, Username = $username, Role = $role, Password = $password
                WHERE UserID = $id
                """;
        command.Parameters.AddWithValue("$id", user.UserID);
        command.Parameters.AddWithValue("$firstName", user.FirstName);
        command.Parameters.AddWithValue("$lastName", user.LastName);
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$username", user.Username);
        command.Parameters.AddWithValue("$role", user.Role);
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            command.Parameters.AddWithValue("$password", PasswordHasher.Hash(newPassword));
        }

        command.ExecuteNonQuery();
    }

    public void DeleteUser(int id)
    {
        using var connection = OpenConnection();
        Execute(connection, "DELETE FROM Users WHERE UserID = $id", ("$id", id));
    }

    public void SaveSkill(Skill skill)
    {
        using var connection = OpenConnection();
        if (skill.SkillID == 0)
        {
            Execute(connection, "INSERT INTO Skills (SkillName, Description) VALUES ($name, $description)",
                ("$name", skill.SkillName), ("$description", skill.Description));
        }
        else
        {
            Execute(connection, "UPDATE Skills SET SkillName = $name, Description = $description WHERE SkillID = $id",
                ("$id", skill.SkillID), ("$name", skill.SkillName), ("$description", skill.Description));
        }
    }

    public void DeleteSkill(int id)
    {
        using var connection = OpenConnection();
        Execute(connection, "DELETE FROM Skills WHERE SkillID = $id", ("$id", id));
    }

    public void SaveResource(LearningResource resource)
    {
        using var connection = OpenConnection();
        if (resource.ResourceID == 0)
        {
            Execute(connection, """
                INSERT INTO Resources (SkillID, Title, Type, URL, Description)
                VALUES ($skillId, $title, $type, $url, $description)
                """, ("$skillId", resource.SkillID), ("$title", resource.Title), ("$type", resource.Type),
                ("$url", resource.URL), ("$description", resource.Description));
        }
        else
        {
            Execute(connection, """
                UPDATE Resources
                SET SkillID = $skillId, Title = $title, Type = $type, URL = $url, Description = $description
                WHERE ResourceID = $id
                """, ("$id", resource.ResourceID), ("$skillId", resource.SkillID), ("$title", resource.Title),
                ("$type", resource.Type), ("$url", resource.URL), ("$description", resource.Description));
        }
    }

    public void DeleteResource(int id)
    {
        using var connection = OpenConnection();
        Execute(connection, "DELETE FROM Resources WHERE ResourceID = $id", ("$id", id));
    }

    public List<ProgressRecord> GetProgress(int userId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT p.ProgressID, p.UserID, p.ResourceID, r.Title, s.SkillName, p.Status, p.DateCompleted
            FROM Progress p
            INNER JOIN Resources r ON r.ResourceID = p.ResourceID
            INNER JOIN Skills s ON s.SkillID = r.SkillID
            WHERE p.UserID = $userId
            ORDER BY COALESCE(p.DateCompleted, '') DESC, r.Title
            """;
        command.Parameters.AddWithValue("$userId", userId);
        using var reader = command.ExecuteReader();
        var progress = new List<ProgressRecord>();
        while (reader.Read())
        {
            progress.Add(new ProgressRecord
            {
                ProgressID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                ResourceID = reader.GetInt32(2),
                ResourceTitle = reader.GetString(3),
                SkillName = reader.GetString(4),
                Status = reader.GetString(5),
                DateCompleted = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return progress;
    }

    public void SaveProgress(int userId, int resourceId, string status)
    {
        using var connection = OpenConnection();
        var dateCompleted = status == "Completed" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : null;
        Execute(connection, """
            INSERT INTO Progress (UserID, ResourceID, Status, DateCompleted)
            VALUES ($userId, $resourceId, $status, $dateCompleted)
            ON CONFLICT(UserID, ResourceID)
            DO UPDATE SET Status = excluded.Status, DateCompleted = excluded.DateCompleted
            """, ("$userId", userId), ("$resourceId", resourceId), ("$status", status),
            ("$dateCompleted", dateCompleted ?? (object)DBNull.Value));
        AddActivity(userId, status == "Completed" ? "Completed Resource" : "Started Resource",
            $"{status}: {GetResource(resourceId)?.Title ?? "Learning resource"}");
    }

    public void AddActivity(int userId, string activityType, string description)
    {
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO ActivityHistory (UserID, ActivityType, Description, ActivityDate)
            VALUES ($userId, $type, $description, $date)
            """, ("$userId", userId), ("$type", activityType), ("$description", description),
            ("$date", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
    }

    public List<ActivityRecord> GetActivities(int? userId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ActivityID, UserID, ActivityType, Description, ActivityDate
            FROM ActivityHistory
            WHERE $userId IS NULL OR UserID = $userId
            ORDER BY ActivityDate DESC
            LIMIT 100
            """;
        command.Parameters.AddWithValue("$userId", userId.HasValue ? userId.Value : DBNull.Value);
        using var reader = command.ExecuteReader();
        var activities = new List<ActivityRecord>();
        while (reader.Read())
        {
            activities.Add(new ActivityRecord
            {
                ActivityID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                ActivityType = reader.GetString(2),
                Description = reader.GetString(3),
                ActivityDate = reader.GetString(4)
            });
        }

        return activities;
    }

    public List<QuizInfo> GetQuizzes()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT q.QuizID, q.SkillID, s.SkillName, q.QuizTitle, q.TotalMarks
            FROM Quiz q
            INNER JOIN Skills s ON s.SkillID = q.SkillID
            ORDER BY s.SkillName, q.QuizTitle
            """;
        using var reader = command.ExecuteReader();
        var quizzes = new List<QuizInfo>();
        while (reader.Read())
        {
            quizzes.Add(ReadQuiz(reader));
        }

        return quizzes;
    }

    public QuizInfo? GetQuiz(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT q.QuizID, q.SkillID, s.SkillName, q.QuizTitle, q.TotalMarks
            FROM Quiz q
            INNER JOIN Skills s ON s.SkillID = q.SkillID
            WHERE q.QuizID = $id
            """;
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadQuiz(reader) : null;
    }

    public void SaveQuiz(QuizInfo quiz)
    {
        using var connection = OpenConnection();
        if (quiz.QuizID == 0)
        {
            Execute(connection, "INSERT INTO Quiz (SkillID, QuizTitle, TotalMarks) VALUES ($skillId, $title, $marks)",
                ("$skillId", quiz.SkillID), ("$title", quiz.QuizTitle), ("$marks", quiz.TotalMarks));
        }
        else
        {
            Execute(connection, "UPDATE Quiz SET SkillID = $skillId, QuizTitle = $title, TotalMarks = $marks WHERE QuizID = $id",
                ("$id", quiz.QuizID), ("$skillId", quiz.SkillID), ("$title", quiz.QuizTitle), ("$marks", quiz.TotalMarks));
        }
    }

    public void DeleteQuiz(int id)
    {
        using var connection = OpenConnection();
        Execute(connection, "DELETE FROM Quiz WHERE QuizID = $id", ("$id", id));
    }

    public List<QuestionInfo> GetQuestions(int? quizId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT qs.QuestionID, qs.QuizID, q.QuizTitle, qs.QuestionText, qs.OptionA, qs.OptionB, qs.OptionC, qs.CorrectAnswer
            FROM Questions qs
            INNER JOIN Quiz q ON q.QuizID = qs.QuizID
            WHERE $quizId IS NULL OR qs.QuizID = $quizId
            ORDER BY q.QuizTitle, qs.QuestionID
            """;
        command.Parameters.AddWithValue("$quizId", quizId.HasValue ? quizId.Value : DBNull.Value);
        using var reader = command.ExecuteReader();
        var questions = new List<QuestionInfo>();
        while (reader.Read())
        {
            questions.Add(ReadQuestion(reader));
        }

        return questions;
    }

    public void SaveQuestion(QuestionInfo question)
    {
        using var connection = OpenConnection();
        if (question.QuestionID == 0)
        {
            Execute(connection, """
                INSERT INTO Questions (QuizID, QuestionText, OptionA, OptionB, OptionC, CorrectAnswer)
                VALUES ($quizId, $text, $a, $b, $c, $answer)
                """, ("$quizId", question.QuizID), ("$text", question.QuestionText), ("$a", question.OptionA),
                ("$b", question.OptionB), ("$c", question.OptionC), ("$answer", question.CorrectAnswer));
        }
        else
        {
            Execute(connection, """
                UPDATE Questions
                SET QuizID = $quizId, QuestionText = $text, OptionA = $a, OptionB = $b, OptionC = $c, CorrectAnswer = $answer
                WHERE QuestionID = $id
                """, ("$id", question.QuestionID), ("$quizId", question.QuizID), ("$text", question.QuestionText),
                ("$a", question.OptionA), ("$b", question.OptionB), ("$c", question.OptionC), ("$answer", question.CorrectAnswer));
        }
    }

    public void DeleteQuestion(int id)
    {
        using var connection = OpenConnection();
        Execute(connection, "DELETE FROM Questions WHERE QuestionID = $id", ("$id", id));
    }

    public int SaveQuizResult(int userId, int quizId, Dictionary<int, string> answers)
    {
        var questions = GetQuestions(quizId);
        var score = questions.Count(q => answers.TryGetValue(q.QuestionID, out var answer) && answer == q.CorrectAnswer);

        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO QuizResults (UserID, QuizID, Score, DateAttempted)
            VALUES ($userId, $quizId, $score, $date)
            """, ("$userId", userId), ("$quizId", quizId), ("$score", score),
            ("$date", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));

        var quizTitle = GetQuiz(quizId)?.QuizTitle ?? "Quiz";
        AddActivity(userId, "Attempted Quiz", $"Scored {score}/{questions.Count} in {quizTitle}");
        return score;
    }

    public List<QuizResultRecord> GetQuizResults(int? userId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT qr.ResultID, qr.UserID, u.FirstName || ' ' || u.LastName AS FullName,
                   qr.QuizID, q.QuizTitle, qr.Score, q.TotalMarks, qr.DateAttempted
            FROM QuizResults qr
            INNER JOIN Users u ON u.UserID = qr.UserID
            INNER JOIN Quiz q ON q.QuizID = qr.QuizID
            WHERE $userId IS NULL OR qr.UserID = $userId
            ORDER BY qr.DateAttempted DESC
            """;
        command.Parameters.AddWithValue("$userId", userId.HasValue ? userId.Value : DBNull.Value);
        using var reader = command.ExecuteReader();
        var results = new List<QuizResultRecord>();
        while (reader.Read())
        {
            results.Add(new QuizResultRecord
            {
                ResultID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                FullName = reader.GetString(2),
                QuizID = reader.GetInt32(3),
                QuizTitle = reader.GetString(4),
                Score = reader.GetInt32(5),
                TotalMarks = reader.GetInt32(6),
                DateAttempted = reader.GetString(7)
            });
        }

        return results;
    }

    public int CountRows(string tableName)
    {
        using var connection = OpenConnection();
        return Count(connection, tableName);
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        Execute(connection, "PRAGMA foreign_keys = ON;");
        return connection;
    }

    private static LearningResource ReadResource(SqliteDataReader reader)
    {
        return new LearningResource
        {
            ResourceID = reader.GetInt32(0),
            SkillID = reader.GetInt32(1),
            SkillName = reader.GetString(2),
            Title = reader.GetString(3),
            Type = reader.GetString(4),
            URL = reader.GetString(5),
            Description = reader.GetString(6)
        };
    }

    private static UserAccount ReadUser(SqliteDataReader reader)
    {
        return new UserAccount
        {
            UserID = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            Username = reader.GetString(4),
            Password = reader.GetString(5),
            Role = reader.GetString(6),
            DateRegistered = reader.GetString(7)
        };
    }

    private static QuizInfo ReadQuiz(SqliteDataReader reader)
    {
        return new QuizInfo
        {
            QuizID = reader.GetInt32(0),
            SkillID = reader.GetInt32(1),
            SkillName = reader.GetString(2),
            QuizTitle = reader.GetString(3),
            TotalMarks = reader.GetInt32(4)
        };
    }

    private static QuestionInfo ReadQuestion(SqliteDataReader reader)
    {
        return new QuestionInfo
        {
            QuestionID = reader.GetInt32(0),
            QuizID = reader.GetInt32(1),
            QuizTitle = reader.GetString(2),
            QuestionText = reader.GetString(3),
            OptionA = reader.GetString(4),
            OptionB = reader.GetString(5),
            OptionC = reader.GetString(6),
            CorrectAnswer = reader.GetString(7)
        };
    }

    private static int Count(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void Execute(SqliteConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Name, parameter.Value);
        }

        command.ExecuteNonQuery();
    }

    private static void Seed(SqliteConnection connection)
    {
        Execute(connection, """
            INSERT INTO Users (FirstName, LastName, Email, Username, Password, Role, DateRegistered)
            VALUES
            ('System', 'Admin', 'admin@skillbridge.local', 'admin', $adminPassword, 'Admin', $date),
            ('John', 'Smith', 'john@skillbridge.local', 'johnsmith99', $memberPassword, 'Member', $date);
            """, ("$adminPassword", PasswordHasher.Hash("Admin@123")), ("$memberPassword", PasswordHasher.Hash("Member@123")),
            ("$date", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));

        Execute(connection, """
            INSERT INTO Skills (SkillName, Description)
            VALUES
            ('Programming', 'Learn coding fundamentals, web development, and problem solving.'),
            ('Maths', 'Build confidence with numbers, formulas, logic, and calculations.'),
            ('Design', 'Explore visual design, layouts, typography, and creative tools.'),
            ('Communication', 'Improve presentations, writing, teamwork, and speaking skills.');
            """);

        Execute(connection, """
            INSERT INTO Resources (SkillID, Title, Type, URL, Description)
            VALUES
            (1, 'Intro to Python', 'Video', 'https://www.youtube.com/watch?v=kqtD5dpn9C8', 'A beginner-friendly Python programming video.'),
            (1, 'HTML and CSS Basics', 'Link', 'https://developer.mozilla.org/en-US/docs/Learn', 'MDN learning material for web development fundamentals.'),
            (2, 'Algebra Practice Guide', 'PDF', 'https://www.mathcentre.ac.uk/resources/uploaded/mc-ty-introductiontoalgebra-2009-1.pdf', 'A PDF guide covering basic algebra practice.'),
            (2, 'Khan Academy Maths', 'Video', 'https://www.khanacademy.org/math', 'Interactive maths lessons and practice videos.'),
            (3, 'Canva Design School', 'Link', 'https://www.canva.com/learn/design/', 'Articles and tutorials for design basics.'),
            (3, 'Design Principles Overview', 'Video', 'https://www.youtube.com/watch?v=YqQx75OPRa0', 'A video introduction to core graphic design principles.'),
            (4, 'Presentation Skills', 'Video', 'https://www.youtube.com/watch?v=MnIPpUiTcRc', 'Practical tips for better public speaking.'),
            (4, 'Effective Communication Guide', 'Link', 'https://www.skillsyouneed.com/ips/communication-skills.html', 'A written guide to communication skills.');
            """);

        Execute(connection, """
            INSERT INTO Quiz (SkillID, QuizTitle, TotalMarks)
            VALUES
            (1, 'Programming Basics Quiz', 5),
            (2, 'Maths Fundamentals Quiz', 5),
            (3, 'Design Basics Quiz', 5),
            (4, 'Communication Skills Quiz', 5);
            """);

        Execute(connection, """
            INSERT INTO Questions (QuizID, QuestionText, OptionA, OptionB, OptionC, CorrectAnswer)
            VALUES
            (1, 'Which language is commonly used for web page structure?', 'HTML', 'Python', 'SQL', 'A'),
            (1, 'Which symbol commonly starts a CSS class selector?', '#', '.', '@', 'B'),
            (1, 'What does a loop help a program do?', 'Repeat actions', 'Delete files only', 'Change screen brightness', 'A'),
            (1, 'Which database language is used to query records?', 'HTML', 'SQL', 'CSS', 'B'),
            (1, 'What is a variable used for?', 'Storing data', 'Drawing icons only', 'Sending emails only', 'A'),
            (2, 'What is 8 x 7?', '54', '56', '64', 'B'),
            (2, 'What is the value of x in x + 5 = 12?', '7', '17', '5', 'A'),
            (2, 'Which shape has three sides?', 'Circle', 'Triangle', 'Square', 'B'),
            (2, 'What is 25% of 100?', '10', '20', '25', 'C'),
            (2, 'Which number is even?', '9', '13', '18', 'C'),
            (3, 'Which principle deals with visual importance?', 'Hierarchy', 'Syntax', 'Compile', 'A'),
            (3, 'Which item is commonly used in brand identity?', 'Logo', 'Loop', 'Database key', 'A'),
            (3, 'What improves readability?', 'Clear typography', 'Random spacing', 'Hidden text', 'A'),
            (3, 'Which colour scheme uses opposite colours?', 'Complementary', 'Identical', 'Invisible', 'A'),
            (3, 'What is whitespace?', 'Empty space around elements', 'Only white paint', 'A database field', 'A'),
            (4, 'What is active listening?', 'Paying attention and responding thoughtfully', 'Interrupting often', 'Ignoring feedback', 'A'),
            (4, 'Which is useful in presentations?', 'Clear structure', 'No eye contact', 'Unreadable slides', 'A'),
            (4, 'What should teamwork include?', 'Communication and responsibility', 'Silence only', 'Avoiding tasks', 'A'),
            (4, 'Which tone is best for professional email?', 'Clear and respectful', 'Aggressive', 'Confusing', 'A'),
            (4, 'What helps reduce misunderstanding?', 'Asking clarifying questions', 'Guessing silently', 'Changing topic', 'A');
            """);
    }
}
