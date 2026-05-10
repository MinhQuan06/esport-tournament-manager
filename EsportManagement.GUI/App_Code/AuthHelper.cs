using System.Web;
using EsportManagement.DTO;

/// <summary>
/// Helper kiểm tra phân quyền dựa trên Session.
/// (Viết theo cú pháp C# 5 để tương thích với compiler mặc định của Web Site Project)
/// </summary>
public static class AuthHelper
{
    public static Account Current
    {
        get { return HttpContext.Current.Session["CurrentUser"] as Account; }
    }

    public static bool IsLoggedIn
    {
        get { return Current != null; }
    }

    public static bool IsAdmin
    {
        get { return Current != null && Current.HasRole("Admin"); }
    }

    public static bool IsManager
    {
        get { return Current != null && Current.HasRole("TeamManager"); }
    }

    /// <summary>
    /// Yêu cầu đã đăng nhập, nếu không sẽ redirect Login.
    /// </summary>
    public static void RequireLogin()
    {
        if (!IsLoggedIn)
            HttpContext.Current.Response.Redirect("~/Login.aspx");
    }

    public static void RequireAdmin()
    {
        RequireLogin();
        if (!IsAdmin)
            HttpContext.Current.Response.Redirect("~/Default.aspx?err=forbidden");
    }

    public static void RequireManager()
    {
        RequireLogin();
        if (!IsManager && !IsAdmin)
            HttpContext.Current.Response.Redirect("~/Default.aspx?err=forbidden");
    }
}
