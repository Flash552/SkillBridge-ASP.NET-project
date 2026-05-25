CREATE DATABASE SkillBridge;
GO

USE SkillBridge;
GO

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

CREATE TABLE Skills (
    SkillID INT IDENTITY(1,1) PRIMARY KEY,
    SkillName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(1000) NOT NULL
);

CREATE TABLE Resources (
    ResourceID INT IDENTITY(1,1) PRIMARY KEY,
    SkillID INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Type NVARCHAR(20) NOT NULL CHECK (Type IN ('Video', 'PDF', 'Link')),
    URL NVARCHAR(500) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    CONSTRAINT FK_Resources_Skills FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE
);

CREATE TABLE Progress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ResourceID INT NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('In Progress', 'Completed')),
    DateCompleted DATETIME NULL,
    CONSTRAINT UQ_Progress_UserResource UNIQUE (UserID, ResourceID),
    CONSTRAINT FK_Progress_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_Progress_Resources FOREIGN KEY (ResourceID) REFERENCES Resources(ResourceID) ON DELETE CASCADE
);

CREATE TABLE ActivityHistory (
    ActivityID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ActivityType NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    ActivityDate DATETIME NOT NULL,
    CONSTRAINT FK_Activity_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

CREATE TABLE Quiz (
    QuizID INT IDENTITY(1,1) PRIMARY KEY,
    SkillID INT NOT NULL,
    QuizTitle NVARCHAR(150) NOT NULL,
    TotalMarks INT NOT NULL,
    CONSTRAINT FK_Quiz_Skills FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE
);

CREATE TABLE Questions (
    QuestionID INT IDENTITY(1,1) PRIMARY KEY,
    QuizID INT NOT NULL,
    QuestionText NVARCHAR(500) NOT NULL,
    OptionA NVARCHAR(255) NOT NULL,
    OptionB NVARCHAR(255) NOT NULL,
    OptionC NVARCHAR(255) NOT NULL,
    CorrectAnswer NVARCHAR(1) NOT NULL CHECK (CorrectAnswer IN ('A', 'B', 'C')),
    CONSTRAINT FK_Questions_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE
);

CREATE TABLE QuizResults (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    QuizID INT NOT NULL,
    Score INT NOT NULL,
    DateAttempted DATETIME NOT NULL,
    CONSTRAINT FK_QuizResults_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_QuizResults_Quiz FOREIGN KEY (QuizID) REFERENCES Quiz(QuizID) ON DELETE CASCADE
);

INSERT INTO Users (FirstName, LastName, Email, Username, Password, Role, DateRegistered)
VALUES
('System', 'Admin', 'admin@skillbridge.local', 'admin', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Admin@123'), 2), 'Admin', GETDATE()),
('John', 'Smith', 'johnsmith99@example.com', 'johnsmith99', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Member@123'), 2), 'Member', GETDATE());

INSERT INTO Skills (SkillName, Description)
VALUES
('Web Development', 'Build interactive websites with HTML5, CSS, JavaScript, and server-side programming.'),
('Database Design', 'Understand relational database design, SQL queries, and data-driven applications.'),
('Programming Basics', 'Practice problem solving, variables, control flow, and reusable code.'),
('Digital Productivity', 'Use digital tools to organize tasks, communicate clearly, and learn independently.');

INSERT INTO Resources (SkillID, Title, Type, URL, Description)
VALUES
(1, 'HTML5 Semantic Layout Video', 'Video', 'https://www.youtube.com/embed/UB1O30fR-EE', 'An embedded video introduction to semantic HTML5 page structure.'),
(1, 'CSS Layout Reference', 'Link', 'https://developer.mozilla.org/en-US/docs/Learn/CSS/CSS_layout', 'MDN guide for modern CSS layout.'),
(2, 'SQL Server SELECT Tutorial', 'Video', 'https://www.youtube.com/embed/HXV3zeQKqGY', 'Video lesson for writing SELECT queries.'),
(2, 'Database Normalization Notes', 'PDF', 'https://www3.ntu.edu.sg/home/ehchua/programming/sql/Relational_Database_Design.pdf', 'PDF notes for relational database normalization.'),
(3, 'C# Basics Guide', 'Link', 'https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/', 'Microsoft Learn introduction to C#.'),
(4, 'Study Planning Image', 'Link', 'Images/learning-dashboard.svg', 'A local image resource used to demonstrate multimedia in the website.');

INSERT INTO Quiz (SkillID, QuizTitle, TotalMarks)
VALUES
(1, 'HTML5 Fundamentals', 3),
(2, 'SQL Basics', 3),
(3, 'C# Basics', 3);

INSERT INTO Questions (QuizID, QuestionText, OptionA, OptionB, OptionC, CorrectAnswer)
VALUES
(1, 'Which element represents a standalone article?', 'section', 'article', 'div', 'B'),
(1, 'Which attribute provides alternative text for images?', 'alt', 'title', 'src', 'A'),
(1, 'Which element is used for page navigation links?', 'nav', 'aside', 'footer', 'A'),
(2, 'Which SQL command reads data?', 'SELECT', 'INSERT', 'DELETE', 'A'),
(2, 'Which key uniquely identifies a row?', 'Foreign key', 'Primary key', 'Candidate text', 'B'),
(2, 'Which clause filters records?', 'WHERE', 'ORDER BY', 'GROUP BY', 'A'),
(3, 'Which keyword declares a variable with explicit type?', 'int', 'loop', 'return', 'A'),
(3, 'Which statement repeats while a condition is true?', 'if', 'while', 'class', 'B'),
(3, 'Which type stores true or false?', 'bool', 'string', 'decimal', 'A');
