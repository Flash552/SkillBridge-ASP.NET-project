# SkillBridge Web Forms

This folder contains the ASP.NET Web Forms rebuild for the WAPP assignment.

## Requirements

- Windows with Visual Studio
- ASP.NET and web development workload
- SQL Server Express or LocalDB
- .NET Framework 4.7.2 developer pack

## Setup

1. Open SQL Server Management Studio or Visual Studio SQL Server Object Explorer.
2. Run `Database/SkillBridge.sql` to create the `SkillBridge` database and seed demo data.
3. Confirm the `SkillBridgeConnection` connection string in `Web.config` matches your SQL Server instance.
4. Open this folder as an ASP.NET Web Site or add it to a Web Forms solution.
5. Run the site with IIS Express and open `Default.aspx`.

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

## Assignment Checklist

- `.aspx` Web Forms pages
- `Site.Master` shared layout
- SQL Server database with ADO.NET
- `Web.config` connection string
- Insert, display, update, and delete operations
- Registration page
- Member module
- Admin module
- RequiredFieldValidator, RegularExpressionValidator, CompareValidator, and RangeValidator examples
- External, internal, and inline CSS
- Embedded image and video multimedia examples
