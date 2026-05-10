using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Tournaments : Page
{
    public bool ShowModal { get; private set; }
    private readonly TournamentBLL _bll = new TournamentBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        if (!IsPostBack) Bind();
    }

    private void Bind()
    {
        gv.DataSource = _bll.Search(txtKw.Text.Trim(), ddlStatus.SelectedValue, ddlGame.SelectedValue);
        gv.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e) { Bind(); }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var t = new Tournament
            {
                TournamentName = txtName.Text.Trim(),
                StartDate      = DateTime.Parse(txtStart.Text),
                EndDate        = DateTime.Parse(txtEnd.Text),
                Description    = txtDesc.Text.Trim(),
                Status         = StatusHelper.TournamentFromDb(ddlStatusEdit.SelectedValue),
                GameType       = ddlGameEdit.SelectedValue,
                Format         = ddlFormat.SelectedValue
            };
            int id;
            if (int.TryParse(hfId.Value, out id) && id > 0)
            { t.TournamentID = id; _bll.Update(t); Notify("Cập nhật thành công.", true); }
            else
            { _bll.Create(t); Notify("Tạo giải đấu thành công.", true); }
            ResetForm(); Bind();
        }
        catch (Exception ex) { Notify(ex.Message, false); ShowModal = true; }
    }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = int.Parse((string)e.CommandArgument);
        if (e.CommandName == "EditItem")
        {
            var t = _bll.GetByID(id); if (t == null) return;
            hfId.Value = t.TournamentID.ToString();
            txtName.Text = t.TournamentName;
            txtStart.Text = t.StartDate.ToString("yyyy-MM-dd");
            txtEnd.Text = t.EndDate.ToString("yyyy-MM-dd");
            txtDesc.Text = t.Description;
            try { ddlStatusEdit.SelectedValue = StatusHelper.ToDb(t.Status); } catch { }
            try { ddlGameEdit.SelectedValue = t.GameType ?? "Khác"; } catch { }
            try { ddlFormat.SelectedValue = t.Format ?? "Round Robin"; } catch { }
            litMdlTitle.Text = "Sửa giải đấu";
            ShowModal = true;
        }
        else if (e.CommandName == "DeleteItem")
        {
            try { _bll.Delete(id); Notify("Đã xóa.", true); Bind(); }
            catch (Exception ex) { Notify(ex.Message, false); }
        }
    }

    private void ResetForm()
    {
        hfId.Value = ""; txtName.Text = ""; txtDesc.Text = "";
        txtStart.Text = ""; txtEnd.Text = "";
        ddlStatusEdit.SelectedIndex = 0; ddlGameEdit.SelectedIndex = 0; ddlFormat.SelectedIndex = 0;
        litMdlTitle.Text = "Tạo giải đấu mới";
    }

    public string GetBadgeClass(TournamentStatus s)
    {
        if (s == TournamentStatus.DangDienRa) return "badge-active";
        if (s == TournamentStatus.ChuaBatDau) return "badge-upcoming";
        return "badge-finished";
    }
    public string GetBadgeText(TournamentStatus s)
    {
        if (s == TournamentStatus.DangDienRa) return "ĐANG DIỄN RA";
        if (s == TournamentStatus.ChuaBatDau) return "SẮP DIỄN RA";
        return "KẾT THÚC";
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
