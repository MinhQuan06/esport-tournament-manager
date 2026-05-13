using System;
using System.Web.UI;
using EsportManagement.BLL;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CurrentUser"] != null)
            Response.Redirect("~/Default.aspx");
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedRole = hfRole.Value;
            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                ShowError("Vui long chon vai tro Admin, Team Manager hoac Viewer.");
                return;
            }

            var acc = new AccountBLL().Login(txtUsername.Text, txtPassword.Text, selectedRole);
            if (acc == null) { ShowError("Ten dang nhap, mat khau hoac vai tro khong dung."); return; }
            if (acc.IsLocked) { ShowError("Tai khoan da bi khoa."); return; }
            Session["CurrentUser"] = acc;
            Response.Redirect("~/Default.aspx");
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void ShowError(string msg)
    {
        lblMessage.Text = "<div class='es-alert danger'><i class='bi bi-exclamation-triangle'></i> " +
            Server.HtmlEncode(msg) + "</div>";
        lblMessage.Visible = true;
    }
}
