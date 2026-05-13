<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="login-wrap">
    <div class="login-card">
        <span class="corner-tl"></span><span class="corner-tr"></span>
        <span class="corner-bl"></span><span class="corner-br"></span>

        <div class="login-logo">
            <div class="login-icon-circle"><i class="bi bi-trophy-fill"></i></div>
            <h1 class="login-title">ESPORT <span class="brand-accent">PRO</span></h1>
            <div class="login-subtitle">TOURNAMENT MANAGEMENT SYSTEM</div>
        </div>

        <asp:Label ID="lblMessage" runat="server" Visible="false" />
        <asp:HiddenField ID="hfRole" runat="server" />

        <div class="mb-3">
            <label class="form-label">Tên đăng nhập</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="admin, teammanager hoặc viewer đã đăng ký" />
        </div>
        <div class="mb-3">
            <label class="form-label">Mật khẩu</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••" />
        </div>
        <div class="mb-3">
            <label class="form-label">Vai trò đăng nhập *</label>
            <div class="role-chips">
                <div class="chip" data-role="Admin" onclick="setLogin('admin','admin123','Admin', this)">
                    <i class="bi bi-shield-fill-check"></i> Admin
                </div>
                <div class="chip" data-role="TeamManager" onclick="setLogin('teammanager','teammanager123','TeamManager', this)">
                    <i class="bi bi-people-fill"></i> Team<br/>Manager
                </div>
                <div class="chip" data-role="Viewer" onclick="setRole('Viewer', this)">
                    <i class="bi bi-eye"></i> Viewer
                </div>
            </div>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="ĐĂNG NHẬP"
            CssClass="btn-login-big" OnClick="btnLogin_Click" />

        <div style="text-align:center;margin-top:18px;">
            <small style="color:var(--text-dim);">Chưa có tài khoản Viewer?
                <a href="<%= ResolveUrl("~/Register.aspx") %>" style="color:var(--accent-cyan);">Đăng ký</a>
            </small>
        </div>

        <div style="text-align:right;margin-top:14px;font-size:9px;color:var(--text-dim);">v2.1.0</div>
    </div>
</div>

<script>
    function setLogin(u, p, role, el) {
        document.getElementById('<%= txtUsername.ClientID %>').value = u;
        document.getElementById('<%= txtPassword.ClientID %>').value = p;
        setRole(role, el);
    }
    function setRole(role, el) {
        document.getElementById('<%= hfRole.ClientID %>').value = role;
        document.querySelectorAll('.role-chips .chip').forEach(c=>c.classList.remove('active'));
        el.classList.add('active');
    }
</script>
</asp:Content>
