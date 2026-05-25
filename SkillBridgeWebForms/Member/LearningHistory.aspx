<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        if (!IsPostBack)
        {
            HistoryGrid.DataSource = SkillBridgeDb.GetActivities(Convert.ToInt32(Session["UserID"]));
            HistoryGrid.DataBind();
        }
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Learning History - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Member Module</p>
        <h1>Learning History</h1>
    </div>

    <div class="table-panel p-3">
        <asp:GridView ID="HistoryGrid" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="ActivityType" HeaderText="Type" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="ActivityDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
