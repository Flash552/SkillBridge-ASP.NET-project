<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void RegisterButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        if (SkillBridgeDb.UsernameExists(UsernameTextBox.Text.Trim(), null))
        {
            MessageLabel.Text = "Username is already used.";
            return;
        }

        if (SkillBridgeDb.EmailExists(EmailTextBox.Text.Trim(), null))
        {
            MessageLabel.Text = "Email is already used.";
            return;
        }

        SkillBridgeDb.AddUser(FirstNameTextBox.Text.Trim(), LastNameTextBox.Text.Trim(), EmailTextBox.Text.Trim(),
            UsernameTextBox.Text.Trim(), PasswordTextBox.Text, "Member");

        Response.Redirect("~/Account/Login.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Register - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="form-panel">
        <p class="eyebrow">New Member</p>
        <h1>Create Account</h1>
        <asp:ValidationSummary ID="RegisterSummary" runat="server" CssClass="validation-summary-errors" ValidationGroup="RegisterGroup" />
        <asp:Label ID="MessageLabel" runat="server" CssClass="error-message" />

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label" for="<%= FirstNameTextBox.ClientID %>">First Name</label>
                <asp:TextBox ID="FirstNameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" ControlToValidate="FirstNameTextBox" ErrorMessage="First name is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= LastNameTextBox.ClientID %>">Last Name</label>
                <asp:TextBox ID="LastNameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" ControlToValidate="LastNameTextBox" ErrorMessage="Last name is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= EmailTextBox.ClientID %>">Email</label>
                <asp:TextBox ID="EmailTextBox" runat="server" CssClass="form-control" TextMode="Email" />
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Email is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
                <asp:RegularExpressionValidator ID="EmailRegex" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Enter a valid email address." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= UsernameTextBox.ClientID %>">Username</label>
                <asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="UsernameRequired" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Username is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= PasswordTextBox.ClientID %>">Password</label>
                <asp:TextBox ID="PasswordTextBox" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Password is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
                <asp:RegularExpressionValidator ID="PasswordRegex" runat="server" ControlToValidate="PasswordTextBox" ErrorMessage="Password must be at least 8 characters and include uppercase, lowercase, number, and symbol." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= ConfirmPasswordTextBox.ClientID %>">Confirm Password</label>
                <asp:TextBox ID="ConfirmPasswordTextBox" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator ID="ConfirmRequired" runat="server" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="Confirm password is required." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToValidate="ConfirmPasswordTextBox" ControlToCompare="PasswordTextBox" ErrorMessage="Passwords must match." CssClass="field-validation-error" Display="Dynamic" ValidationGroup="RegisterGroup" />
            </div>
        </div>

        <asp:Button ID="RegisterButton" runat="server" CssClass="btn btn-primary mt-4" Text="Register" OnClick="RegisterButton_Click" ValidationGroup="RegisterGroup" />
    </section>
</asp:Content>
