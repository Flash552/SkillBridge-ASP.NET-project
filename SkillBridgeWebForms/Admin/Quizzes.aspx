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
        int marks = Convert.ToInt32(MarksTextBox.Text);
        if (Int32.TryParse(QuizIdHidden.Value, out id))
        {
            SkillBridgeDb.UpdateQuiz(id, Convert.ToInt32(SkillList.SelectedValue), TitleTextBox.Text.Trim(), marks);
        }
        else
        {
            SkillBridgeDb.AddQuiz(Convert.ToInt32(SkillList.SelectedValue), TitleTextBox.Text.Trim(), marks);
        }
        ClearForm();
        BindData();
    }

    protected void QuizzesRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Delete")
        {
            SkillBridgeDb.DeleteQuiz(id);
        }
        else if (e.CommandName == "Edit")
        {
            QuizInfo quiz = SkillBridgeDb.GetQuiz(id);
            QuizIdHidden.Value = quiz.QuizID.ToString();
            SkillList.SelectedValue = quiz.SkillID.ToString();
            TitleTextBox.Text = quiz.QuizTitle;
            MarksTextBox.Text = quiz.TotalMarks.ToString();
            SaveButton.Text = "Update Quiz";
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
        QuizzesRepeater.DataSource = SkillBridgeDb.GetQuizzes();
        QuizzesRepeater.DataBind();
    }

    private void ClearForm()
    {
        QuizIdHidden.Value = "";
        TitleTextBox.Text = "";
        MarksTextBox.Text = "";
        SaveButton.Text = "Save Quiz";
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Manage Quizzes - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Quizzes</h1>
    <section class="form-panel mb-4">
        <asp:HiddenField ID="QuizIdHidden" runat="server" />
        <asp:ValidationSummary ID="Summary" runat="server" CssClass="validation-summary-errors" ValidationGroup="QuizGroup" />
        <label class="form-label">Skill</label><asp:DropDownList ID="SkillList" runat="server" CssClass="form-select mb-2" />
        <label class="form-label">Quiz Title</label><asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="TitleRequired" runat="server" ControlToValidate="TitleTextBox" ErrorMessage="Quiz title is required." ValidationGroup="QuizGroup" CssClass="field-validation-error" />
        <label class="form-label">Total Marks</label><asp:TextBox ID="MarksTextBox" runat="server" CssClass="form-control mb-2" TextMode="Number" /><asp:RequiredFieldValidator ID="MarksRequired" runat="server" ControlToValidate="MarksTextBox" ErrorMessage="Total marks is required." ValidationGroup="QuizGroup" CssClass="field-validation-error" /><asp:RangeValidator ID="MarksRange" runat="server" ControlToValidate="MarksTextBox" MinimumValue="1" MaximumValue="100" Type="Integer" ErrorMessage="Marks must be 1 to 100." ValidationGroup="QuizGroup" CssClass="field-validation-error" />
        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-3" Text="Save Quiz" OnClick="SaveButton_Click" ValidationGroup="QuizGroup" />
    </section>

    <div class="table-panel p-3">
        <table class="table table-striped"><thead><tr><th>Title</th><th>Skill</th><th>Marks</th><th>Actions</th></tr></thead><tbody>
        <asp:Repeater ID="QuizzesRepeater" runat="server" OnItemCommand="QuizzesRepeater_ItemCommand">
            <ItemTemplate>
                <tr><td><%# Eval("QuizTitle") %></td><td><%# Eval("SkillName") %></td><td><%# Eval("TotalMarks") %></td><td><asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="Edit" CommandArgument='<%# Eval("QuizID") %>'>Edit</asp:LinkButton> <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Delete" CommandArgument='<%# Eval("QuizID") %>' OnClientClick="return confirm('Delete this quiz?');">Delete</asp:LinkButton></td></tr>
            </ItemTemplate>
        </asp:Repeater>
        </tbody></table>
    </div>
</asp:Content>
