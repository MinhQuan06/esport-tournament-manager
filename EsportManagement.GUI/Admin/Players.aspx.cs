using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Players : Page
{
    public bool ShowModal { get; private set; }
    private readonly PlayerBLL _bll = new PlayerBLL();
    private readonly TeamBLL _teamBll = new TeamBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        if (!IsPostBack)
        {
            BindTeams();
            Bind();
        }
    }

    private void BindTeams()
    {
        var teams = _teamBll.GetAll();
        ddlTeam.DataSource = teams; ddlTeam.DataBind();
        ddlTeamEdit.DataSource = teams; ddlTeamEdit.DataBind();
    }

    private void Bind()
    {
        int? tid = null;
        int t;
        if (int.TryParse(ddlTeam.SelectedValue, out t) && t > 0) tid = t;
        gv.DataSource = _bll.Search(txtKw.Text.Trim(), tid, ddlPos.SelectedValue);
        gv.DataBind();
    }

    protected void btnFilter_Click(object sender, EventArgs e) { Bind(); }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            var p = new Player
            {
                TeamID      = int.Parse(ddlTeamEdit.SelectedValue),
                PlayerName  = txtName.Text.Trim(),
                Nickname    = txtNick.Text.Trim(),
                Position    = ddlPosEdit.SelectedValue,
                Country     = string.IsNullOrEmpty(txtCountry.Text) ? "Việt Nam" : txtCountry.Text.Trim(),
                BirthDate   = string.IsNullOrEmpty(txtBirth.Text) ? (DateTime?)null : DateTime.Parse(txtBirth.Text),
                ContactInfo = txtContact.Text.Trim(),
                IsActive    = true
            };
            int id;
            if (int.TryParse(hfId.Value, out id) && id > 0)
            { p.PlayerID = id; _bll.Update(p); Notify("Cập nhật thành công.", true); }
            else
            { _bll.Add(p); Notify("Thêm người chơi thành công.", true); }
            ResetForm(); Bind();
        }
        catch (Exception ex) { Notify(ex.Message, false); ShowModal = true; }
    }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = int.Parse((string)e.CommandArgument);
        if (e.CommandName == "EditItem")
        {
            var p = _bll.GetByID(id); if (p == null) return;
            hfId.Value = p.PlayerID.ToString();
            txtName.Text = p.PlayerName; txtNick.Text = p.Nickname;
            try { ddlTeamEdit.SelectedValue = p.TeamID.ToString(); } catch { }
            try { ddlPosEdit.SelectedValue = p.Position ?? ""; } catch { }
            txtCountry.Text = p.Country;
            txtBirth.Text = p.BirthDate.HasValue ? p.BirthDate.Value.ToString("yyyy-MM-dd") : "";
            txtContact.Text = p.ContactInfo;
            litTitle.Text = "Sửa người chơi";
            ShowModal = true;
        }
        else if (e.CommandName == "DeleteItem")
        {
            try { _bll.Remove(id); Notify("Đã xóa.", true); Bind(); }
            catch (Exception ex) { Notify(ex.Message, false); }
        }
    }

    private void ResetForm()
    {
        hfId.Value = ""; txtName.Text = ""; txtNick.Text = ""; txtBirth.Text = "";
        txtContact.Text = ""; txtCountry.Text = "Việt Nam";
        ddlPosEdit.SelectedIndex = 0;
        litTitle.Text = "Thêm người chơi";
    }

    public string GetInitial(string name)
    {
        if (string.IsNullOrEmpty(name)) return "?";
        var parts = name.Trim().Split(' ');
        if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
        return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
    }

    public string GetAvatarColor(string name)
    {
        string[] colors = { "#3b82f6", "#ef4444", "#10b981", "#f59e0b", "#8b5cf6", "#06b6d4", "#ec4899" };
        int hash = 0;
        foreach (var c in name ?? "") hash = (hash * 31 + c) & 0x7fffffff;
        return colors[hash % colors.Length];
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
