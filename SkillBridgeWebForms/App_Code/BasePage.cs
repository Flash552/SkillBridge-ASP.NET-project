using System;
using System.Web.UI;

public class BasePage : Page
{
    protected int CurrentUserId
    {
        get { return Session["UserID"] == null ? 0 : Convert.ToInt32(Session["UserID"]); }
    }

    protected string CurrentRole
    {
        get { return Convert.ToString(Session["Role"]); }
    }

    protected string CurrentFullName
    {
        get { return Convert.ToString(Session["FullName"]); }
    }

    protected bool IsLoggedIn
    {
        get { return CurrentUserId > 0; }
    }

    protected bool IsMember
    {
        get { return CurrentRole == "Member"; }
    }

    protected bool IsAdmin
    {
        get { return CurrentRole == "Admin"; }
    }

    protected void RequireMember()
    {
        if (!IsLoggedIn)
        {
            Response.Redirect("~/Account/Login.aspx", true);
        }

        if (!IsMember)
        {
            Response.Redirect("~/Admin/Dashboard.aspx", true);
        }
    }

    protected void RequireAdmin()
    {
        if (!IsLoggedIn)
        {
            Response.Redirect("~/Account/Login.aspx", true);
        }

        if (!IsAdmin)
        {
            Response.Redirect("~/Member/Dashboard.aspx", true);
        }
    }

    protected void SignIn(UserAccount user)
    {
        Session["UserID"] = user.UserID;
        Session["Username"] = user.Username;
        Session["Role"] = user.Role;
        Session["FullName"] = user.FullName;
    }

    protected void SignOut()
    {
        Session.Clear();
        Session.Abandon();
    }
}
