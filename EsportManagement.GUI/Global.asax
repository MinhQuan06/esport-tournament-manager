<%@ Application Language="C#" %>
<script runat="server">
    void Application_Start(object sender, EventArgs e) { }
    void Session_Start(object sender, EventArgs e) { }
    void Application_Error(object sender, EventArgs e)
    {
        var ex = Server.GetLastError();
        // Có thể log ex tại đây
    }
</script>
