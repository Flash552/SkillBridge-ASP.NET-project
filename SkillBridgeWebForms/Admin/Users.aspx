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
        if (Int32.TryParse(UserIdHidden.Value, out id))
        {
            SkillBridgeDb.UpdateUser(id, FirstNameTextBox.Text.Trim(), LastNameTextBox.Text.Trim(), EmailTextBox.Text.Trim(), UsernameTextBox.Text.Trim(), RoleList.SelectedValue);
        }
        else
        {
            SkillBridgeDb.AddUser(FirstNameTextBox.Text.Trim(), LastNameTextBox.Text.Trim(), EmailTextBox.Text.Trim(), UsernameTextBox.Text.Trim(), PasswordTextBox.Text, RoleList.SelectedValue);
        }
        ClearForm();
        BindData();
    }

    protected void UsersRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Delete")
        {
            SkillBridgeDb.DeleteUser(id);
        }
        else if (e.CommandName == "Edit")
        {
            UserAccount user = SkillBridgeDb.GetUser(id);
            UserIdHidden.Value = user.UserID.ToString();
            FirstNameTextBox.Text = user.FirstName;
            LastNameTextBox.Text = user.LastName;
            EmailTextBox.Text = user.Email;
            UsernameTextBox.Text = user.Username;
            RoleList.SelectedValue = user.Role;
            PasswordRequired.Enabled = false;
            SaveButton.Text = "Update User";
        }
        BindData();
    }

    private void BindData()
    {
        UsersRepeater.DataSource = SkillBridgeDb.GetUsers();
        UsersRepeater.DataBind();
    }

    private void ClearForm()
    {
        UserIdHidden.Value = "";
        FirstNameTextBox.Text = "";
        LastNameTextBox.Text = "";
        EmailTextBox.Text = "";
        UsernameTextBox.Text = "";
        PasswordTextBox.Text = "";
        RoleList.SelectedValue = "Member";
        PasswordRequired.Enabled = true;
        SaveButton.Text = "Save User";
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Manage Users - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Users</h1>
    <section class="form-panel mb-4">
        <asp:HiddenField ID="UserIdHidden" runat="server" />
        <asp:ValidationSummary ID="Summary" runat="server" CssClass="validation-summary-errors" ValidationGroup="UserGroup" />
        <div class="row g-3">
            <div class="col-md-6"><label class="form-label">First Name</label><asp:TextBox ID="FirstNameTextBox" runat="server" CssClass="form-control" /><asp:RequiredFieldValidator ID="FirstRequired" runat="server" ControlToValidate="FirstNameTextBox" ErrorMessage="First name is required." ValidationGroup="UserGroup" CssClass="field-validation-error" /></div>
            <div class="col-md-6"><label class="form-label">Last Name</label><asp:TextBox ID="LastNameTextBox" runat="server" CssClass="form-control" /><asp:RequiredFieldValidator ID="LastRequired" runat="server" ControlToValidate="LastNameTextBox" ErrorMessage="Last name is required." ValidationGroup="UserGroup" CssClass="field-validation-error" /></div>
            <div class="col-md-6"><label class="form-label">Email</label><asp:TextBox ID="EmailTextBox" runat="server" CssClass="form-control" /><asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Email is required." ValidationGroup="UserGroup" CssClass="field-validation-error" /><asp:RegularExpressionValidator ID="EmailRegex" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Valid email is required." ValidationGroup="UserGroup" CssClass="field-validation-error" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" /></div>
            <div class="col-md-6"><label class="form-label">Username</label><asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" /><asp:RequiredFieldValidator ID="UsernameRequired" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Username is required." ValidationGroup="UserGroup" CssClass="field-validation-error" /></div>
            <div class="col-md-6"><label class="form-label">Password</label><asp:TextBox ID="PasswordTextBox" runat="server" CssClass="form-control" TextMode="Password" /><asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Password is required for new users." ValidationGroup="UserGroup" CssClass="field-validation-error" /></div>
            <div class="col-md-6"><label class="form-label">Role</label><asp:DropDownList ID="RoleList" runat="server" CssClass="form-select"><asp:ListItem>Member</asp:ListItem><asp:ListItem>Admin</asp:ListItem></asp:DropDownList></div>
        </div>
        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-3" Text="Save User" OnClick="SaveButton_Click" ValidationGroup="UserGroup" />
    </section>

    <div class="table-panel p-3">
        <table class="table table-striped">
            <thead><tr><th>Name</th><th>Email</th><th>Username</th><th>Role</th><th>Actions</th></tr></thead>
            <tbody>
                <asp:Repeater ID="UsersRepeater" runat="server" OnItemCommand="UsersRepeater_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("FullName") %></td><td><%# Eval("Email") %></td><td><%# Eval("Username") %></td><td><%# Eval("Role") %></td>
                            <td><asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="Edit" CommandArgument='<%# Eval("UserID") %>'>Edit</asp:LinkButton> <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Delete" CommandArgument='<%# Eval("UserID") %>' OnClientClick="return confirm('Delete this user?');">Delete</asp:LinkButton></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</asp:Content>
