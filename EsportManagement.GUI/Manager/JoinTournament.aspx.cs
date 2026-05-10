using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Manager_JoinTournament : Page
{
    private readonly TournamentBLL _tourBll = new TournamentBLL();
    private readonly TeamBLL _teamBll = new TeamBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireManager();
        if (!IsPostBack)
        {
            BindTournaments();
            pnlNoTour.Visible = ddlTour.SelectedValue == "0";
            pnlTeamPicker.Visible = !pnlNoTour.Visible;
            if (pnlTeamPicker.Visible) BindMyTeams();
        }
    }

    private void BindTournaments()
    {
        var tours = _tourBll.GetAll()
            .Where(t => t.Status == TournamentStatus.ChuaBatDau)
            .Select(t => new {
                TournamentID = t.TournamentID,
                DisplayName = t.TournamentName + " (" + t.GameType + " · "
                              + t.StartDate.ToString("dd/MM/yyyy") + ")",
                GameType = t.GameType
            }).ToList();
        ddlTour.DataSource = tours; ddlTour.DataBind();
        ddlTour.Items.Insert(0, new ListItem("-- Chọn giải đấu --", "0"));
    }

    protected void ddlTour_Changed(object sender, EventArgs e)
    {
        if (ddlTour.SelectedValue == "0")
        {
            pnlNoTour.Visible = true;
            pnlTeamPicker.Visible = false;
            return;
        }
        pnlNoTour.Visible = false;
        pnlTeamPicker.Visible = true;
        BindMyTeams();
        ResetForm();
    }

    private void BindMyTeams()
    {
        int tid = int.Parse(ddlTour.SelectedValue);
        var tour = _tourBll.GetByID(tid);
        if (tour == null) return;
        litGame.Text = tour.GameType ?? "—";

        var acc = AuthHelper.Current;
        var myTeams = _teamBll.GetMyTeamsByGame(acc.AccountID, tour.GameType)
            .Select(t => new {
                TeamID = t.TeamID,
                DisplayName = t.TeamName + " (" + (string.IsNullOrEmpty(t.ShortName) ? "—" : t.ShortName)
                              + " · " + t.GameType + ")"
            }).ToList();
        ddlExisting.Items.Clear();
        ddlExisting.Items.Add(new ListItem("-- Tạo đội mới --", ""));
        foreach (var t in myTeams)
            ddlExisting.Items.Add(new ListItem(t.DisplayName, t.TeamID.ToString()));
    }

    protected void ddlExisting_Changed(object sender, EventArgs e)
    {
        int tid;
        if (!int.TryParse(ddlExisting.SelectedValue, out tid) || tid == 0)
        {
            ResetForm();
            return;
        }
        var src = _teamBll.GetByID(tid);
        if (src == null) { ResetForm(); return; }
        // Auto-fill form từ đội đã có
        txtName.Text = src.TeamName;
        txtShort.Text = src.ShortName;
        txtManager.Text = src.ManagerName;
        txtDesc.Text = src.Description;
        try { ddlColor.SelectedValue = src.LogoColor ?? "#3b82f6"; } catch { }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        try
        {
            var acc = AuthHelper.Current;
            int tid = int.Parse(ddlTour.SelectedValue);
            var tour = _tourBll.GetByID(tid);
            if (tour == null) throw new Exception("Giải đấu không tồn tại.");

            new TeamBLL().Create(new Team {
                TournamentID = tid,
                TeamName     = txtName.Text.Trim(),
                ShortName    = txtShort.Text.Trim(),
                Description  = txtDesc.Text.Trim(),
                ManagerAccountID = acc.AccountID,
                ManagerName  = string.IsNullOrEmpty(txtManager.Text.Trim()) ? acc.FullName : txtManager.Text.Trim(),
                GameType     = tour.GameType,    // Lấy theo game của giải
                LogoColor    = ddlColor.SelectedValue,
                IsActive     = true
            });
            Notify("Đăng ký thành công vào giải '" + tour.TournamentName + "'! Vào 'Đội của tôi' để quản lý.", true);
            ResetForm();
        }
        catch (Exception ex) { Notify(ex.Message, false); }
    }

    private void ResetForm()
    {
        txtName.Text = txtShort.Text = txtManager.Text = txtDesc.Text = "";
        ddlColor.SelectedIndex = 0;
        ddlExisting.SelectedValue = "";
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
