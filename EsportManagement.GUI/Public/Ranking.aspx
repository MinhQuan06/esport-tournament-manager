<%@ Page Title="Bảng xếp hạng" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Ranking.aspx.cs" Inherits="Public_Ranking" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Kết quả &amp; Bảng xếp hạng</h1>
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
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px;">
                <h6 class="es-card-title" style="margin:0;">Bảng xếp hạng</h6>
                <span style="font-size:11px;color:var(--text-muted);">Cập nhật: <%= DateTime.Now.ToString("dd/MM/yyyy") %></span>
            </div>
            <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
                CssClass="es-table" GridLines="None"
                OnRowDataBound="gv_RowDataBound"
                EmptyDataText="<div style='padding:30px;text-align:center;color:var(--text-muted);'>Chưa có dữ liệu xếp hạng.</div>">
                <Columns>
                    <asp:TemplateField HeaderText="#"><ItemTemplate>
                        <span class="rank-num"><%# Eval("Rank") %></span>
                    </ItemTemplate></asp:TemplateField>
                    <asp:TemplateField HeaderText="Đội">
                        <ItemTemplate>
                            <div style="display:flex;align-items:center;gap:10px;">
                                <div class="team-avatar sm" style="background:<%# GetTeamColor(Eval("TeamID")) %>;">
                                    <%# GetTeamShort(Eval("TeamID")) %>
                                </div>
                                <strong><%# Eval("TeamName") %></strong>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Played"   HeaderText="T" />
                    <asp:BoundField DataField="Wins"     HeaderText="T.T" />
                    <asp:BoundField DataField="Losses"   HeaderText="T.B" />
                    <asp:TemplateField HeaderText="Điểm">
                        <ItemTemplate><strong style="color:var(--accent-cyan);"><%# Eval("Points") %></strong></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Hiệu số">
                        <ItemTemplate>
                            <span style="color:<%# (int)Eval("GoalDiff") >= 0 ? "var(--accent-green)" : "var(--accent-red)" %>;">
                                <%# (int)Eval("GoalDiff") >= 0 ? "+" : "" %><%# Eval("GoalDiff") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="col-lg-4">
        <div class="es-card mb-3">
            <h6 class="es-card-title"><i class="bi bi-trophy-fill" style="color:var(--accent-orange);"></i> Top 3 hiện tại</h6>
            <asp:Repeater ID="rptTop3" runat="server">
                <ItemTemplate>
                    <div style="display:flex;align-items:center;gap:12px;padding:10px 0;border-bottom:1px solid var(--border);">
                        <div style="font-size:18px;font-weight:800;color:var(--accent-cyan);width:24px;"><%# Eval("Rank") %></div>
                        <div class="team-avatar sm" style="background:<%# GetTeamColor(Eval("TeamID")) %>;"><%# GetTeamShort(Eval("TeamID")) %></div>
                        <div style="flex:1;font-weight:600;"><%# Eval("TeamName") %></div>
                        <div style="font-weight:700;color:var(--text-main);"><%# Eval("Points") %> <span style="font-size:10px;color:var(--text-muted);">pts</span></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="es-card">
            <h6 class="es-card-title"><i class="bi bi-clock-history"></i> Kết quả gần đây</h6>
            <asp:Repeater ID="rptRecent" runat="server">
                <ItemTemplate>
                    <div style="padding:10px 0;border-bottom:1px solid var(--border);">
                        <div style="font-size:10px;color:var(--text-muted);letter-spacing:1px;margin-bottom:6px;">
                            <%# ((DateTime)Eval("MatchTime")).ToString("dd/MM") %> ·
                            <%# Eval("RoundName") %>
                        </div>
                        <div style="display:flex;align-items:center;gap:8px;">
                            <div class="team-avatar sm" style="background:<%# Eval("Team1Color") %>;"><%# Eval("Team1Short") %></div>
                            <span style="flex:1;font-size:12px;"><%# Eval("Team1Name") %></span>
                            <strong style="margin:0 6px;"><%# Eval("ScoreTeam1") %> : <%# Eval("ScoreTeam2") %></strong>
                            <span style="flex:1;text-align:right;font-size:12px;"><%# Eval("Team2Name") %></span>
                            <div class="team-avatar sm" style="background:<%# Eval("Team2Color") %>;"><%# Eval("Team2Short") %></div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>

<div class="es-alert info mt-3" style="font-size:12px;">
    <i class="bi bi-info-circle"></i> <strong>Quy tắc tính điểm:</strong>
    Thắng = 3đ · Hòa = 1đ · Thua = 0đ. Đồng điểm xét theo hiệu số bàn thắng/thua.
</div>
</asp:Content>
