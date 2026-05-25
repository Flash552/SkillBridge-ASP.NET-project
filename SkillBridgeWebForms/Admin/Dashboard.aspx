<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireAdmin();
        if (!IsPostBack)
        {
            UsersCount.Text = SkillBridgeDb.Count("Users").ToString();
            SkillsCount.Text = SkillBridgeDb.Count("Skills").ToString();
            ResourcesCount.Text = SkillBridgeDb.Count("Resources").ToString();
            QuizzesCount.Text = SkillBridgeDb.Count("Quiz").ToString();
        }
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Admin Dashboard - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Administrator Module</p>
        <h1>Admin Dashboard</h1>
    </div>

    <div class="row g-4">
        <div class="col-md-3"><div class="dashboard-card p-4"><h2><asp:Label ID="UsersCount" runat="server" /></h2><p>Users</p></div></div>
        <div class="col-md-3"><div class="dashboard-card p-4"><h2><asp:Label ID="SkillsCount" runat="server" /></h2><p>Skills</p></div></div>
        <div class="col-md-3"><div class="dashboard-card p-4"><h2><asp:Label ID="ResourcesCount" runat="server" /></h2><p>Resources</p></div></div>
        <div class="col-md-3"><div class="dashboard-card p-4"><h2><asp:Label ID="QuizzesCount" runat="server" /></h2><p>Quizzes</p></div></div>
    </div>

    <div class="section-card p-4 mt-4">
        <a class="btn btn-primary me-2 mb-2" href="Users.aspx">Manage Users</a>
        <a class="btn btn-primary me-2 mb-2" href="Skills.aspx">Manage Skills</a>
        <a class="btn btn-primary me-2 mb-2" href="Resources.aspx">Manage Resources</a>
        <a class="btn btn-primary me-2 mb-2" href="Quizzes.aspx">Manage Quizzes</a>
        <a class="btn btn-primary me-2 mb-2" href="Questions.aspx">Manage Questions</a>
        <a class="btn btn-outline-primary mb-2" href="Results.aspx">View Results</a>
    </div>
</asp:Content>
