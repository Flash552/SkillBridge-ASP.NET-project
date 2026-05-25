<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">SkillBridge - Home</asp:Content>

<asp:Content ID="Head" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .home-stat { border-left: 5px solid #2754d8; }
        .home-stat strong { display: block; font-size: 2rem; }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero mb-4">
        <p class="eyebrow">Digital Learning Platform</p>
        <h1 class="display-5 fw-bold">Learn by skill, grow by progress.</h1>
        <p class="lead col-lg-8">SkillBridge helps guests and members find digital learning resources by skill area, attempt quizzes, and track learning progress.</p>
        <a class="btn btn-primary me-2" href="Resources/Index.aspx">Browse Resources</a>
        <a class="btn btn-outline-primary" href="Account/Register.aspx">Join as Member</a>
    </section>

    <section class="row g-4 mb-4" aria-label="SkillBridge features">
        <article class="col-md-4">
            <div class="section-card home-stat p-4 h-100">
                <strong>01</strong>
                <h2 class="h5">Browse Skills</h2>
                <p>Explore learning topics without creating an account.</p>
            </div>
        </article>
        <article class="col-md-4">
            <div class="section-card home-stat p-4 h-100">
                <strong>02</strong>
                <h2 class="h5">Attempt Quizzes</h2>
                <p>Registered members can complete quizzes and save scores.</p>
            </div>
        </article>
        <article class="col-md-4">
            <div class="section-card home-stat p-4 h-100" style="border-left-color:#16a085;">
                <strong>03</strong>
                <h2 class="h5">Track Progress</h2>
                <p>Mark resources as in progress or completed.</p>
            </div>
        </article>
    </section>

    <section class="section-card p-4">
        <div class="row align-items-center g-4">
            <div class="col-lg-6">
                <p class="eyebrow">Multimedia Example</p>
                <h2>Learning dashboard preview</h2>
                <p>This page demonstrates HTML5 structure, external CSS, internal CSS, inline CSS, and multimedia content required by the assignment.</p>
            </div>
            <div class="col-lg-6">
                <img class="learning-image" src="Images/learning-dashboard.svg" alt="SkillBridge learning dashboard illustration" />
            </div>
        </div>
    </section>
</asp:Content>
