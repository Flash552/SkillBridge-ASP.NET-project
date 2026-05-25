<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        if (!IsPostBack)
        {
            NameLabel.Text = Convert.ToString(Session["FullName"]);
            ProgressCount.Text = SkillBridgeDb.CountForUser("Progress", Convert.ToInt32(Session["UserID"])).ToString();
            QuizCount.Text = SkillBridgeDb.CountForUser("QuizResults", Convert.ToInt32(Session["UserID"])).ToString();
            HistoryCount.Text = SkillBridgeDb.CountForUser("ActivityHistory", Convert.ToInt32(Session["UserID"])).ToString();
        }
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Member Dashboard - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-heading">
        <p class="eyebrow">Member Dashboard</p>
        <h1>Welcome, <asp:Label ID="NameLabel" runat="server" /></h1>
    </section>

    <div class="row g-4">
        <div class="col-md-4"><div class="dashboard-card p-4"><h2><asp:Label ID="ProgressCount" runat="server" /></h2><p>Progress records</p></div></div>
        <div class="col-md-4"><div class="dashboard-card p-4"><h2><asp:Label ID="QuizCount" runat="server" /></h2><p>Quiz attempts</p></div></div>
        <div class="col-md-4"><div class="dashboard-card p-4"><h2><asp:Label ID="HistoryCount" runat="server" /></h2><p>History records</p></div></div>
    </div>

    <div class="section-card p-4 mt-4">
        <a class="btn btn-primary me-2" href="QuizCentre.aspx">Attempt Quiz</a>
        <a class="btn btn-outline-primary me-2" href="MyProgress.aspx">View Progress</a>
        <a class="btn btn-outline-primary" href="Profile.aspx">Edit Profile</a>
    </div>
</asp:Content>
