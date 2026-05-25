# SkillBridge

SkillBridge is an ASP.NET Core Razor Pages learning platform built for the CT050-3-2 WAPP Web Applications assignment. The app helps users browse digital learning resources by skill area instead of course name.

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

- ASP.NET Core Razor Pages
- C#
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

The SQLite database file is created automatically as `skillbridge.db` when the app starts.

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

## How To Run

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
- HTML5 elements
- External CSS and Bootstrap styling
- Local database connectivity
- Insert, display, update, and delete operations
- Registration page
- Registered member module
- Administrator module
- Form validation
- Navigation support
- Multimedia learning resources
