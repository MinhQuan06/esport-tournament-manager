using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Admin_Accounts : Page
{
    public bool ShowAddModal { get; private set; }
    public bool ShowEditModal { get; private set; }
    private readonly AccountBLL _bll = new AccountBLL();
    private readonly RoleBLL _roleBll = new RoleBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        if (!IsPostBack) Bind();
    }

    private void Bind()
    {
        var list = _bll.GetAll();
        gv.DataSource = list; gv.DataBind();
        litTotal.Text  = list.Count.ToString();
        litActive.Text = list.Count(a => !a.IsLocked).ToString();
        litMgr.Text    = list.Count(a => a.HasRole("TeamManager")).ToString();
        litLocked.Text = list.Count(a => a.IsLocked).ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtAddPwd.Text != txtAddPwd2.Text)
                throw new Exception("Mật khẩu xác nhận không khớp.");

            var acc = _bll.Register(
                txtAddUsername.Text, txtAddPwd.Text,
                txtAddEmail.Text, txtAddFullName.Text);

            // Gán thêm vai trò nếu chọn khác Viewer
            string roleName = ddlAddRole.SelectedValue;
            if (roleName != "Viewer")
            {
                var role = _roleBll.GetByName(roleName);
                if (role != null) _bll.AssignRole(acc.AccountID, role.RoleID);
            }

            // Reset form
            txtAddFullName.Text = txtAddUsername.Text = txtAddEmail.Text = "";
            txtAddPwd.Text = txtAddPwd2.Text = "";
            ddlAddRole.SelectedIndex = 0;

            Notify("Đã tạo tài khoản '" + acc.Username + "' thành công.", true);
            Bind();
        }
        catch (Exception ex)
        {
            Notify(ex.Message, false);
            ShowAddModal = true;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int id = int.Parse(hfId.Value);
            var acc = _bll.GetByID(id);
            if (acc == null) throw new Exception("Tài khoản không tồn tại.");

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
                throw new Exception("Họ tên không được trống.");
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                throw new Exception("Email không được trống.");

            acc.FullName = txtFullName.Text.Trim();
            acc.Email    = txtEmail.Text.Trim();
            acc.IsLocked = chkLocked.Checked;
            _bll.Update(acc);

            Notify("Cập nhật tài khoản thành công.", true);
            Bind();
        }
        catch (Exception ex)
        {
            Notify(ex.Message, false);
            ShowEditModal = true;
        }
    }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = int.Parse((string)e.CommandArgument);
        var acc = _bll.GetByID(id); if (acc == null) return;
        try
        {
            switch (e.CommandName)
            {
                case "EditItem":
                    hfId.Value         = acc.AccountID.ToString();
                    litEditUser.Text   = Server.HtmlEncode(acc.Username);
                    txtFullName.Text   = acc.FullName;
                    txtEmail.Text      = acc.Email;
                    chkLocked.Checked  = acc.IsLocked;
                    ShowEditModal      = true;
                    return;
                case "ToggleLock":
                    acc.IsLocked = !acc.IsLocked;
                    _bll.Update(acc);
                    Notify(acc.IsLocked ? "Đã khóa." : "Đã mở khóa.", true);
                    break;
                case "ToggleManager":
                    var role = _roleBll.GetByName("TeamManager");
                    if (role == null) break;
                    if (acc.HasRole("TeamManager")) _bll.RemoveRole(id, role.RoleID);
                    else _bll.AssignRole(id, role.RoleID);
                    Notify("Đã cập nhật vai trò.", true);
                    break;
                case "ResetPwd":
                    _bll.ResetPassword(id, "123456");
                    Notify("Đã reset mật khẩu về '123456'.", true);
                    break;
                case "DeleteItem":
                    _bll.Delete(id);
                    Notify("Đã xóa tài khoản.", true);
                    break;
            }
            Bind();
        }
        catch (Exception ex) { Notify(ex.Message, false); }
    }

    public string GetRoleBadge(string role)
    {
        if (role == "Admin") return "badge-paused";
        if (role == "Team Manager") return "badge-upcoming";
        return "badge-finished";
    }

    public string GetColor(string s)
    {
        string[] colors = { "#3b82f6","#ef4444","#10b981","#f59e0b","#8b5cf6","#06b6d4" };
        int hash = 0; foreach (var c in s ?? "") hash = (hash*31+c) & 0x7fffffff;
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
