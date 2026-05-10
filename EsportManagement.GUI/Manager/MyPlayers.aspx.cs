using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Manager_MyPlayers : Page
{
    private readonly TeamBLL _teamBll = new TeamBLL();
    private readonly PlayerBLL _playerBll = new PlayerBLL();
    private int TeamId
    {
        get { int id; return int.TryParse(Request.QueryString["team"], out id) ? id : 0; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireManager();
        if (TeamId == 0) Response.Redirect("MyTeams.aspx");
        var acc = AuthHelper.Current;
        var myTeams = _teamBll.GetByManager(acc.AccountID);
        var team = myTeams.FirstOrDefault(t => t.TeamID == TeamId);
        if (team == null && !acc.HasRole("Admin")) Response.Redirect("MyTeams.aspx");
        if (team == null) team = _teamBll.GetByID(TeamId);
        if (team != null) litTeam.Text = Server.HtmlEncode(team.TeamName);
        if (!IsPostBack) Bind();
    }

    private void Bind()
    {
        var list = _playerBll.GetByTeam(TeamId);
        gv.DataSource = list; gv.DataBind();
        litCount.Text = list.Count.ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e) { pnlForm.Visible = true; }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            _playerBll.Add(new Player {
                TeamID      = TeamId,
                PlayerName  = txtName.Text.Trim(),
                Nickname    = txtNick.Text.Trim(),
                Position    = ddlPos.SelectedValue,
                ContactInfo = txtContact.Text.Trim(),
                Country     = "Việt Nam",
                IsActive    = true
            });
            Notify("Thêm thành công.", true);
            txtName.Text = txtNick.Text = txtContact.Text = "";
            pnlForm.Visible = false; Bind();
        }
        catch (Exception ex) { Notify(ex.Message, false); pnlForm.Visible = true; }
    }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteItem")
        {
            try { _playerBll.Remove(int.Parse((string)e.CommandArgument)); Notify("Đã xóa.", true); Bind(); }
            catch (Exception ex) { Notify(ex.Message, false); }
        }
    }

    private void Notify(string msg, bool success)
    {
        lblMsg.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMsg.Visible = true;
    }
}
