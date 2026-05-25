<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void LoginButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        UserAccount user = SkillBridgeDb.Login(UsernameTextBox.Text.Trim(), PasswordTextBox.Text);
        if (user == null)
        {
            MessageLabel.Text = "Invalid username or password.";
            return;
        }

        Session["UserID"] = user.UserID;
        Session["Username"] = user.Username;
        Session["Role"] = user.Role;
        Session["FullName"] = user.FullName;

        Response.Redirect(user.Role == "Admin" ? "~/Admin/Dashboard.aspx" : "~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Login - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="form-panel">
        <p class="eyebrow">Account</p>
        <h1>Login</h1>
        <asp:ValidationSummary ID="LoginSummary" runat="server" CssClass="validation-summary-errors" ValidationGroup="LoginGroup" />
        <asp:Label ID="MessageLabel" runat="server" CssClass="error-message" />

        <div class="mb-3">
            <label class="form-label" for="<%= UsernameTextBox.ClientID %>">Username</label>
            <asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="UsernameRequired" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Username is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="LoginGroup" />
        </div>
        <div class="mb-3">
            <label class="form-label" for="<%= PasswordTextBox.ClientID %>">Password</label>
            <asp:TextBox ID="PasswordTextBox" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Password is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="LoginGroup" />
        </div>
        <asp:Button ID="LoginButton" runat="server" CssClass="btn btn-primary" Text="Login" OnClick="LoginButton_Click" ValidationGroup="LoginGroup" />
    </section>
</asp:Content>
