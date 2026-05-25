<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        if (!IsPostBack)
        {
            ProgressGrid.DataSource = SkillBridgeDb.GetProgressForUser(Convert.ToInt32(Session["UserID"]));
            ProgressGrid.DataBind();
        }
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">My Progress - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Member Module</p>
        <h1>My Progress</h1>
    </div>

    <div class="table-panel p-3">
        <asp:GridView ID="ProgressGrid" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="ResourceTitle" HeaderText="Resource" />
                <asp:BoundField DataField="SkillName" HeaderText="Skill" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:BoundField DataField="DateCompleted" HeaderText="Date Completed" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
