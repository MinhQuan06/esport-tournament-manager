using System;
using System.Linq;
using System.Web.UI;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Manager_JoinTournament : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireManager();
        if (!IsPostBack)
        {
            var tours = new TournamentBLL().GetAll()
                .Where(t => t.Status == TournamentStatus.ChuaBatDau).ToList();
            ddlTour.DataSource = tours; ddlTour.DataBind();
        }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        try
        {
            var acc = AuthHelper.Current;
            int tid;
            if (!int.TryParse(ddlTour.SelectedValue, out tid))
                throw new Exception("Vui lòng chọn giải đấu.");
            string[] colors = { "#3b82f6","#ef4444","#10b981","#f59e0b","#8b5cf6","#06b6d4","#ec4899" };
            int hash = 0; foreach (var c in txtName.Text) hash = (hash*31+c) & 0x7fffffff;
            new TeamBLL().Create(new Team {
                TournamentID = tid,
                TeamName     = txtName.Text.Trim(),
                ShortName    = txtShort.Text.Trim(),
                Description  = txtDesc.Text.Trim(),
                ManagerAccountID = acc.AccountID,
                GameType     = ddlGame.SelectedValue,
                LogoColor    = colors[hash % colors.Length],
                IsActive     = true
            });
            Notify("Đăng ký thành công! Bạn có thể quản lý đội tại 'Đội của tôi'.", true);
            txtName.Text = txtShort.Text = txtDesc.Text = "";
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
