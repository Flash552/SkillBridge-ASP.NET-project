<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireAdmin();
        if (!IsPostBack) BindData();
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        int id;
        if (Int32.TryParse(SkillIdHidden.Value, out id))
        {
            SkillBridgeDb.UpdateSkill(id, NameTextBox.Text.Trim(), DescriptionTextBox.Text.Trim());
        }
        else
        {
            SkillBridgeDb.AddSkill(NameTextBox.Text.Trim(), DescriptionTextBox.Text.Trim());
        }
        ClearForm();
        BindData();
    }

    protected void SkillsRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Delete")
        {
            SkillBridgeDb.DeleteSkill(id);
        }
        else if (e.CommandName == "Edit")
        {
            Skill skill = SkillBridgeDb.GetSkill(id);
            SkillIdHidden.Value = skill.SkillID.ToString();
            NameTextBox.Text = skill.SkillName;
            DescriptionTextBox.Text = skill.Description;
            SaveButton.Text = "Update Skill";
        }
        BindData();
    }

    private void BindData()
    {
        SkillsRepeater.DataSource = SkillBridgeDb.GetSkills();
        SkillsRepeater.DataBind();
    }

    private void ClearForm()
    {
        SkillIdHidden.Value = "";
        NameTextBox.Text = "";
        DescriptionTextBox.Text = "";
        SaveButton.Text = "Save Skill";
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Manage Skills - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Skills</h1>
    <section class="form-panel mb-4">
        <asp:HiddenField ID="SkillIdHidden" runat="server" />
        <asp:ValidationSummary ID="Summary" runat="server" CssClass="validation-summary-errors" ValidationGroup="SkillGroup" />
        <label class="form-label">Skill Name</label>
        <asp:TextBox ID="NameTextBox" runat="server" CssClass="form-control mb-2" />
        <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="NameTextBox" ErrorMessage="Skill name is required." ValidationGroup="SkillGroup" CssClass="field-validation-error" />
        <label class="form-label mt-3">Description</label>
        <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="3" />
        <asp:RequiredFieldValidator ID="DescriptionRequired" runat="server" ControlToValidate="DescriptionTextBox" ErrorMessage="Description is required." ValidationGroup="SkillGroup" CssClass="field-validation-error" />
        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-3" Text="Save Skill" OnClick="SaveButton_Click" ValidationGroup="SkillGroup" />
    </section>

    <div class="table-panel p-3">
        <table class="table table-striped">
            <thead><tr><th>Name</th><th>Description</th><th>Actions</th></tr></thead>
            <tbody>
                <asp:Repeater ID="SkillsRepeater" runat="server" OnItemCommand="SkillsRepeater_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("SkillName") %></td>
                            <td><%# Eval("Description") %></td>
                            <td>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="Edit" CommandArgument='<%# Eval("SkillID") %>'>Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Delete" CommandArgument='<%# Eval("SkillID") %>' OnClientClick="return confirm('Delete this skill?');">Delete</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</asp:Content>
