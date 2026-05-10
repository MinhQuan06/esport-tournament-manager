<%@ Page Title="Lịch thi đấu" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Schedule.aspx.cs" Inherits="Public_Schedule" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Lịch thi đấu</h1>
    <div class="page-actions">
        <asp:DropDownList ID="ddlTour" runat="server" CssClass="form-select"
            AutoPostBack="true" OnSelectedIndexChanged="ddlTour_Changed"
            DataValueField="TournamentID" DataTextField="TournamentName"
            Style="width:240px;display:inline-block;" />
    </div>
</div>

<div class="row g-3">
    <div class="col-lg-8">
        <div class="es-card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px;">
                <div style="display:flex;align-items:center;gap:14px;">
                    <asp:LinkButton ID="btnPrev" runat="server" CssClass="btn-icon"
                        OnClick="btnPrev_Click"><i class="bi bi-chevron-left"></i></asp:LinkButton>
                    <h5 style="margin:0;font-weight:700;color:var(--text-main);">
                        <asp:Literal ID="litMonth" runat="server" />
                    </h5>
                    <asp:LinkButton ID="btnNext" runat="server" CssClass="btn-icon"
                        OnClick="btnNext_Click"><i class="bi bi-chevron-right"></i></asp:LinkButton>
                </div>
                <asp:LinkButton ID="btnToday" runat="server" CssClass="btn btn-sm btn-secondary"
                    OnClick="btnToday_Click">Hôm nay</asp:LinkButton>
            </div>

            <table class="calendar-table">
                <thead><tr>
                    <th>T2</th><th>T3</th><th>T4</th><th>T5</th><th>T6</th><th>T7</th><th>CN</th>
                </tr></thead>
                <tbody>
                    <asp:Literal ID="litCalendar" runat="server" />
                </tbody>
            </table>
        </div>
    </div>

    <div class="col-lg-4">
        <div class="es-card mb-3">
            <h6 class="es-card-title"><i class="bi bi-fire" style="color:var(--accent-red);"></i> Hôm nay · <%= DateTime.Now.ToString("dd/MM") %></h6>
            <asp:Repeater ID="rptToday" runat="server">
                <ItemTemplate>
                    <div style="padding:12px 0;border-bottom:1px solid var(--border);">
                        <div style="font-size:11px;color:var(--text-muted);">
                            <%# ((DateTime)Eval("MatchTime")).ToString("HH:mm") %> ·
                            <%# Eval("TournamentName") ?? "" %>
                        </div>
                        <div style="display:flex;align-items:center;gap:8px;margin-top:6px;">
                            <span style="font-weight:600;font-size:13px;"><%# Eval("Team1Name") %></span>
                            <span class="es-badge badge-upcoming">vs</span>
                            <span style="font-weight:600;font-size:13px;"><%# Eval("Team2Name") %></span>
                        </div>
                        <div style="font-size:11px;color:var(--text-muted);margin-top:4px;">
                            <%# Eval("MatchFormat") %> · <%# Eval("RoundName") %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoToday" runat="server" Visible="false" Style="text-align:center;color:var(--text-muted);padding:14px;font-size:12px;">
                Không có trận đấu hôm nay.
            </asp:Panel>
        </div>

        <div class="es-card">
            <h6 class="es-card-title"><i class="bi bi-calendar-check"></i> Sắp diễn ra</h6>
            <asp:Repeater ID="rptUpcoming" runat="server">
                <ItemTemplate>
                    <div style="padding:10px 0;border-bottom:1px solid var(--border);">
                        <div style="font-size:11px;color:var(--text-muted);">
                            <%# ((DateTime)Eval("MatchTime")).ToString("dd/MM HH:mm") %>
                        </div>
                        <div style="display:flex;align-items:center;gap:8px;margin-top:4px;">
                            <span style="font-weight:600;font-size:13px;"><%# Eval("Team1Name") %></span>
                            <span style="color:var(--text-muted);font-size:11px;">vs</span>
                            <span style="font-weight:600;font-size:13px;"><%# Eval("Team2Name") %></span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>

<style>
.calendar-table { width: 100%; border-collapse: separate; border-spacing: 4px; }
.calendar-table th { padding: 8px; color: var(--text-muted); font-size: 11px; letter-spacing: 1px; font-weight: 700; }
.calendar-day {
    background: var(--bg-input);
    border: 1px solid var(--border);
    border-radius: 6px;
    padding: 8px; height: 90px;
    vertical-align: top;
    font-size: 12px;
}
.calendar-day.other-month { opacity: .3; }
.calendar-day.today { border-color: var(--accent-cyan); background: rgba(0,212,255,0.06); }
.calendar-day .day-num { font-weight: 700; color: var(--text-main); margin-bottom: 4px; }
.calendar-day.today .day-num { color: var(--accent-cyan); }
.match-pill {
    display: block;
    padding: 2px 6px;
    background: rgba(0,212,255,0.15);
    border-left: 3px solid var(--accent-cyan);
    border-radius: 3px;
    margin-bottom: 2px;
    font-size: 10px;
    white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
    color: var(--accent-cyan);
}
.match-pill.live { background: rgba(239,68,68,.15); border-left-color: var(--accent-red); color: var(--accent-red); }
.match-pill.done { background: rgba(107,122,153,.15); border-left-color: var(--text-muted); color: var(--text-muted); }
</style>
</asp:Content>
