<%@ Page Title="Quản lý người chơi" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Players.aspx.cs" Inherits="Admin_Players" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Quản lý người chơi</h1>
    <div class="page-actions">
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mdl">
            <i class="bi bi-plus-lg"></i> Thêm người chơi
        </button>
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="es-card mb-3">
    <div class="row g-3">
        <div class="col-md-5">
            <asp:TextBox ID="txtKw" runat="server" CssClass="form-control" placeholder="🔍 Tìm theo tên, nick name..." />
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlTeam" runat="server" CssClass="form-select"
                DataValueField="TeamID" DataTextField="TeamName" AppendDataBoundItems="true">
                <asp:ListItem Value="" Text="Tất cả đội" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlPos" runat="server" CssClass="form-select">
                <asp:ListItem Value="" Text="Tất cả vị trí" />
                <asp:ListItem Value="Top"     Text="Top" />
                <asp:ListItem Value="Jungle"  Text="Jungle" />
                <asp:ListItem Value="Mid"     Text="Mid" />
                <asp:ListItem Value="ADC"     Text="ADC" />
                <asp:ListItem Value="Support" Text="Support" />
                <asp:ListItem Value="Duelist" Text="Duelist" />
                <asp:ListItem Value="Sentinel" Text="Sentinel" />
            </asp:DropDownList>
        </div>
        <div class="col-md-1">
            <asp:Button ID="btnFilter" runat="server" Text="Lọc"
                CssClass="btn btn-primary w-100" OnClick="btnFilter_Click" />
        </div>
    </div>
</div>

<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
    CssClass="es-table" GridLines="None" DataKeyNames="PlayerID"
    OnRowCommand="gv_RowCommand"
    EmptyDataText="<div style='padding:30px;text-align:center;color:var(--text-muted);'>Chưa có người chơi.</div>">
    <Columns>
        <asp:TemplateField HeaderText="#"><ItemTemplate>
            <span class="num"><%# string.Format("{0:000}", Eval("PlayerID")) %></span>
        </ItemTemplate></asp:TemplateField>
        <asp:TemplateField HeaderText="Người chơi">
            <ItemTemplate>
                <div style="display:flex;align-items:center;gap:10px;">
                    <div class="player-avatar" style="background:<%# GetAvatarColor(Eval("PlayerName").ToString()) %>;">
                        <%# GetInitial(Eval("PlayerName").ToString()) %>
                    </div>
                    <div>
                        <div style="font-weight:600;color:var(--text-main);"><%# Eval("PlayerName") %></div>
                        <div style="font-size:11px;color:var(--text-muted);font-family:monospace;"><%# Eval("Nickname") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vị trí">
            <ItemTemplate>
                <span class="es-badge badge-upcoming"><%# Eval("Position") %></span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="TeamName" HeaderText="Đội" />
        <asp:TemplateField HeaderText="Quốc tịch">
            <ItemTemplate>🇻🇳 <%# Eval("Country") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ngày sinh">
            <ItemTemplate><%# Eval("BirthDate") == null ? "-" : ((DateTime)Eval("BirthDate")).ToString("dd/MM/yyyy") %></ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ContactInfo" HeaderText="Liên hệ" />
        <asp:TemplateField HeaderText="Trạng thái">
            <ItemTemplate>
                <span class="es-badge <%# (bool)Eval("IsActive") ? "badge-active" : "badge-finished" %>">
                    <%# (bool)Eval("IsActive") ? "ACTIVE" : "INACTIVE" %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-edit"
                    CommandName="EditItem" CommandArgument='<%# Eval("PlayerID") %>'>
                    <i class="bi bi-pencil"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-delete"
                    CommandName="DeleteItem" CommandArgument='<%# Eval("PlayerID") %>'
                    OnClientClick="return confirm('Xóa người chơi này?');">
                    <i class="bi bi-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<div class="modal fade" id="mdl" tabindex="-1">
  <div class="modal-dialog"><div class="modal-content">
    <div class="modal-header">
        <h5 class="modal-title"><asp:Literal ID="litTitle" runat="server" Text="Thêm người chơi" /></h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
        <asp:HiddenField ID="hfId" runat="server" />
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Họ tên *</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Nickname</label>
                <asp:TextBox ID="txtNick" runat="server" CssClass="form-control" placeholder="Faker, T1_Faker..." />
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Đội *</label>
                <asp:DropDownList ID="ddlTeamEdit" runat="server" CssClass="form-select"
                    DataValueField="TeamID" DataTextField="TeamName" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Vị trí</label>
                <asp:DropDownList ID="ddlPosEdit" runat="server" CssClass="form-select">
                    <asp:ListItem Value="" Text="-- chọn --" />
                    <asp:ListItem Value="Top" />
                    <asp:ListItem Value="Jungle" />
                    <asp:ListItem Value="Mid" />
                    <asp:ListItem Value="ADC" />
                    <asp:ListItem Value="Support" />
                    <asp:ListItem Value="Duelist" />
                    <asp:ListItem Value="Sentinel" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Quốc tịch</label>
                <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control" Text="Việt Nam" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Ngày sinh</label>
                <asp:TextBox ID="txtBirth" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>
        <div class="mb-3">
            <label class="form-label">Liên hệ (email/SĐT)</label>
            <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" />
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <asp:Button ID="btnSave" runat="server" Text="Lưu người chơi" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    </div>
  </div></div>
</div>
<% if (ShowModal) { %>
<script>document.addEventListener('DOMContentLoaded',function(){new bootstrap.Modal(document.getElementById('mdl')).show();});</script>
<% } %>
</asp:Content>
