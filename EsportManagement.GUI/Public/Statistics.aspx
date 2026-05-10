<%@ Page Title="Thống kê" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Public_Statistics" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Thống kê tổng quan</h1>
</div>

<div class="row g-3 mb-4">
    <div class="col-md-3"><div class="metric-card">
        <div class="label">Tổng giải đấu</div>
        <div class="value"><asp:Literal ID="litTotalTour" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card green">
        <div class="label">Tổng đội thi đấu</div>
        <div class="value"><asp:Literal ID="litTotalTeams" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card orange">
        <div class="label">Tổng người chơi</div>
        <div class="value"><asp:Literal ID="litTotalPlayers" runat="server" /></div>
    </div></div>
    <div class="col-md-3"><div class="metric-card purple">
        <div class="label">Tổng trận đấu</div>
        <div class="value"><asp:Literal ID="litTotalMatches" runat="server" /></div>
    </div></div>
</div>

<div class="row g-3">
    <div class="col-md-6">
        <div class="es-card">
            <h6 class="es-card-title">Phân bố giải đấu theo Game</h6>
            <asp:Repeater ID="rptGames" runat="server">
                <ItemTemplate>
                    <div style="margin-bottom:12px;">
                        <div style="display:flex;justify-content:space-between;font-size:13px;margin-bottom:4px;">
                            <span><%# Eval("Key") %></span>
                            <strong style="color:var(--accent-cyan);"><%# Eval("Value") %></strong>
                        </div>
                        <div style="background:var(--bg-input);height:8px;border-radius:4px;overflow:hidden;">
                            <div style="height:100%;background:var(--gradient-btn);width:<%# GetPercent(Eval("Value")) %>%;"></div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <div class="col-md-6">
        <div class="es-card">
            <h6 class="es-card-title">Top 5 đội nhiều trận thắng nhất</h6>
            <asp:Repeater ID="rptTopTeams" runat="server">
                <ItemTemplate>
                    <div style="display:flex;align-items:center;gap:12px;padding:10px 0;border-bottom:1px solid var(--border);">
                        <div style="font-weight:800;color:var(--accent-cyan);width:24px;"><%# Container.ItemIndex + 1 %></div>
                        <div class="team-avatar sm" style="background:<%# Eval("LogoColor") %>;"><%# Eval("ShortName") %></div>
                        <div style="flex:1;">
                            <div style="font-weight:600;"><%# Eval("TeamName") %></div>
                            <div style="font-size:11px;color:var(--text-muted);"><%# Eval("GameType") %></div>
                        </div>
                        <div style="color:var(--accent-green);font-weight:700;"><%# Eval("Wins") %> trận</div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>

<div class="row g-3 mt-1">
    <div class="col-md-6">
        <div class="es-card">
            <h6 class="es-card-title">Trạng thái giải đấu</h6>
            <div style="display:flex;gap:14px;">
                <div style="flex:1;text-align:center;padding:18px;background:rgba(0,212,255,0.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--accent-cyan);"><asp:Literal ID="litUpcoming" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Sắp diễn ra</div>
                </div>
                <div style="flex:1;text-align:center;padding:18px;background:rgba(16,185,129,.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--accent-green);"><asp:Literal ID="litOngoing" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Đang diễn ra</div>
                </div>
                <div style="flex:1;text-align:center;padding:18px;background:rgba(107,122,153,.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--text-muted);"><asp:Literal ID="litFinished" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Kết thúc</div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-md-6">
        <div class="es-card">
            <h6 class="es-card-title">Trạng thái trận đấu</h6>
            <div style="display:flex;gap:14px;">
                <div style="flex:1;text-align:center;padding:18px;background:rgba(0,212,255,0.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--accent-cyan);"><asp:Literal ID="litMatchUpcoming" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Chưa diễn ra</div>
                </div>
                <div style="flex:1;text-align:center;padding:18px;background:rgba(239,68,68,.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--accent-red);"><asp:Literal ID="litMatchLive" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Đang LIVE</div>
                </div>
                <div style="flex:1;text-align:center;padding:18px;background:rgba(16,185,129,.08);border-radius:8px;">
                    <div style="font-size:28px;font-weight:800;color:var(--accent-green);"><asp:Literal ID="litMatchDone" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Đã kết thúc</div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
