<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SkillList.DataSource = SkillBridgeDb.GetSkills();
            SkillList.DataTextField = "SkillName";
            SkillList.DataValueField = "SkillID";
            SkillList.DataBind();
            SkillList.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All skills", ""));

            string skillId = Request.QueryString["skillId"];
            if (!String.IsNullOrEmpty(skillId) && SkillList.Items.FindByValue(skillId) != null)
            {
                SkillList.SelectedValue = skillId;
            }

            BindResources();
        }
    }

    protected void FilterButton_Click(object sender, EventArgs e)
    {
        BindResources();
    }

    private void BindResources()
    {
        int parsed;
        int? skillId = Int32.TryParse(SkillList.SelectedValue, out parsed) ? (int?)parsed : null;
        ResourcesRepeater.DataSource = SkillBridgeDb.GetResources(skillId);
        ResourcesRepeater.DataBind();
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Resources - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Learning Resources</p>
        <h1>Browse Digital Learning Resources</h1>
    </div>

    <div class="section-card p-4 mb-4">
        <label class="form-label" for="<%= SkillList.ClientID %>">Filter by skill</label>
        <div class="d-flex gap-2">
            <asp:DropDownList ID="SkillList" runat="server" CssClass="form-select" />
            <asp:Button ID="FilterButton" runat="server" CssClass="btn btn-primary" Text="Filter" OnClick="FilterButton_Click" />
        </div>
    </div>

    <div class="row g-4">
        <asp:Repeater ID="ResourcesRepeater" runat="server">
            <ItemTemplate>
                <div class="col-md-6">
                    <article class="resource-card p-4 h-100">
                        <span class='type-pill type-<%# Eval("Type").ToString().ToLower() %>'><%# Eval("Type") %></span>
                        <span class="skill-badge ms-2"><%# Eval("SkillName") %></span>
                        <h2 class="h4 mt-3"><%# Eval("Title") %></h2>
                        <p><%# Eval("Description") %></p>
                        <a class="btn btn-outline-primary" href='Details.aspx?id=<%# Eval("ResourceID") %>'>View Details</a>
                    </article>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
