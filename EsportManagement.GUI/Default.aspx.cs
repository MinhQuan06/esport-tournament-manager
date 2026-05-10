using System;
using System.Linq;
using System.Web.UI;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class DefaultPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireLogin();
        if (Request.QueryString["err"] == "forbidden") pnlError.Visible = true;
        litToday.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy",
            new System.Globalization.CultureInfo("vi-VN"));

        if (!IsPostBack) LoadDashboard();
    }

    private void LoadDashboard()
    {
        var tours = new TournamentBLL().GetAll();
        litTour.Text   = tours.Count.ToString();
        litTourNew.Text = tours.Count(t => t.CreatedAt.Month == DateTime.Now.Month
                                         && t.CreatedAt.Year == DateTime.Now.Year).ToString();
        litTeams.Text   = tours.Sum(t => t.TeamCount).ToString();
        litMatches.Text = tours.Sum(t => t.MatchCount).ToString();
        litOngoing.Text = tours.Count(t => t.Status == TournamentStatus.DangDienRa).ToString();

        // Tổng player
        try { litPlayers.Text = new PlayerBLL().GetAll().Count.ToString(); }
        catch { litPlayers.Text = "0"; }

        rptTours.DataSource = tours.Take(5).ToList();
        rptTours.DataBind();

        var matches = new MatchBLL();
        try
        {
            var upcoming = new EsportManagement.DAL.MatchDAL().GetUpcoming(7).Take(5).ToList();
            rptMatches.DataSource = upcoming;
            rptMatches.DataBind();
        }
        catch { }
    }

    public string GetBadgeClass(TournamentStatus s)
    {
        if (s == TournamentStatus.DangDienRa) return "badge-active";
        if (s == TournamentStatus.ChuaBatDau) return "badge-upcoming";
        return "badge-finished";
    }

    public string GetGameIcon(string gameType)
    {
        if (gameType == null) return "bi bi-controller";
        var g = gameType.ToLower();
        if (g.Contains("legend") || g.Contains("lmht")) return "bi bi-shield-fill";
        if (g.Contains("cs"))      return "bi bi-bullseye";
        if (g.Contains("valorant")) return "bi bi-x-diamond-fill";
        if (g.Contains("dota"))    return "bi bi-lightning-fill";
        return "bi bi-controller";
    }

    public string GetGameColor(string gameType)
    {
        if (gameType == null) return "#3b82f6";
        var g = gameType.ToLower();
        if (g.Contains("legend"))   return "#3b82f6";
        if (g.Contains("cs"))       return "#f59e0b";
        if (g.Contains("valorant")) return "#ef4444";
        if (g.Contains("dota"))     return "#8b5cf6";
        return "#06b6d4";
    }
}
