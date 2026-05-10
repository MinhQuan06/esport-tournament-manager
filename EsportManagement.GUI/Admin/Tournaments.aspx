<%@ Page Title="Quản lý giải đấu" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Tournaments.aspx.cs" Inherits="Admin_Tournaments" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Quản lý giải đấu</h1>
    <div class="page-actions">
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#mdlAdd">
            <i class="bi bi-plus-lg"></i> Tạo giải đấu mới
        </button>
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="es-card mb-3">
    <div class="row g-3">
        <div class="col-md-5">
            <asp:TextBox ID="txtKw" runat="server" CssClass="form-control" placeholder="🔍 Tìm kiếm giải đấu..." />
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                <asp:ListItem Value="" Text="Tất cả trạng thái" />
                <asp:ListItem Value="Chưa bắt đầu" Text="Sắp diễn ra" />
                <asp:ListItem Value="Đang diễn ra" Text="Đang diễn ra" />
                <asp:ListItem Value="Đã kết thúc"  Text="Kết thúc" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlGame" runat="server" CssClass="form-select">
                <asp:ListItem Value="" Text="Tất cả game" />
                <asp:ListItem Value="League of Legends" Text="League of Legends" />
                <asp:ListItem Value="CS:GO" Text="CS:GO" />
                <asp:ListItem Value="Valorant" Text="Valorant" />
                <asp:ListItem Value="Dota 2" Text="Dota 2" />
                <asp:ListItem Value="Mobile Legends" Text="Mobile Legends" />
                <asp:ListItem Value="PUBG" Text="PUBG" />
            </asp:DropDownList>
        </div>
        <div class="col-md-1">
            <asp:Button ID="btnSearch" runat="server" Text="Lọc"
                CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
        </div>
    </div>
</div>

<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
    CssClass="es-table" GridLines="None" DataKeyNames="TournamentID"
    OnRowCommand="gv_RowCommand"
    EmptyDataText="<div style='padding:30px;text-align:center;color:var(--text-muted);'>Không có giải đấu nào.</div>">
    <Columns>
        <asp:TemplateField HeaderText="#"><ItemTemplate>
            <span class="num"><%# string.Format("{0:000}", Eval("TournamentID")) %></span>
        </ItemTemplate></asp:TemplateField>
        <asp:BoundField DataField="TournamentName" HeaderText="Tên giải đấu" />
        <asp:BoundField DataField="GameType" HeaderText="Game" />
        <asp:BoundField DataField="TeamCount" HeaderText="Số đội" />
        <asp:BoundField DataField="Format" HeaderText="Thể thức" />
        <asp:TemplateField HeaderText="Ngày bắt đầu">
            <ItemTemplate><%# ((DateTime)Eval("StartDate")).ToString("dd/MM/yyyy") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ngày kết thúc">
            <ItemTemplate><%# ((DateTime)Eval("EndDate")).ToString("dd/MM/yyyy") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Trạng thái">
            <ItemTemplate>
                <span class="es-badge <%# GetBadgeClass((EsportManagement.DTO.TournamentStatus)Eval("Status")) %>">
                    <%# GetBadgeText((EsportManagement.DTO.TournamentStatus)Eval("Status")) %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
                <a href='<%# ResolveUrl("~/Public/Schedule.aspx?id=" + Eval("TournamentID")) %>'
                   class="btn-icon btn-icon-view" title="Xem"><i class="bi bi-eye"></i></a>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-edit"
                    CommandName="EditItem" CommandArgument='<%# Eval("TournamentID") %>'
                    ToolTip="Sửa"><i class="bi bi-pencil"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-delete"
                    CommandName="DeleteItem" CommandArgument='<%# Eval("TournamentID") %>'
                    OnClientClick="return confirm('Xóa giải đấu này?');"
                    ToolTip="Xóa"><i class="bi bi-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<div class="modal fade" id="mdlAdd" tabindex="-1">
  <div class="modal-dialog modal-lg"><div class="modal-content">
    <div class="modal-header">
        <h5 class="modal-title"><i class="bi bi-plus-circle text-info"></i>
            <asp:Literal ID="litMdlTitle" runat="server" Text="Tạo giải đấu mới" /></h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
        <asp:HiddenField ID="hfId" runat="server" />
        <div class="mb-3">
            <label class="form-label">Tên giải đấu *</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Game *</label>
                <asp:DropDownList ID="ddlGameEdit" runat="server" CssClass="form-select">
                    <asp:ListItem Value="League of Legends" />
                    <asp:ListItem Value="CS:GO" />
                    <asp:ListItem Value="Valorant" />
                    <asp:ListItem Value="Dota 2" />
                    <asp:ListItem Value="Mobile Legends" />
                    <asp:ListItem Value="PUBG" />
                    <asp:ListItem Value="Khác" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Thể thức *</label>
                <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-select">
                    <asp:ListItem Value="Round Robin" />
                    <asp:ListItem Value="Single Elim." />
                    <asp:ListItem Value="Double Elim." />
                    <asp:ListItem Value="Group Stage" />
                    <asp:ListItem Value="Battle Royale" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Ngày bắt đầu *</label>
                <asp:TextBox ID="txtStart" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Ngày kết thúc *</label>
                <asp:TextBox ID="txtEnd" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>
        <div class="mb-3">
            <label class="form-label">Trạng thái</label>
            <asp:DropDownList ID="ddlStatusEdit" runat="server" CssClass="form-select">
                <asp:ListItem Value="Chưa bắt đầu" />
                <asp:ListItem Value="Đang diễn ra" />
                <asp:ListItem Value="Đã kết thúc" />
            </asp:DropDownList>
        </div>
        <div class="mb-3">
            <label class="form-label">Mô tả</label>
            <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <asp:Button ID="btnSave" runat="server" Text="Lưu giải đấu" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    </div>
  </div></div>
</div>
<% if (ShowModal) { %>
<script>document.addEventListener('DOMContentLoaded',function(){new bootstrap.Modal(document.getElementById('mdlAdd')).show();});</script>
<% } %>
</asp:Content>
