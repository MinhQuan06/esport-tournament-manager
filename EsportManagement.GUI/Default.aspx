<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="DefaultPage" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Dashboard</h1>
    <div class="page-actions">
        <span style="color:var(--text-muted);font-size:13px;">
            <i class="bi bi-calendar3"></i> <asp:Literal ID="litToday" runat="server" />
        </span>
    </div>
</div>

<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="es-alert warning">
    <i class="bi bi-exclamation-triangle"></i> Bạn không có quyền truy cập chức năng vừa yêu cầu.
</asp:Panel>

<div class="row g-3 mb-4">
    <div class="col-md-3">
        <div class="metric-card">
            <div class="label">Tổng giải đấu</div>
            <div class="value"><asp:Literal ID="litTour" runat="server" /></div>
            <div class="sub"><span class="up">+<asp:Literal ID="litTourNew" runat="server" /></span> tháng này</div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="metric-card green">
            <div class="label">Đội thi đấu</div>
            <div class="value"><asp:Literal ID="litTeams" runat="server" /></div>
            <div class="sub"><span class="up">Active</span> mọi giải</div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="metric-card orange">
            <div class="label">Người chơi</div>
            <div class="value"><asp:Literal ID="litPlayers" runat="server" /></div>
            <div class="sub">Tổng số đăng ký</div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="metric-card purple">
            <div class="label">Trận đấu</div>
            <div class="value"><asp:Literal ID="litMatches" runat="server" /></div>
            <div class="sub"><asp:Literal ID="litOngoing" runat="server" /> đang diễn ra</div>
        </div>
    </div>
</div>

<div class="row g-3">
    <div class="col-lg-7">
        <div class="es-card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px;">
                <h6 class="es-card-title" style="margin:0;">Giải đấu gần đây</h6>
                <a href="<%= ResolveUrl("~/Public/Tournaments.aspx") %>" style="color:var(--accent-cyan);font-size:12px;">Xem tất cả →</a>
            </div>
            <asp:Repeater ID="rptTours" runat="server">
                <ItemTemplate>
                    <div style="display:flex;align-items:center;gap:14px;padding:12px 0;border-bottom:1px solid var(--border);">
                        <div class="team-avatar" style="background:<%# GetGameColor(Eval("GameType").ToString()) %>;">
                            <i class="<%# GetGameIcon(Eval("GameType").ToString()) %>"></i>
                        </div>
                        <div style="flex:1;">
                            <div style="font-weight:600;color:var(--text-main);"><%# Eval("TournamentName") %></div>
                            <div style="font-size:11px;color:var(--text-muted);">
                                <%# Eval("GameType") %> · <%# Eval("TeamCount") %> đội · <%# Eval("Format") %>
                            </div>
                        </div>
                        <span class="es-badge <%# GetBadgeClass((EsportManagement.DTO.TournamentStatus)Eval("Status")) %>">
                            <%# Eval("StatusText") %>
                        </span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="col-lg-5">
        <div class="es-card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px;">
                <h6 class="es-card-title" style="margin:0;">Trận đấu sắp tới</h6>
                <a href="<%= ResolveUrl("~/Public/Schedule.aspx") %>" style="color:var(--accent-cyan);font-size:12px;">Xem lịch →</a>
            </div>
            <asp:Repeater ID="rptMatches" runat="server">
                <ItemTemplate>
                    <div style="display:flex;align-items:center;justify-content:space-between;padding:10px 0;border-bottom:1px solid var(--border);">
                        <div style="display:flex;align-items:center;gap:8px;flex:1;">
                            <span style="font-weight:600;color:var(--text-main);"><%# Eval("Team1Name") %></span>
                            <span style="color:var(--text-muted);font-size:11px;">vs</span>
                            <span style="font-weight:600;color:var(--text-main);"><%# Eval("Team2Name") %></span>
                        </div>
                        <div style="text-align:right;">
                            <%# Eval("ScoreTeam1") == null
                                ? "<span style='color:var(--text-dim);'>" + ((DateTime)Eval("MatchTime")).ToString("HH:mm") + "</span>"
                                : "<span style='font-weight:700;color:var(--text-main);'>" + Eval("ScoreTeam1") + " : " + Eval("ScoreTeam2") + "</span>" %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
</asp:Content>
