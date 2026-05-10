<%@ Page Title="Quản lý tài khoản" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Accounts.aspx.cs" Inherits="Admin_Accounts" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Quản lý tài khoản</h1>
    <div class="page-actions">
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mdlAdd">
            <i class="bi bi-plus-lg"></i> Tạo tài khoản mới
        </button>
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="row g-3 mb-4">
    <div class="col-md-3"><div class="metric-card">
        <div class="label">Tổng tài khoản</div>
        <div class="value"><asp:Literal ID="litTotal" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card green">
        <div class="label">Đang hoạt động</div>
        <div class="value"><asp:Literal ID="litActive" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card orange">
        <div class="label">Team Manager</div>
        <div class="value"><asp:Literal ID="litMgr" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card" style="border-top-color:var(--accent-red);">
        <div class="label">Bị khóa</div>
        <div class="value" style="color:var(--accent-red);"><asp:Literal ID="litLocked" runat="server" /></div>
    </div></div>
</div>

<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
    CssClass="es-table" GridLines="None" DataKeyNames="AccountID"
    OnRowCommand="gv_RowCommand">
    <Columns>
        <asp:TemplateField HeaderText="#"><ItemTemplate>
            <span class="num"><%# string.Format("{0:000}", Eval("AccountID")) %></span>
        </ItemTemplate></asp:TemplateField>
        <asp:TemplateField HeaderText="Tài khoản">
            <ItemTemplate>
                <div style="display:flex;align-items:center;gap:10px;">
                    <div class="player-avatar" style="background:<%# GetColor(Eval("Username").ToString()) %>;">
                        <%# Eval("Initials") %>
                    </div>
                    <div>
                        <div style="font-weight:600;color:var(--text-main);"><%# Eval("Username") %></div>
                        <div style="font-size:11px;color:var(--text-muted);"><%# Eval("FullName") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Email" HeaderText="Email" />
        <asp:TemplateField HeaderText="Vai trò">
            <ItemTemplate>
                <span class="es-badge <%# GetRoleBadge(((EsportManagement.DTO.Account)Container.DataItem).PrimaryRole) %>">
                    <%# ((EsportManagement.DTO.Account)Container.DataItem).PrimaryRole %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ngày tạo">
            <ItemTemplate><%# ((DateTime)Eval("CreatedAt")).ToString("dd/MM/yyyy") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Đăng nhập cuối">
            <ItemTemplate><%# Eval("LastLoginAt") == null ? "-" : ((DateTime)Eval("LastLoginAt")).ToString("dd/MM HH:mm") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Trạng thái">
            <ItemTemplate>
                <%# (bool)Eval("IsLocked")
                    ? "<span class='es-badge badge-locked'>BỊ KHÓA</span>"
                    : "<span class='es-badge badge-active'>ACTIVE</span>" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-edit"
                    CommandName="EditItem" CommandArgument='<%# Eval("AccountID") %>' ToolTip="Sửa thông tin">
                    <i class="bi bi-pencil"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon"
                    CommandName="ToggleLock" CommandArgument='<%# Eval("AccountID") %>' ToolTip="Khóa/Mở">
                    <i class="bi bi-lock"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon"
                    CommandName="ToggleManager" CommandArgument='<%# Eval("AccountID") %>' ToolTip="Bật/tắt vai trò Manager">
                    <i class="bi bi-shield"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon"
                    CommandName="ResetPwd" CommandArgument='<%# Eval("AccountID") %>'
                    OnClientClick="return confirm('Reset mật khẩu về 123456?');" ToolTip="Reset mật khẩu">
                    <i class="bi bi-key"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-delete"
                    CommandName="DeleteItem" CommandArgument='<%# Eval("AccountID") %>'
                    OnClientClick="return confirm('Xóa tài khoản?');" ToolTip="Xóa">
                    <i class="bi bi-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<!-- Modal: Tạo tài khoản mới -->
<div class="modal fade" id="mdlAdd" tabindex="-1">
  <div class="modal-dialog"><div class="modal-content">
    <div class="modal-header">
        <h5 class="modal-title"><i class="bi bi-plus-circle text-info"></i> Tạo tài khoản mới</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Họ và tên *</label>
                <asp:TextBox ID="txtAddFullName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Tên đăng nhập *</label>
                <asp:TextBox ID="txtAddUsername" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="mb-3">
            <label class="form-label">Email *</label>
            <asp:TextBox ID="txtAddEmail" runat="server" CssClass="form-control" TextMode="Email" />
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Mật khẩu *</label>
                <asp:TextBox ID="txtAddPwd" runat="server" CssClass="form-control" TextMode="Password" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Xác nhận mật khẩu *</label>
                <asp:TextBox ID="txtAddPwd2" runat="server" CssClass="form-control" TextMode="Password" />
            </div>
        </div>
        <div class="mb-3">
            <label class="form-label">Vai trò</label>
            <asp:DropDownList ID="ddlAddRole" runat="server" CssClass="form-select">
                <asp:ListItem Value="Viewer" Text="Viewer (mặc định)" />
                <asp:ListItem Value="TeamManager" Text="Team Manager" />
                <asp:ListItem Value="Admin" Text="Admin" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <asp:Button ID="btnAdd" runat="server" Text="Tạo tài khoản" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </div>
  </div></div>
</div>

<!-- Modal: Sửa thông tin tài khoản -->
<div class="modal fade" id="mdlEdit" tabindex="-1">
  <div class="modal-dialog"><div class="modal-content">
    <div class="modal-header">
        <h5 class="modal-title"><i class="bi bi-pencil text-warning"></i> Sửa tài khoản: <asp:Literal ID="litEditUser" runat="server" /></h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
        <asp:HiddenField ID="hfId" runat="server" />
        <div class="mb-3">
            <label class="form-label">Họ và tên *</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label class="form-label">Email *</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
        </div>
        <div class="mb-3">
            <asp:CheckBox ID="chkLocked" runat="server" Text=" Khóa tài khoản" CssClass="form-check-input" />
        </div>
        <div class="es-alert info" style="font-size:12px;">
            <i class="bi bi-info-circle"></i> Username không thể thay đổi (BR-ACC-06).
            Để đổi mật khẩu dùng nút <i class="bi bi-key"></i> ở danh sách.
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <asp:Button ID="btnSave" runat="server" Text="Lưu thay đổi" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    </div>
  </div></div>
</div>

<% if (ShowAddModal) { %>
<script>document.addEventListener('DOMContentLoaded',function(){new bootstrap.Modal(document.getElementById('mdlAdd')).show();});</script>
<% } %>
<% if (ShowEditModal) { %>
<script>document.addEventListener('DOMContentLoaded',function(){new bootstrap.Modal(document.getElementById('mdlEdit')).show();});</script>
<% } %>
</asp:Content>
