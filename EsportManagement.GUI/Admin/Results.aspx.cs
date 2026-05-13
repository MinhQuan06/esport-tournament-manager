using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Results : Page
{
    private readonly TournamentBLL _tourBll = new TournamentBLL();
    private readonly MatchBLL _matchBll = new MatchBLL();
    private readonly MatchResultBLL _resultBll = new MatchResultBLL();

    public string Team1Short { get; private set; }
    public string Team2Short { get; private set; }
    public string Team1Color { get; private set; }
    public string Team2Color { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        Team1Short = Team2Short = ""; Team1Color = "#3b82f6"; Team2Color = "#ef4444";
        if (!IsPostBack)
        {
            var tournaments = _tourBll.GetAll();
            ddlTour.DataSource = tournaments;
            ddlTour.DataBind();
            ddlTour.Items.Insert(0, new ListItem("-- Ch?n gi?i --", "0"));
            int matchId;
            if (int.TryParse(Request.QueryString["match"], out matchId))
            {
                var m = _matchBll.GetByID(matchId);
                if (m != null)
                {
                    ddlTour.SelectedValue = m.TournamentID.ToString();
                    LoadMatches();
                    ddlMatch.SelectedValue = matchId.ToString();
                }
            }
            else
            {
                var defaultTournament = tournaments.FirstOrDefault(t => t.Status == TournamentStatus.DangDienRa)
                    ?? tournaments.FirstOrDefault();
                if (defaultTournament != null)
                {
                    ddlTour.SelectedValue = defaultTournament.TournamentID.ToString();
                    LoadMatches();
                }
            }
        }
        LoadCurrentMatch();
    }

    protected void ddlTour_Changed(object sender, EventArgs e) { LoadMatches(); pnlEntry.Visible = false; }
    protected void ddlMatch_Changed(object sender, EventArgs e) { LoadCurrentMatch(); }

    private void LoadMatches()
    {
        int tid;
        if (!int.TryParse(ddlTour.SelectedValue, out tid) || tid == 0)
        { ddlMatch.DataSource = null; ddlMatch.DataBind(); return; }
        var matches = _matchBll.GetByTournament(tid)
            .Select(m => new {
                MatchID = m.MatchID,
                DisplayName = string.Format("#{0} {1} vs {2} ({3:dd/MM HH:mm})",
                    m.MatchID, m.Team1Name, m.Team2Name, m.MatchTime)
            }).ToList();
        ddlMatch.DataSource = matches; ddlMatch.DataBind();
        ddlMatch.Items.Insert(0, new ListItem("-- Chọn trận --", "0"));
    }

    private void LoadCurrentMatch()
    {
        int mid;
        if (!int.TryParse(ddlMatch.SelectedValue, out mid) || mid == 0)
        { pnlEntry.Visible = false; return; }
        var m = _matchBll.GetByID(mid);
        if (m == null) { pnlEntry.Visible = false; return; }
        pnlEntry.Visible = true;
        hfMatchId.Value = mid.ToString();
        litTeam1.Text = litTeam1B.Text = Server.HtmlEncode((m.Team1Name ?? "").ToUpper());
        litTeam2.Text = litTeam2B.Text = Server.HtmlEncode((m.Team2Name ?? "").ToUpper());
        Team1Short = m.Team1Short ?? ""; Team2Short = m.Team2Short ?? "";
        Team1Color = m.Team1Color ?? "#3b82f6"; Team2Color = m.Team2Color ?? "#ef4444";
        litRound.Text = string.Format("{0} · {1}", m.RoundName ?? "", m.MatchFormat ?? "BO3");

        if (m.ScoreTeam1.HasValue) txtScore1.Text = m.ScoreTeam1.Value.ToString();
        if (m.ScoreTeam2.HasValue) txtScore2.Text = m.ScoreTeam2.Value.ToString();

        if (m.WinnerTeamID.HasValue)
        {
            pnlWinner.Visible = true;
            litWinner.Text = m.WinnerName + " thắng (" + m.ScoreTeam1 + " - " + m.ScoreTeam2 + ")";
        }
        else if (m.ScoreTeam1.HasValue && m.ScoreTeam1 == m.ScoreTeam2)
        {
            pnlWinner.Visible = true;
            litWinner.Text = "Hòa " + m.ScoreTeam1 + " - " + m.ScoreTeam2;
        }

        bool confirmed = m.IsConfirmed == true;
        pnlConfirmed.Visible = confirmed;
        txtScore1.Enabled = txtScore2.Enabled = !confirmed;
        btnEnter.Enabled = !confirmed;
        btnConfirm.Enabled = !confirmed && m.ScoreTeam1.HasValue;
    }

    protected void btnEnter_Click(object sender, EventArgs e)
    {
        try
        {
            int mid = int.Parse(hfMatchId.Value);
            int s1 = int.Parse(txtScore1.Text); int s2 = int.Parse(txtScore2.Text);
            _resultBll.EnterResult(mid, s1, s2);
            Notify("Đã lưu kết quả. BXH cập nhật tự động.", true);
            LoadCurrentMatch();
        }
        catch (Exception ex) { Notify(ex.Message, false); }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        try
        {
            int mid = int.Parse(hfMatchId.Value);
            _resultBll.ConfirmResult(mid);
            Notify("Đã xác nhận kết quả chính thức.", true);
            LoadCurrentMatch();
        }
        catch (Exception ex) { Notify(ex.Message, false); }
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
