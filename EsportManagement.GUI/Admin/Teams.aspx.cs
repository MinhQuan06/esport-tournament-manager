using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Teams : Page
{
    public bool ShowModal { get; private set; }
    private readonly TeamBLL _bll = new TeamBLL();
    private readonly TournamentBLL _tourBll = new TournamentBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        if (!IsPostBack)
        {
            BindFilters();
            BindList();
        }
    }

    private void BindFilters()
    {
        var tours = _tourBll.GetAll();
        ddlTour.DataSource = tours; ddlTour.DataBind();
        ddlTourEdit.DataSource = tours; ddlTourEdit.DataBind();
    }

    private void BindList()
    {
        int tid;
        var list = (int.TryParse(ddlTour.SelectedValue, out tid) && tid > 0)
            ? _bll.GetByTournament(tid) : _bll.GetAll();
        rpt.DataSource = list; rpt.DataBind();
    }

    protected void ddlTour_Changed(object sender, EventArgs e) { BindList(); }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var t = new Team
            {
                TournamentID = int.Parse(ddlTourEdit.SelectedValue),
                TeamName     = txtName.Text.Trim(),
                ShortName    = txtShort.Text.Trim(),
                Description  = txtDesc.Text.Trim(),
                ManagerName  = txtManager.Text.Trim(),
                GameType     = ddlGameEdit.SelectedValue,
                LogoColor    = ddlColor.SelectedValue,
                IsActive     = true
            };
            int id;
            if (int.TryParse(hfId.Value, out id) && id > 0)
            { t.TeamID = id; _bll.Update(t); Notify("Cập nhật thành công.", true); }
            else
            { _bll.Create(t); Notify("Tạo đội thành công.", true); }
            Reset(); BindList();
        }
        catch (Exception ex) { Notify(ex.Message, false); ShowModal = true; }
    }

    protected void rpt_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        int id = int.Parse((string)e.CommandArgument);
        if (e.CommandName == "EditItem")
        {
            var t = _bll.GetByID(id); if (t == null) return;
            hfId.Value = t.TeamID.ToString();
            try { ddlTourEdit.SelectedValue = t.TournamentID.ToString(); } catch { }
            txtName.Text = t.TeamName; txtShort.Text = t.ShortName;
            txtDesc.Text = t.Description;
            txtManager.Text = t.ManagerName;
            try { ddlGameEdit.SelectedValue = t.GameType ?? "League of Legends"; } catch { }
            try { ddlColor.SelectedValue = t.LogoColor ?? "#3b82f6"; } catch { }
            litTitle.Text = "Sửa thông tin đội";
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
        hfId.Value = ""; txtName.Text = ""; txtShort.Text = "";
        txtDesc.Text = ""; txtManager.Text = "";
        litTitle.Text = "Thêm đội mới";
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
