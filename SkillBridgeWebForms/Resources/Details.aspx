<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    private LearningResource CurrentResource;

    protected void Page_Load(object sender, EventArgs e)
    {
        int id;
        if (!Int32.TryParse(Request.QueryString["id"], out id))
        {
            Response.Redirect("Index.aspx");
            return;
        }

        CurrentResource = SkillBridgeDb.GetResource(id);
        if (CurrentResource == null)
        {
            Response.Redirect("Index.aspx");
            return;
        }

        if (!IsPostBack)
        {
            BindResource();
        }
    }

    protected void SaveProgress_Click(object sender, EventArgs e)
    {
        if (Session["UserID"] == null)
        {
            Response.Redirect("~/Account/Login.aspx");
            return;
        }

        string status = ((System.Web.UI.WebControls.Button)sender).CommandArgument;
        SkillBridgeDb.SaveProgress(Convert.ToInt32(Session["UserID"]), CurrentResource.ResourceID, status);
        MessageLabel.Text = "Progress saved.";
        BindResource();
    }

    private void BindResource()
    {
        TitleLabel.Text = CurrentResource.Title;
        SkillLabel.Text = CurrentResource.SkillName;
        TypeLabel.Text = CurrentResource.Type;
        DescriptionLabel.Text = CurrentResource.Description;
        OpenLink.NavigateUrl = CurrentResource.Url;

        if (CurrentResource.Type == "Video" && CurrentResource.Url.Contains("youtube.com/embed"))
        {
            MediaLiteral.Text = "<iframe class='media-frame' src='" + Server.HtmlEncode(CurrentResource.Url) + "' title='Learning video' allowfullscreen></iframe>";
        }
        else if (CurrentResource.Url.EndsWith(".svg") || CurrentResource.Url.EndsWith(".png") || CurrentResource.Url.EndsWith(".jpg"))
        {
            MediaLiteral.Text = "<img class='learning-image' src='" + ResolveUrl("~/" + CurrentResource.Url) + "' alt='Learning resource image' />";
        }
        else
        {
            MediaLiteral.Text = "<img class='learning-image' src='" + ResolveUrl("~/Images/learning-dashboard.svg") + "' alt='Learning dashboard illustration' />";
        }
    }
</script>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Resource Details - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <article class="section-card p-4">
        <span class="skill-badge"><asp:Label ID="SkillLabel" runat="server" /></span>
        <span class="type-pill type-video ms-2"><asp:Label ID="TypeLabel" runat="server" /></span>
        <h1 class="mt-3"><asp:Label ID="TitleLabel" runat="server" /></h1>
        <p class="lead"><asp:Label ID="DescriptionLabel" runat="server" /></p>

        <div class="my-4">
            <asp:Literal ID="MediaLiteral" runat="server" />
        </div>

        <asp:HyperLink ID="OpenLink" runat="server" CssClass="btn btn-primary me-2" Target="_blank">Open Resource</asp:HyperLink>
        <asp:Button ID="InProgressButton" runat="server" CssClass="btn btn-outline-primary me-2" Text="Mark In Progress" CommandArgument="In Progress" OnClick="SaveProgress_Click" />
        <asp:Button ID="CompletedButton" runat="server" CssClass="btn btn-outline-success" Text="Mark Completed" CommandArgument="Completed" OnClick="SaveProgress_Click" />
        <asp:Label ID="MessageLabel" runat="server" CssClass="success-message d-block mt-3" />
    </article>
</asp:Content>
