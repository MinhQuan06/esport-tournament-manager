using System;
using System.Web.UI;
using EsportManagement.BLL;

public partial class Register : Page
{
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        if (txtPassword.Text != txtConfirm.Text) { Show("Mật khẩu nhập lại không khớp.", false); return; }
        try
        {
            var acc = new AccountBLL().Register(
                txtUsername.Text, txtPassword.Text, txtEmail.Text, txtFullName.Text);
            Session["CurrentUser"] = acc;
            Show("Đăng ký thành công! Đang chuyển...", true);
            Response.AddHeader("REFRESH", "1.5;URL=Default.aspx");
        }
        catch (Exception ex) { Show(ex.Message, false); }
    }

    private void Show(string msg, bool success)
    {
        lblMessage.Text = "<div class='es-alert " + (success ? "success" : "danger") + "'>" +
            (success ? "<i class='bi bi-check-circle'></i> " : "<i class='bi bi-exclamation-triangle'></i> ") +
            Server.HtmlEncode(msg) + "</div>";
        lblMessage.Visible = true;
    }
}
