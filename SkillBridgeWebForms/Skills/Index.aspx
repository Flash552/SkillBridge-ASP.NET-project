<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SkillsRepeater.DataSource = SkillBridgeDb.GetSkills();
            SkillsRepeater.DataBind();
        }
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Skills - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <p class="eyebrow">Public Skills</p>
        <h1>Skill Categories</h1>
        <p class="lead">Choose a skill area to find related learning resources.</p>
    </div>

    <div class="row g-4">
        <asp:Repeater ID="SkillsRepeater" runat="server">
            <ItemTemplate>
                <div class="col-md-6">
                    <article class="section-card p-4 h-100">
                        <span class="skill-badge">Skill</span>
                        <h2 class="h4 mt-3"><%# Eval("SkillName") %></h2>
                        <p><%# Eval("Description") %></p>
                        <a class="btn btn-outline-primary" href='../Resources/Index.aspx?skillId=<%# Eval("SkillID") %>'>View Resources</a>
                    </article>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
