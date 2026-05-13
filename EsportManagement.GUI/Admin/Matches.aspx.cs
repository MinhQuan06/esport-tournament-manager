using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Matches : Page
{
    public bool ShowModal { get; private set; }
    private readonly MatchBLL _bll = new MatchBLL();
    private readonly TournamentBLL _tourBll = new TournamentBLL();
    private readonly TeamBLL _teamBll = new TeamBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        if (!IsPostBack)
        {
            var tournaments = _tourBll.GetAll();
            ddlTour.DataSource = tournaments;
            ddlTour.DataBind();
            var defaultTournament = tournaments.FirstOrDefault(t => t.Status == TournamentStatus.DangDienRa)
                ?? tournaments.FirstOrDefault();
            if (defaultTournament != null)
                ddlTour.SelectedValue = defaultTournament.TournamentID.ToString();
            BindTeamsForTour();
            BindList();
        }
    }

    protected void ddlTour_Changed(object sender, EventArgs e)
    {
        BindList(); BindTeamsForTour();
        ViewState["tab"] = "all";
    }

    protected void Tab_Click(object sender, EventArgs e)
    {
        ViewState["tab"] = ((LinkButton)sender).CommandArgument;
        BindList();
    }

    private void BindList()
    {
        int tid;
        var tab = ViewState["tab"] as string ?? "all";
        if (!int.TryParse(ddlTour.SelectedValue, out tid) || tid == 0)
        {
            rpt.DataSource = null; rpt.DataBind();
            litAll.Text = litUpcoming.Text = litOngoing.Text = litDone.Text = "0";
            pnlEmpty.Visible = true;
            return;
        }
        var all = _bll.GetByTournament(tid);
        litAll.Text      = all.Count.ToString();
        litUpcoming.Text = all.Count(m => m.Status == MatchStatus.ChuaDienRa).ToString();
        litOngoing.Text  = all.Count(m => m.Status == MatchStatus.DangDienRa).ToString();
        litDone.Text     = all.Count(m => m.Status == MatchStatus.DaKetThuc).ToString();

        SetTabActive(tab);

        var filtered = all;
        if (tab == "upcoming") filtered = all.Where(m => m.Status == MatchStatus.ChuaDienRa).ToList();
        else if (tab == "ongoing") filtered = all.Where(m => m.Status == MatchStatus.DangDienRa).ToList();
        else if (tab == "done")    filtered = all.Where(m => m.Status == MatchStatus.DaKetThuc).ToList();

        rpt.DataSource = filtered; rpt.DataBind();
        pnlEmpty.Visible = filtered.Count == 0;
    }

    private void SetTabActive(string tab)
    {
        tabAll.CssClass = tabUpcoming.CssClass = tabOngoing.CssClass = tabDone.CssClass = "match-tab";
        if (tab == "upcoming") tabUpcoming.CssClass += " active";
        else if (tab == "ongoing") tabOngoing.CssClass += " active";
        else if (tab == "done") tabDone.CssClass += " active";
        else tabAll.CssClass += " active";
    }

    private void BindTeamsForTour()
    {
        int tid;
        if (!int.TryParse(ddlTour.SelectedValue, out tid) || tid == 0) return;
        var teams = _teamBll.GetByTournament(tid);
        ddlTeam1.DataSource = teams; ddlTeam1.DataBind();
        ddlTeam2.DataSource = teams; ddlTeam2.DataBind();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int tid;
        if (!int.TryParse(ddlTour.SelectedValue, out tid) || tid == 0)
        { Notify("Vui lòng chọn giải đấu trước.", false); return; }
        BindTeamsForTour(); Reset(); ShowModal = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var m = new Match
            {
                TournamentID = int.Parse(ddlTour.SelectedValue),
                Team1ID      = int.Parse(ddlTeam1.SelectedValue),
                Team2ID      = int.Parse(ddlTeam2.SelectedValue),
                MatchTime    = DateTime.Parse(txtTime.Text),
                Status       = StatusHelper.MatchFromDb(ddlStatus.SelectedValue),
                RoundName    = ddlRound.SelectedValue,
                GroupName    = ddlGroup.SelectedValue,
                MatchFormat  = ddlFormat.SelectedValue
            };
            int id;
            if (int.TryParse(hfId.Value, out id) && id > 0)
            { m.MatchID = id; _bll.Update(m); Notify("Cập nhật thành công.", true); }
            else
            { _bll.Create(m); Notify("Tạo trận đấu thành công.", true); }
            Reset(); BindList();
        }
        catch (Exception ex) { BindTeamsForTour(); Notify(ex.Message, false); ShowModal = true; }
    }

    protected void rpt_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        int id = int.Parse((string)e.CommandArgument);
        if (e.CommandName == "EditItem")
        {
            var m = _bll.GetByID(id); if (m == null) return;
            BindTeamsForTour();
            hfId.Value = m.MatchID.ToString();
            try { ddlTeam1.SelectedValue = m.Team1ID.ToString(); } catch { }
            try { ddlTeam2.SelectedValue = m.Team2ID.ToString(); } catch { }
            txtTime.Text = m.MatchTime.ToString("yyyy-MM-ddTHH:mm");
            try { ddlStatus.SelectedValue = StatusHelper.ToDb(m.Status); } catch { }
            try { ddlRound.SelectedValue = m.RoundName ?? "Vòng bảng"; } catch { }
            try { ddlGroup.SelectedValue = m.GroupName ?? ""; } catch { }
            try { ddlFormat.SelectedValue = m.MatchFormat ?? "BO3"; } catch { }
            litTitle.Text = "Sửa trận đấu";
            ShowModal = true;
        }
        else if (e.CommandName == "DeleteItem")
        {
            try { _bll.Delete(id); Notify("Đã xóa.", true); BindList(); }
            catch (Exception ex) { Notify(ex.Message, false); }
        }
    }

    private void Reset()
    {
        hfId.Value = ""; txtTime.Text = "";
        ddlStatus.SelectedIndex = 0; ddlRound.SelectedIndex = 0;
        ddlGroup.SelectedIndex = 0; ddlFormat.SelectedIndex = 1;
        litTitle.Text = "Thêm trận đấu mới";
    }

    public string GetCardClass(object item)
    {
        var m = (Match)item;
        if (m.IsConfirmed == true) return "confirmed";
        if (m.Status == MatchStatus.DangDienRa) return "live";
        if (m.Status == MatchStatus.ChuaDienRa) return "upcoming";
        return "done";
    }
    public string GetStatusClass(object item)
    {
        var m = (Match)item;
        if (m.IsConfirmed == true) return "badge-active";
        if (m.Status == MatchStatus.DangDienRa) return "badge-live";
        if (m.Status == MatchStatus.ChuaDienRa) return "badge-upcoming";
        return "badge-finished";
    }
    public string GetStatusText(object item)
    {
        var m = (Match)item;
        if (m.IsConfirmed == true) return "✓ XÁC NHẬN";
        if (m.Status == MatchStatus.DangDienRa) return "LIVE";
        if (m.Status == MatchStatus.ChuaDienRa) return "SẮP DIỄN RA";
        if (m.ScoreTeam1 == null) return "⚠ CHỜ KQ";
        return "ĐÃ KẾT THÚC";
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
