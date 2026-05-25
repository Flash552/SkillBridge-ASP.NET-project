<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireAdmin();
        if (!IsPostBack)
        {
            ResultsGrid.DataSource = SkillBridgeDb.GetQuizResults(null);
            ResultsGrid.DataBind();
        }
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Quiz Results - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Administrator Module</p>
        <h1>Quiz Results</h1>
    </div>

    <div class="table-panel p-3">
        <asp:GridView ID="ResultsGrid" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Member" />
                <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
                <asp:BoundField DataField="Score" HeaderText="Score" />
                <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" />
                <asp:BoundField DataField="DateAttempted" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
