<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        RequireAdmin();
        if (!IsPostBack)
        {
            BindQuizzes();
            BindData();
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        int id;
        if (Int32.TryParse(QuestionIdHidden.Value, out id))
        {
            SkillBridgeDb.UpdateQuestion(id, Convert.ToInt32(QuizList.SelectedValue), QuestionTextBox.Text.Trim(), OptionATextBox.Text.Trim(), OptionBTextBox.Text.Trim(), OptionCTextBox.Text.Trim(), CorrectList.SelectedValue);
        }
        else
        {
            SkillBridgeDb.AddQuestion(Convert.ToInt32(QuizList.SelectedValue), QuestionTextBox.Text.Trim(), OptionATextBox.Text.Trim(), OptionBTextBox.Text.Trim(), OptionCTextBox.Text.Trim(), CorrectList.SelectedValue);
        }
        ClearForm();
        BindData();
    }

    protected void QuestionsRepeater_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "Delete")
        {
            SkillBridgeDb.DeleteQuestion(id);
        }
        else if (e.CommandName == "Edit")
        {
            QuestionInfo question = null;
            foreach (QuestionInfo item in SkillBridgeDb.GetQuestions(null))
            {
                if (item.QuestionID == id) question = item;
            }

            if (question != null)
            {
                QuestionIdHidden.Value = question.QuestionID.ToString();
                QuizList.SelectedValue = question.QuizID.ToString();
                QuestionTextBox.Text = question.QuestionText;
                OptionATextBox.Text = question.OptionA;
                OptionBTextBox.Text = question.OptionB;
                OptionCTextBox.Text = question.OptionC;
                CorrectList.SelectedValue = question.CorrectAnswer;
                SaveButton.Text = "Update Question";
            }
        }
        BindData();
    }

    private void BindQuizzes()
    {
        QuizList.DataSource = SkillBridgeDb.GetQuizzes();
        QuizList.DataTextField = "QuizTitle";
        QuizList.DataValueField = "QuizID";
        QuizList.DataBind();
    }

    private void BindData()
    {
        QuestionsRepeater.DataSource = SkillBridgeDb.GetQuestions(null);
        QuestionsRepeater.DataBind();
    }

    private void ClearForm()
    {
        QuestionIdHidden.Value = "";
        QuestionTextBox.Text = "";
        OptionATextBox.Text = "";
        OptionBTextBox.Text = "";
        OptionCTextBox.Text = "";
        CorrectList.SelectedValue = "A";
        SaveButton.Text = "Save Question";
    }

    private void RequireAdmin()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Admin") Response.Redirect("~/Member/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Manage Questions - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Manage Questions</h1>
    <section class="form-panel mb-4">
        <asp:HiddenField ID="QuestionIdHidden" runat="server" />
        <asp:ValidationSummary ID="Summary" runat="server" CssClass="validation-summary-errors" ValidationGroup="QuestionGroup" />
        <label class="form-label">Quiz</label><asp:DropDownList ID="QuizList" runat="server" CssClass="form-select mb-2" />
        <label class="form-label">Question</label><asp:TextBox ID="QuestionTextBox" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="3" /><asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="QuestionTextBox" ErrorMessage="Question is required." ValidationGroup="QuestionGroup" CssClass="field-validation-error" />
        <label class="form-label">Option A</label><asp:TextBox ID="OptionATextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="ARequired" runat="server" ControlToValidate="OptionATextBox" ErrorMessage="Option A is required." ValidationGroup="QuestionGroup" CssClass="field-validation-error" />
        <label class="form-label">Option B</label><asp:TextBox ID="OptionBTextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="BRequired" runat="server" ControlToValidate="OptionBTextBox" ErrorMessage="Option B is required." ValidationGroup="QuestionGroup" CssClass="field-validation-error" />
        <label class="form-label">Option C</label><asp:TextBox ID="OptionCTextBox" runat="server" CssClass="form-control mb-2" /><asp:RequiredFieldValidator ID="CRequired" runat="server" ControlToValidate="OptionCTextBox" ErrorMessage="Option C is required." ValidationGroup="QuestionGroup" CssClass="field-validation-error" />
        <label class="form-label">Correct Answer</label><asp:DropDownList ID="CorrectList" runat="server" CssClass="form-select"><asp:ListItem>A</asp:ListItem><asp:ListItem>B</asp:ListItem><asp:ListItem>C</asp:ListItem></asp:DropDownList>
        <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary mt-3" Text="Save Question" OnClick="SaveButton_Click" ValidationGroup="QuestionGroup" />
    </section>

    <div class="table-panel p-3">
        <table class="table table-striped"><thead><tr><th>Quiz</th><th>Question</th><th>Correct</th><th>Actions</th></tr></thead><tbody>
        <asp:Repeater ID="QuestionsRepeater" runat="server" OnItemCommand="QuestionsRepeater_ItemCommand">
            <ItemTemplate>
                <tr><td><%# Eval("QuizTitle") %></td><td><%# Eval("QuestionText") %></td><td><%# Eval("CorrectAnswer") %></td><td><asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="Edit" CommandArgument='<%# Eval("QuestionID") %>'>Edit</asp:LinkButton> <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Delete" CommandArgument='<%# Eval("QuestionID") %>' OnClientClick="return confirm('Delete this question?');">Delete</asp:LinkButton></td></tr>
            </ItemTemplate>
        </asp:Repeater>
        </tbody></table>
    </div>
</asp:Content>
