<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireAdmin();
        if (!IsPostBack)
        {
            BindSkills();
            BindData();
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        int id;
        if (Int32.TryParse(ResourceIdHidden.Value, out id))
        {
            SkillBridgeDb.UpdateResource(id, Convert.ToInt32(SkillList.SelectedValue), TitleTextBox.Text.Trim(), TypeList.SelectedValue, UrlTextBox.Text.Trim(), DescriptionTextBox.Text.Trim());
        }
        else
        {
            SkillBridgeDb.AddResource(Convert.ToInt32(SkillList.SelectedValue), TitleTextBox.Text.Trim(), TypeList.SelectedValue, UrlTextBox.Text.Trim(), DescriptionTextBox.Text.Trim());
        }
        ClearForm();
        BindData();
    }

    protected void ResourcesRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Delete")
        {
            SkillBridgeDb.DeleteResource(id);
        }
        else if (e.CommandName == "Edit")
        {
            LearningResource resource = SkillBridgeDb.GetResource(id);
            ResourceIdHidden.Value = resource.ResourceID.ToString();
            SkillList.SelectedValue = resource.SkillID.ToString();
            TitleTextBox.Text = resource.Title;
            TypeList.SelectedValue = resource.Type;
            UrlTextBox.Text = resource.Url;
            DescriptionTextBox.Text = resource.Description;
            SaveButton.Text = "Update Resource";
        }
        BindData();
    }

    private void BindSkills()
    {
        SkillList.DataSource = SkillBridgeDb.GetSkills();
        SkillList.DataTextField = "SkillName";
        SkillList.DataValueField = "SkillID";
        SkillList.DataBind();
    }

    private void BindData()
    {
        ResourcesRepeater.DataSource = SkillBridgeDb.GetResources(null);
        ResourcesRepeater.DataBind();
    }

    private void ClearForm()
    {
        ResourceIdHidden.Value = "";
        TitleTextBox.Text = "";
        UrlTextBox.Text = "";
        DescriptionTextBox.Text = "";
        TypeList.SelectedValue = "Link";
        SaveButton.Text = "Save Resource";
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Manage Resources - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Resources</h1>
    <section class="form-panel mb-4">
        <asp:HiddenField ID="ResourceIdHidden" runat="server" />
        <asp:ValidationSummary ID="Summary" runat="server" CssClass="validation-summary-errors" ValidationGroup="ResourceGroup" />
        <label class="form-label">Skill</label><asp:DropDownList ID="SkillList" runat="server" CssClass="form-select mb-2" />
        <label class="form-label">Title</label><asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="TitleRequired" runat="server" ControlToValidate="TitleTextBox" ErrorMessage="Title is required." ValidationGroup="ResourceGroup" CssClass="field-validation-error" />
        <label class="form-label mt-2">Type</label><asp:DropDownList ID="TypeList" runat="server" CssClass="form-select mb-2"><asp:ListItem>Video</asp:ListItem><asp:ListItem>PDF</asp:ListItem><asp:ListItem>Link</asp:ListItem></asp:DropDownList>
        <label class="form-label">URL</label><asp:TextBox ID="UrlTextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="UrlRequired" runat="server" ControlToValidate="UrlTextBox" ErrorMessage="URL is required." ValidationGroup="ResourceGroup" CssClass="field-validation-error" />
        <label class="form-label mt-2">Description</label><asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="3" /><asp:RequiredFieldValidator ID="DescriptionRequired" runat="server" ControlToValidate="DescriptionTextBox" ErrorMessage="Description is required." ValidationGroup="ResourceGroup" CssClass="field-validation-error" />
        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-3" Text="Save Resource" OnClick="SaveButton_Click" ValidationGroup="ResourceGroup" />
    </section>

    <div class="table-panel p-3">
        <table class="table table-striped"><thead><tr><th>Title</th><th>Skill</th><th>Type</th><th>Actions</th></tr></thead><tbody>
        <asp:Repeater ID="ResourcesRepeater" runat="server" OnItemCommand="ResourcesRepeater_ItemCommand">
            <ItemTemplate>
                <tr><td><%# Eval("Title") %></td><td><%# Eval("SkillName") %></td><td><%# Eval("Type") %></td><td><asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="Edit" CommandArgument='<%# Eval("ResourceID") %>'>Edit</asp:LinkButton> <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Delete" CommandArgument='<%# Eval("ResourceID") %>' OnClientClick="return confirm('Delete this resource?');">Delete</asp:LinkButton></td></tr>
            </ItemTemplate>
        </asp:Repeater>
        </tbody></table>
    </div>
</asp:Content>
