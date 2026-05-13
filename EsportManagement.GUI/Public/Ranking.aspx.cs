using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Public_Ranking : Page
{
    private List<Team> _teamsCache;

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireLogin();
        if (!IsPostBack)
        {
            var tours = new TournamentBLL().GetAll();
            ddlTour.DataSource = tours; ddlTour.DataBind();
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
                try { ddlTour.SelectedValue = id.ToString(); } catch { }
            else
            {
                var defaultTournament = tours.FirstOrDefault(t => t.Status == TournamentStatus.DangDienRa)
                    ?? tours.FirstOrDefault();
                if (defaultTournament != null)
                    ddlTour.SelectedValue = defaultTournament.TournamentID.ToString();
            }
        }
        Bind();
    }

    protected void ddlTour_Changed(object sender, EventArgs e) { Bind(); }

    private void Bind()
    {
        int id;
        if (!int.TryParse(ddlTour.SelectedValue, out id) || id == 0)
        {
            gv.DataSource = null; gv.DataBind();
            rptTop3.DataSource = null; rptTop3.DataBind();
            rptRecent.DataSource = null; rptRecent.DataBind();
            return;
        }
        _teamsCache = new TeamBLL().GetByTournament(id);
        var ranking = new RankingBLL().GetByTournament(id);
        gv.DataSource = ranking; gv.DataBind();
        rptTop3.DataSource = ranking.Take(3).ToList();
        rptTop3.DataBind();

        var recent = new MatchBLL().GetByTournament(id)
            .Where(m => m.ScoreTeam1.HasValue)
            .OrderByDescending(m => m.MatchTime)
            .Take(4).ToList();
        rptRecent.DataSource = recent; rptRecent.DataBind();
    }

    protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int rank = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Rank"));
            e.Row.CssClass = "ranking-row rank-" + rank;
        }
    }

    public string GetTeamColor(object teamId)
    {
        if (_teamsCache == null) return "#3b82f6";
        var t = _teamsCache.FirstOrDefault(x => x.TeamID == (int)teamId);
        return t != null ? t.LogoColor : "#3b82f6";
    }
    public string GetTeamShort(object teamId)
    {
        if (_teamsCache == null) return "";
        var t = _teamsCache.FirstOrDefault(x => x.TeamID == (int)teamId);
        return t != null && !string.IsNullOrEmpty(t.ShortName) ? t.ShortName : "";
    }
}
