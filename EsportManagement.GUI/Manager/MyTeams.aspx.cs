using System;
using System.Web.UI;
using EsportManagement.BLL;

public partial class Manager_MyTeams : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireManager();
        var list = new TeamBLL().GetByManager(AuthHelper.Current.AccountID);
        rpt.DataSource = list; rpt.DataBind();
        pnlEmpty.Visible = list.Count == 0;
    }
}
