<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        if (!IsPostBack)
        {
            QuizRepeater.DataSource = SkillBridgeDb.GetQuizzes();
            QuizRepeater.DataBind();
        }
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Quiz Centre - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Member Module</p>
        <h1>Quiz Centre</h1>
    </div>

    <div class="row g-4">
        <asp:Repeater ID="QuizRepeater" runat="server">
            <ItemTemplate>
                <div class="col-md-6">
                    <article class="section-card p-4 h-100">
                        <span class="skill-badge"><%# Eval("SkillName") %></span>
                        <h2 class="h4 mt-3"><%# Eval("QuizTitle") %></h2>
                        <p>Total marks: <%# Eval("TotalMarks") %></p>
                        <a class="btn btn-primary" href='AttemptQuiz.aspx?id=<%# Eval("QuizID") %>'>Attempt Quiz</a>
                    </article>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
