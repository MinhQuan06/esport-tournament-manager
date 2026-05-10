using System;
using System.Web.UI;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Public_Tournaments : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireLogin();
        if (!IsPostBack) Bind();
    }
    private void Bind()
    {
        var list = new TournamentBLL().Search(txtKw.Text.Trim(), ddlStatus.SelectedValue, "");
        rpt.DataSource = list; rpt.DataBind();
    }
    protected void btnSearch_Click(object sender, EventArgs e) { Bind(); }

    public string GetBadge(object status)
    {
        var s = (TournamentStatus)status;
        if (s == TournamentStatus.DangDienRa) return "badge-active";
        if (s == TournamentStatus.ChuaBatDau) return "badge-upcoming";
        return "badge-finished";
    }
    public string GetBadgeText(object status)
    {
        var s = (TournamentStatus)status;
        if (s == TournamentStatus.DangDienRa) return "ĐANG DIỄN RA";
        if (s == TournamentStatus.ChuaBatDau) return "SẮP DIỄN RA";
        return "KẾT THÚC";
    }
}
