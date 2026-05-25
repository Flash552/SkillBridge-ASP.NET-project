# SkillBridge

SkillBridge is a web-based learning platform built for the CT050-3-2 WAPP Web Applications assignment. The app helps users browse digital learning resources by skill area instead of course name.

The repository now contains two versions:

- `SkillBridgeWebForms/`: the assignment-ready ASP.NET Web Forms version using `.aspx` pages, `Site.Master`, SQL Server, ADO.NET, `Web.config`, and Web Forms validators.
- Root ASP.NET Core Razor Pages app: the earlier working prototype using SQLite.

## Features

- Guest browsing for public skills and learning resources
- Member registration and login using session variables
- Member dashboard with progress, quiz centre, learning history, and profile editing
- Admin dashboard with CRUD management pages
- SQLite local database connectivity
- Quizzes with scores saved in the database
- Multimedia resource types: videos, PDFs, and external links
- Form validation and role-based page protection
- Shared layout with Bootstrap and custom CSS styling

## User Roles

- Guest: can browse public skills and resources.
- Member: can log in, attempt quizzes, track progress, view history, and edit profile.
- Admin: can manage users, skills, resources, quizzes, questions, results, and activity records.

## Technology Used

- ASP.NET Web Forms
- ASP.NET Core Razor Pages
- C#
- SQL Server
- ADO.NET
- SQLite
- Microsoft.Data.Sqlite
- Bootstrap
- HTML5, CSS, and JavaScript

## Database Tables

- Users
- Skills
- Resources
- Progress
- ActivityHistory
- Quiz
- Questions
- QuizResults

The Web Forms version uses SQL Server tables created by `SkillBridgeWebForms/Database/SkillBridge.sql`.

The older Razor Pages prototype creates a SQLite database file named `skillbridge.db` when it starts.

## Demo Accounts

Admin:

```text
Username: admin
Password: Admin@123
```

Member:

```text
Username: johnsmith99
Password: Member@123
```

## How To Run The Web Forms Version

Classic ASP.NET Web Forms requires Windows with Visual Studio, .NET Framework, IIS Express, and SQL Server/LocalDB. It does not run with `dotnet run` on macOS.

1. Install Visual Studio 2022 on Windows.
2. Select the **ASP.NET and web development** workload.
3. Make sure SQL Server Express LocalDB and the .NET Framework 4.7.2 Developer Pack are installed.
4. Open Visual Studio, then choose **File > Open > Web Site**.
5. Select the `SkillBridgeWebForms` folder.
6. Open SQL Server Object Explorer and connect to `(LocalDB)\MSSQLLocalDB`.
7. Run `SkillBridgeWebForms/Database/SkillBridge.sql`.
8. Confirm `SkillBridgeWebForms/Web.config` contains the correct `SkillBridgeConnection` connection string.
9. Right-click `Default.aspx`, choose **Set As Start Page**, then run with IIS Express.

Web Forms common pages:

```text
/Default.aspx
/Account/Login.aspx
/Resources/Index.aspx
/Member/Dashboard.aspx
/Admin/Dashboard.aspx
```

## How To Run The Razor Prototype

Make sure the .NET SDK is installed, then run:

```bash
dotnet restore
dotnet run
```

Open the localhost URL shown in the terminal.

Common pages:

```text
/Account/Login
/Resources
/Member/Dashboard
/Admin/Dashboard
```

## Project Structure

```text
SkillBridgeWebForms/    ASP.NET Web Forms assignment version
  App_Code/             ADO.NET helper, models, password hashing, shared helpers
  Account/              Login, register, logout pages
  Admin/                Admin dashboard and CRUD pages
  Member/               Member dashboard, quizzes, progress, history, profile
  Resources/            Public resource browsing and details
  Skills/               Public skill categories
  Content/              External CSS
  Database/             SQL Server schema and seed script
  Images/               Multimedia assets

Data/                 Database helper and password hashing
Models/               Application models
Pages/Account/        Register, login, logout
Pages/Admin/          Admin dashboard and CRUD pages
Pages/Member/         Member dashboard, quizzes, progress, history, profile
Pages/Resources/      Public resource browsing and details
Pages/Skills/         Public skill categories
Pages/Shared/         Shared layout
wwwroot/css/          Custom styling
```

## Assignment Requirements Covered

- Interlinked webpages
- ASP.NET Web Forms `.aspx` pages
- Master Page layout using `Site.Master`
- SQL Server database connectivity using ADO.NET
- `Web.config` connection string
- HTML5 elements
- External, internal, and inline CSS
- Insert, display, update, and delete operations
- Registration page
- Registered member module
- Administrator module
- Web Forms validation controls
- Navigation support
- Embedded multimedia learning resources
