<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    private QuizInfo CurrentQuiz;

    protected void Page_Load(object sender, EventArgs e)
    {
        RequireMember();
        int id;
        if (!Int32.TryParse(Request.QueryString["id"], out id)) Response.Redirect("QuizCentre.aspx");
        CurrentQuiz = SkillBridgeDb.GetQuiz(id);
        if (CurrentQuiz == null) Response.Redirect("QuizCentre.aspx");

        if (!IsPostBack)
        {
            QuizTitleLabel.Text = CurrentQuiz.QuizTitle;
            QuestionsRepeater.DataSource = SkillBridgeDb.GetQuestions(CurrentQuiz.QuizID);
            QuestionsRepeater.DataBind();
        }
    }

    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        int score = 0;
        bool allAnswered = true;

        foreach (System.Web.UI.WebControls.RepeaterItem item in QuestionsRepeater.Items)
        {
            System.Web.UI.WebControls.HiddenField correct = (System.Web.UI.WebControls.HiddenField)item.FindControl("CorrectAnswerHidden");
            System.Web.UI.WebControls.RadioButtonList answers = (System.Web.UI.WebControls.RadioButtonList)item.FindControl("AnswersList");
            if (answers.SelectedValue == "")
            {
                allAnswered = false;
            }
            else if (answers.SelectedValue == correct.Value)
            {
                score++;
            }
        }

        if (!allAnswered)
        {
            MessageLabel.Text = "Please answer all questions.";
            return;
        }

        SkillBridgeDb.SaveQuizResult(Convert.ToInt32(Session["UserID"]), CurrentQuiz.QuizID, score);
        MessageLabel.CssClass = "success-message d-block mt-3";
        MessageLabel.Text = "Quiz submitted. Your score is " + score + " out of " + CurrentQuiz.TotalMarks + ".";
        SubmitButton.Enabled = false;
    }

    private void RequireMember()
    {
        if (Session["UserID"] == null) Response.Redirect("~/Account/Login.aspx");
        if (Convert.ToString(Session["Role"]) != "Member") Response.Redirect("~/Admin/Dashboard.aspx");
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Attempt Quiz - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="form-panel">
        <p class="eyebrow">Quiz</p>
        <h1><asp:Label ID="QuizTitleLabel" runat="server" /></h1>

        <asp:Repeater ID="QuestionsRepeater" runat="server">
            <ItemTemplate>
                <div class="section-card p-3 mb-3">
                    <asp:HiddenField ID="CorrectAnswerHidden" runat="server" Value='<%# Eval("CorrectAnswer") %>' />
                    <h2 class="h5"><%# Eval("QuestionText") %></h2>
                    <asp:RadioButtonList ID="AnswersList" runat="server">
                        <asp:ListItem Value="A" Text='<%# Eval("OptionA") %>' />
                        <asp:ListItem Value="B" Text='<%# Eval("OptionB") %>' />
                        <asp:ListItem Value="C" Text='<%# Eval("OptionC") %>' />
                    </asp:RadioButtonList>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Button ID="SubmitButton" runat="server" CssClass="btn btn-primary" Text="Submit Quiz" OnClick="SubmitButton_Click" />
        <asp:Label ID="MessageLabel" runat="server" CssClass="error-message d-block mt-3" />
    </section>
</asp:Content>
