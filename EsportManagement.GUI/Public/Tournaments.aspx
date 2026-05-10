<%@ Page Title="Giải đấu" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Tournaments.aspx.cs" Inherits="Public_Tournaments" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Danh sách giải đấu</h1>
</div>

<div class="es-card mb-3">
    <div class="row g-3">
        <div class="col-md-6">
            <asp:TextBox ID="txtKw" runat="server" CssClass="form-control" placeholder="🔍 Tìm giải đấu..." />
        </div>
        <div class="col-md-3">
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                <asp:ListItem Value="" Text="Tất cả trạng thái" />
                <asp:ListItem Value="Chưa bắt đầu" Text="Sắp diễn ra" />
                <asp:ListItem Value="Đang diễn ra" />
                <asp:ListItem Value="Đã kết thúc" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm"
                CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
        </div>
    </div>
</div>

<asp:Repeater ID="rpt" runat="server">
    <HeaderTemplate><div class="row g-3"></HeaderTemplate>
    <ItemTemplate>
        <div class="col-md-4">
            <div class="es-card">
                <div style="display:flex;justify-content:space-between;align-items:center;">
                    <span class="es-badge <%# GetBadge(Eval("Status")) %>"><%# GetBadgeText(Eval("Status")) %></span>
                    <span style="font-size:10px;color:var(--text-muted);"><%# Eval("GameType") %></span>
                </div>
                <h5 style="margin:14px 0 6px;color:var(--text-main);"><%# Eval("TournamentName") %></h5>
                <div style="font-size:12px;color:var(--text-muted);margin-bottom:14px;">
                    <i class="bi bi-calendar"></i>
                    <%# ((DateTime)Eval("StartDate")).ToString("dd/MM") %> -
                    <%# ((DateTime)Eval("EndDate")).ToString("dd/MM/yyyy") %>
                </div>
                <div style="display:flex;gap:10px;font-size:11px;color:var(--text-muted);">
                    <span><i class="bi bi-shield"></i> <%# Eval("TeamCount") %> đội</span>
                    <span><i class="bi bi-lightning"></i> <%# Eval("MatchCount") %> trận</span>
                    <span><i class="bi bi-diagram-3"></i> <%# Eval("Format") %></span>
                </div>
                <div style="display:flex;gap:6px;margin-top:14px;">
                    <a class="btn btn-secondary btn-sm" style="flex:1;"
                       href='<%# ResolveUrl("~/Public/Schedule.aspx?id=" + Eval("TournamentID")) %>'>
                        <i class="bi bi-calendar-event"></i> Lịch
                    </a>
                    <a class="btn btn-primary btn-sm" style="flex:1;"
                       href='<%# ResolveUrl("~/Public/Ranking.aspx?id=" + Eval("TournamentID")) %>'>
                        <i class="bi bi-bar-chart"></i> BXH
                    </a>
                </div>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>
</asp:Content>
