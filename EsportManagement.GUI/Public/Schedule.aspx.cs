using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using EsportManagement.BLL;
using EsportManagement.DTO;

public partial class Public_Schedule : Page
{
    private DateTime CurrentMonth
    {
        get { return ViewState["m"] != null ? (DateTime)ViewState["m"] : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); }
        set { ViewState["m"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthHelper.RequireLogin();
        if (!IsPostBack)
        {
            var tours = new TournamentBLL().GetAll();
            ddlTour.DataSource = tours; ddlTour.DataBind();
            ddlTour.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Tất cả giải --", "0"));
            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
                try { ddlTour.SelectedValue = id.ToString(); } catch { }
        }
        Render();
    }

    protected void ddlTour_Changed(object sender, EventArgs e) { Render(); }
    protected void btnPrev_Click(object sender, EventArgs e) { CurrentMonth = CurrentMonth.AddMonths(-1); Render(); }
    protected void btnNext_Click(object sender, EventArgs e) { CurrentMonth = CurrentMonth.AddMonths(1); Render(); }
    protected void btnToday_Click(object sender, EventArgs e) { CurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); Render(); }

    private void Render()
    {
        litMonth.Text = "Tháng " + CurrentMonth.Month + ", " + CurrentMonth.Year;

        var dal = new EsportManagement.DAL.MatchDAL();
        var allMatches = dal.GetUpcoming(120);
        int tid;
        if (int.TryParse(ddlTour.SelectedValue, out tid) && tid > 0)
            allMatches = allMatches.Where(m => m.TournamentID == tid).ToList();

        // Today
        var today = DateTime.Today;
        var todayMatches = allMatches.Where(m => m.MatchTime.Date == today).ToList();
        rptToday.DataSource = todayMatches;
        rptToday.DataBind();
        pnlNoToday.Visible = todayMatches.Count == 0;

        // Upcoming next 7 days
        var upcoming = allMatches.Where(m => m.MatchTime.Date > today && m.MatchTime <= today.AddDays(7))
                                  .OrderBy(m => m.MatchTime).Take(5).ToList();
        rptUpcoming.DataSource = upcoming; rptUpcoming.DataBind();

        // Calendar grid
        var first = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        // Start at Monday before first day
        int dow = ((int)first.DayOfWeek + 6) % 7; // Mon=0
        var start = first.AddDays(-dow);

        var sb = new StringBuilder();
        for (int week = 0; week < 6; week++)
        {
            sb.Append("<tr>");
            for (int day = 0; day < 7; day++)
            {
                var d = start.AddDays(week * 7 + day);
                bool otherMonth = d.Month != CurrentMonth.Month;
                bool isToday = d.Date == today;
                sb.AppendFormat("<td class='calendar-day{0}{1}'>",
                    otherMonth ? " other-month" : "",
                    isToday ? " today" : "");
                sb.AppendFormat("<div class='day-num'>{0}</div>", d.Day);
                var dayMatches = allMatches.Where(m => m.MatchTime.Date == d.Date).ToList();
                foreach (var m in dayMatches.Take(3))
                {
                    string cls = "";
                    if (m.Status == MatchStatus.DangDienRa) cls = " live";
                    else if (m.Status == MatchStatus.DaKetThuc) cls = " done";
                    sb.AppendFormat("<span class='match-pill{0}' title='{1} vs {2}'>{3} {4} vs {5}</span>",
                        cls,
                        Server.HtmlEncode(m.Team1Name), Server.HtmlEncode(m.Team2Name),
                        m.MatchTime.ToString("HH:mm"),
                        Server.HtmlEncode(m.Team1Short ?? m.Team1Name),
                        Server.HtmlEncode(m.Team2Short ?? m.Team2Name));
                }
                if (dayMatches.Count > 3)
                    sb.AppendFormat("<span style='font-size:10px;color:var(--text-muted);'>+{0} trận</span>",
                        dayMatches.Count - 3);
                sb.Append("</td>");
            }
            sb.Append("</tr>");
            if (start.AddDays(week * 7 + 6).Month > CurrentMonth.Month && week >= 4) break;
        }
        litCalendar.Text = sb.ToString();
    }
}
