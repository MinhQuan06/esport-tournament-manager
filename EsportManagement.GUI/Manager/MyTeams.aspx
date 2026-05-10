<%@ Page Title="Đội của tôi" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="MyTeams.aspx.cs" Inherits="Manager_MyTeams" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Đội của tôi</h1>
    <div class="page-actions">
        <a class="btn btn-primary" href="<%= ResolveUrl("~/Manager/JoinTournament.aspx") %>">
            <i class="bi bi-plus-lg"></i> Đăng ký đội mới
        </a>
    </div>
</div>

<asp:Repeater ID="rpt" runat="server">
    <HeaderTemplate><div class="row g-3"></HeaderTemplate>
    <ItemTemplate>
        <div class="col-md-6 col-lg-4">
            <div class="team-card">
                <div class="team-card-header">
                    <div class="team-avatar lg" style="background:<%# Eval("LogoColor") %>;"><%# Eval("ShortName") %></div>
                    <div class="team-card-title">
                        <h5><%# Eval("TeamName") %></h5>
                        <div class="game"><%# Eval("GameType") %></div>
                    </div>
                </div>
                <div class="team-stats">
                    <div class="team-stat"><div class="value cyan"><%# Eval("PlayerCount") %></div><div class="label">PLAYER</div></div>
                    <div class="team-stat"><div class="value green"><%# Eval("Wins") %></div><div class="label">THẮNG</div></div>
                    <div class="team-stat"><div class="value red"><%# Eval("Losses") %></div><div class="label">THUA</div></div>
                    <div class="team-stat"><div class="value">#<%# Eval("Rank") %></div><div class="label">XẾP HẠNG</div></div>
                </div>
                <div style="font-size:11px;color:var(--text-muted);margin-bottom:8px;">
                    <i class="bi bi-trophy"></i> <%# Eval("TournamentName") %>
                </div>
                <a class="btn btn-primary btn-sm w-100"
                   href='<%# ResolveUrl("~/Manager/MyPlayers.aspx?team=" + Eval("TeamID")) %>'>
                    <i class="bi bi-person-lines-fill"></i> Quản lý người chơi
                </a>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="es-card" style="text-align:center;padding:40px;color:var(--text-muted);">
        <i class="bi bi-shield-x" style="font-size:48px;"></i>
        <p style="margin-top:14px;">Bạn chưa có đội nào.</p>
        <a class="btn btn-primary" href="<%= ResolveUrl("~/Manager/JoinTournament.aspx") %>">Đăng ký giải đấu</a>
    </div>
</asp:Panel>
</asp:Content>
