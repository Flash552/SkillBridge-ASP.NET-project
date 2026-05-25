<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        if (!IsPostBack)
        {
            UserAccount user = SkillBridgeDb.GetUser(Convert.ToInt32(Session["UserID"]));
            FirstNameTextBox.Text = user.FirstName;
            LastNameTextBox.Text = user.LastName;
            EmailTextBox.Text = user.Email;
            UsernameTextBox.Text = user.Username;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        int userId = Convert.ToInt32(Session["UserID"]);
        if (SkillBridgeDb.UsernameExists(UsernameTextBox.Text.Trim(), userId))
        {
            MessageLabel.Text = "Username is already used.";
            return;
        }

        if (SkillBridgeDb.EmailExists(EmailTextBox.Text.Trim(), userId))
        {
            MessageLabel.Text = "Email is already used.";
            return;
        }

        SkillBridgeDb.UpdateUserProfile(userId, FirstNameTextBox.Text.Trim(), LastNameTextBox.Text.Trim(), EmailTextBox.Text.Trim(), UsernameTextBox.Text.Trim());
        Session["FullName"] = FirstNameTextBox.Text.Trim() + " " + LastNameTextBox.Text.Trim();
        MessageLabel.CssClass = "success-message d-block";
        MessageLabel.Text = "Profile updated.";
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Profile - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="form-panel">
        <p class="eyebrow">Member Module</p>
        <h1>Edit Profile</h1>
        <asp:ValidationSummary ID="ProfileSummary" runat="server" CssClass="validation-summary-errors" ValidationGroup="ProfileGroup" />
        <asp:Label ID="MessageLabel" runat="server" CssClass="error-message d-block" />

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label" for="<%= FirstNameTextBox.ClientID %>">First Name</label>
                <asp:TextBox ID="FirstNameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" ControlToValidate="FirstNameTextBox" ErrorMessage="First name is required." CssClass="field-validation-error" ValidationGroup="ProfileGroup" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= LastNameTextBox.ClientID %>">Last Name</label>
                <asp:TextBox ID="LastNameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" ControlToValidate="LastNameTextBox" ErrorMessage="Last name is required." CssClass="field-validation-error" ValidationGroup="ProfileGroup" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= EmailTextBox.ClientID %>">Email</label>
                <asp:TextBox ID="EmailTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Email is required." CssClass="field-validation-error" ValidationGroup="ProfileGroup" />
                <asp:RegularExpressionValidator ID="EmailRegex" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Enter a valid email address." CssClass="field-validation-error" ValidationGroup="ProfileGroup" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
            </div>
            <div class="col-md-6">
                <label class="form-label" for="<%= UsernameTextBox.ClientID %>">Username</label>
                <asp:TextBox ID="UsernameTextBox" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="UsernameRequired" runat="server" ControlToValidate="UsernameTextBox" ErrorMessage="Username is required." CssClass="field-validation-error" ValidationGroup="ProfileGroup" />
            </div>
        </div>

        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-4" Text="Save Profile" OnClick="SaveButton_Click" ValidationGroup="ProfileGroup" />
    </section>
</asp:Content>
