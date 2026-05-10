<%@ Page Title="Quản lý vai trò" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Roles.aspx.cs" Inherits="Admin_Roles" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Quản lý vai trò</h1>
</div>

<div class="row g-3">
    <div class="col-md-5">
        <div class="es-card">
            <h6 class="es-card-title">Danh sách vai trò</h6>
            <asp:Repeater ID="rptRoles" runat="server">
                <ItemTemplate>
                    <div style="border:1px solid var(--border);border-left:3px solid <%# GetColor(Eval("RoleName").ToString()) %>;border-radius:8px;padding:14px;margin-bottom:10px;">
                        <div style="display:flex;align-items:center;gap:8px;margin-bottom:6px;">
                            <i class="<%# GetIcon(Eval("RoleName").ToString()) %>" style="color:<%# GetColor(Eval("RoleName").ToString()) %>;"></i>
                            <strong style="font-size:15px;color:var(--text-main);"><%# Eval("RoleName") %></strong>
                            <span class="es-badge badge-paused" style="margin-left:auto;">MẶC ĐỊNH</span>
                        </div>
                        <div style="color:var(--text-muted);font-size:12px;"><%# Eval("Description") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="col-md-7">
        <div class="es-card">
            <h6 class="es-card-title">Ma trận quyền hạn</h6>
            <table class="es-table" style="border:none;">
                <thead><tr>
                    <th>Chức năng</th><th style="text-align:center;">VIEWER</th>
                    <th style="text-align:center;">TEAM MGR</th><th style="text-align:center;">ADMIN</th>
                </tr></thead>
                <tbody>
                <%
                  string[] funcs = { "Xem lịch thi đấu","Xem kết quả trận đấu","Xem bảng xếp hạng",
                    "Quản lý đội của mình","Quản lý người chơi","Đăng ký đội vào giải",
                    "Tạo / Sửa / Xóa giải đấu","Quản lý lịch thi đấu","Nhập & xác nhận kết quả",
                    "Quản lý tài khoản","Quản lý vai trò" };
                  bool[,] perms = new bool[,] {
                    {true,true,true},   {true,true,true},   {true,true,true},
                    {false,true,true},  {false,true,true},  {false,true,true},
                    {false,false,true}, {false,false,true}, {false,false,true},
                    {false,false,true}, {false,false,true}
                  };
                  for(int i=0;i<funcs.Length;i++){ %>
                    <tr><td><%= funcs[i] %></td>
                        <td style="text-align:center;"><%= perms[i,0]?"<span style='color:var(--accent-green);'>✓</span>":"<span style='color:var(--text-dim);'>—</span>" %></td>
                        <td style="text-align:center;"><%= perms[i,1]?"<span style='color:var(--accent-green);'>✓</span>":"<span style='color:var(--text-dim);'>—</span>" %></td>
                        <td style="text-align:center;"><%= perms[i,2]?"<span style='color:var(--accent-green);'>✓</span>":"<span style='color:var(--text-dim);'>—</span>" %></td>
                    </tr>
                <% } %>
                </tbody>
            </table>
        </div>
    </div>
</div>
</asp:Content>
