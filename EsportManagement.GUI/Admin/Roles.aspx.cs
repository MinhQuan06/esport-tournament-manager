using System;
using System.Web.UI;
using EsportManagement.BLL;

public partial class Admin_Roles : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireAdmin();
        rptRoles.DataSource = new RoleBLL().GetAll();
        rptRoles.DataBind();
    }

    public string GetColor(string roleName)
    {
        if (roleName == "Admin") return "#f59e0b";
        if (roleName == "TeamManager") return "#06b6d4";
        return "#8b5cf6";
    }

    public string GetIcon(string roleName)
    {
        if (roleName == "Admin") return "bi bi-shield-fill-check";
        if (roleName == "TeamManager") return "bi bi-controller";
        return "bi bi-eye";
    }
}
