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
            var acc = new AccountBLL().Login(txtUsername.Text, txtPassword.Text);
            if (acc == null) { ShowError("Tên đăng nhập hoặc mật khẩu không đúng."); return; }
            if (acc.IsLocked) { ShowError("Tài khoản đã bị khóa."); return; }
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
