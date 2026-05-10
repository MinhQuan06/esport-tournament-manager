using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.DTO;

public partial class SiteMaster : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var acc = Session["CurrentUser"] as Account;
        if (acc != null)
        {
            phSidebar.Visible = true;
            bodyTag.Attributes["class"] = "has-sidebar";
            litInitials.Text = Server.HtmlEncode(acc.Initials);
            litUserName.Text = Server.HtmlEncode(acc.FullName);
            litUserRole.Text = Server.HtmlEncode(acc.PrimaryRole.ToUpper());
            phAdminMenu.Visible = acc.HasRole("Admin");

            lnkDashboard.NavigateUrl   = ResolveUrl("~/Default.aspx");
            lnkTournaments.NavigateUrl = ResolveUrl(acc.HasRole("Admin")
                ? "~/Admin/Tournaments.aspx" : "~/Public/Tournaments.aspx");
            lnkTeams.NavigateUrl = ResolveUrl(acc.HasRole("Admin")
                ? "~/Admin/Teams.aspx"
                : (acc.HasRole("TeamManager") ? "~/Manager/MyTeams.aspx" : "~/Default.aspx"));
            lnkSchedule.NavigateUrl    = ResolveUrl("~/Public/Schedule.aspx");
            lnkRanking.NavigateUrl     = ResolveUrl("~/Public/Ranking.aspx");
            lnkStatistics.NavigateUrl  = ResolveUrl("~/Public/Statistics.aspx");

            if (acc.HasRole("Admin"))
            {
                lnkPlayers.Visible = true;
                lnkPlayers.NavigateUrl  = ResolveUrl("~/Admin/Players.aspx");
                lnkMatches.Visible = true;
                lnkMatches.NavigateUrl  = ResolveUrl("~/Admin/Matches.aspx");
                lnkAccounts.NavigateUrl = ResolveUrl("~/Admin/Accounts.aspx");
                lnkRoles.NavigateUrl    = ResolveUrl("~/Admin/Roles.aspx");
            }

            // Cho cả Admin và TeamManager thấy "Đăng ký giải đấu"
            if (acc.HasRole("Admin") || acc.HasRole("TeamManager"))
            {
                lnkJoinTournament.Visible = true;
                lnkJoinTournament.NavigateUrl = ResolveUrl("~/Manager/JoinTournament.aspx");
            }

            string url = Request.Url.AbsolutePath.ToLower();
            HighlightIfMatch(lnkDashboard,       url, "default.aspx");
            HighlightIfMatch(lnkTournaments,     url, "tournaments");
            HighlightIfMatch(lnkTeams,           url, "teams", "myteams");
            HighlightIfMatch(lnkPlayers,         url, "players", "myplayers");
            HighlightIfMatch(lnkSchedule,        url, "schedule");
            HighlightIfMatch(lnkMatches,         url, "matches", "results");
            HighlightIfMatch(lnkJoinTournament,  url, "jointournament");
            HighlightIfMatch(lnkRanking,         url, "ranking");
            HighlightIfMatch(lnkStatistics,      url, "statistics");
            HighlightIfMatch(lnkAccounts,        url, "accounts");
            HighlightIfMatch(lnkRoles,           url, "roles");
        }
        else
        {
            bodyTag.Attributes["class"] = "no-sidebar";
        }
    }

    private void HighlightIfMatch(HyperLink link, string url, params string[] keywords)
    {
        if (link == null) return;
        foreach (var kw in keywords)
        {
            if (url.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                link.CssClass = "menu-item active";
                return;
            }
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Login.aspx");
    }
}
