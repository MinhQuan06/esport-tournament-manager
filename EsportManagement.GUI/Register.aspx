<%@ Page Title="Đăng ký" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="login-wrap">
    <div class="login-card">
        <span class="corner-tl"></span><span class="corner-tr"></span>
        <span class="corner-bl"></span><span class="corner-br"></span>
        <div class="login-logo">
            <div class="login-icon-circle"><i class="bi bi-person-plus"></i></div>
            <h1 class="login-title">ĐĂNG KÝ</h1>
            <div class="login-subtitle">TẠO TÀI KHOẢN MỚI</div>
        </div>
        <asp:Label ID="lblMessage" runat="server" Visible="false" />
        <div class="mb-3"><label class="form-label">Họ và tên</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" /></div>
        <div class="mb-3"><label class="form-label">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" /></div>
        <div class="mb-3"><label class="form-label">Tên đăng nhập</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" /></div>
        <div class="mb-3"><label class="form-label">Mật khẩu (≥6 ký tự)</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" /></div>
        <div class="mb-3"><label class="form-label">Nhập lại mật khẩu</label>
            <asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control" TextMode="Password" /></div>
        <asp:Button ID="btnRegister" runat="server" Text="ĐĂNG KÝ"
            CssClass="btn-login-big" OnClick="btnRegister_Click" />
        <div style="text-align:center;margin-top:18px;">
            <small style="color:var(--text-dim);">Đã có tài khoản?
                <a href="<%= ResolveUrl("~/Login.aspx") %>" style="color:var(--accent-cyan);">Đăng nhập</a>
            </small>
        </div>
    </div>
</div>
</asp:Content>
