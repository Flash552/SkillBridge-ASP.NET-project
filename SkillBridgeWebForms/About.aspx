<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">About - SkillBridge</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-heading mb-4">
        <p class="eyebrow">About</p>
        <h1>About SkillBridge</h1>
        <p class="lead">SkillBridge is a web-based learning system built with ASP.NET Web Forms, SQL Server, ADO.NET, HTML5, CSS, and server-side validation.</p>
    </section>

    <section class="row g-4">
        <article class="col-md-6">
            <div class="section-card p-4 h-100">
                <h2 class="h4">Objectives</h2>
                <p>Help users discover digital resources by skill, support member learning activities, and provide admin tools for managing content.</p>
            </div>
        </article>
        <article class="col-md-6">
            <div class="section-card p-4 h-100">
                <h2 class="h4">Modules</h2>
                <ul>
                    <li>Guest browsing for skills and resources.</li>
                    <li>Member quiz, progress, history, and profile features.</li>
                    <li>Administrator CRUD management for website content.</li>
                </ul>
            </div>
        </article>
    </section>
</asp:Content>
