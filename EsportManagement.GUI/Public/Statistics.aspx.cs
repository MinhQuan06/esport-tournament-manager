using System;
using System.Linq;
using System.Web.UI;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Public_Statistics : Page
{
    private int _maxGameCount = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireLogin();
        var tours    = new TournamentBLL().GetAll();
        var teams    = new TeamBLL().GetAll();
        var players  = new PlayerBLL().GetAll();

        litTotalTour.Text    = tours.Count.ToString();
        litTotalTeams.Text   = teams.Count.ToString();
        litTotalPlayers.Text = players.Count.ToString();
        litTotalMatches.Text = tours.Sum(t => t.MatchCount).ToString();

        litUpcoming.Text = tours.Count(t => t.Status == TournamentStatus.ChuaBatDau).ToString();
        litOngoing.Text  = tours.Count(t => t.Status == TournamentStatus.DangDienRa).ToString();
        litFinished.Text = tours.Count(t => t.Status == TournamentStatus.DaKetThuc).ToString();

        // Phân bố theo game
        var byGame = tours.GroupBy(t => string.IsNullOrEmpty(t.GameType) ? "Khác" : t.GameType)
                          .Select(g => new System.Collections.Generic.KeyValuePair<string, int>(g.Key, g.Count()))
                          .OrderByDescending(x => x.Value).ToList();
        if (byGame.Any()) _maxGameCount = byGame.Max(x => x.Value);
        rptGames.DataSource = byGame; rptGames.DataBind();

        rptTopTeams.DataSource = teams.OrderByDescending(t => t.Wins).Take(5).ToList();
        rptTopTeams.DataBind();

        // Match status
        try
        {
            int up = 0, live = 0, done = 0;
            foreach (var t in tours)
            {
                var ms = new MatchBLL().GetByTournament(t.TournamentID);
                up   += ms.Count(m => m.Status == MatchStatus.ChuaDienRa);
                live += ms.Count(m => m.Status == MatchStatus.DangDienRa);
                done += ms.Count(m => m.Status == MatchStatus.DaKetThuc);
            }
            litMatchUpcoming.Text = up.ToString();
            litMatchLive.Text     = live.ToString();
            litMatchDone.Text     = done.ToString();
        }
        catch { litMatchUpcoming.Text = litMatchLive.Text = litMatchDone.Text = "0"; }
    }

    public int GetPercent(object val)
    {
        if (_maxGameCount <= 0) return 0;
        return Convert.ToInt32(((int)val * 100.0) / _maxGameCount);
    }
}
