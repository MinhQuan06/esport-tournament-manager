<%@ Page Title="Quản lý trận đấu" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Matches.aspx.cs" Inherits="Admin_Matches" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Quản lý trận đấu</h1>
    <div class="page-actions">
        <asp:DropDownList ID="ddlTour" runat="server" CssClass="form-select"
            AutoPostBack="true" OnSelectedIndexChanged="ddlTour_Changed"
            DataValueField="TournamentID" DataTextField="TournamentName"
            AppendDataBoundItems="true" Style="width:240px;display:inline-block;">
            <asp:ListItem Value="0" Text="Chọn giải đấu" />
        </asp:DropDownList>
        <asp:Button ID="btnAdd" runat="server" Text="+ Thêm trận đấu"
            CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="es-card mb-3" style="padding:0;">
    <div style="display:flex;border-bottom:1px solid var(--border);">
        <asp:LinkButton ID="tabAll" runat="server" CssClass="match-tab active"
            OnClick="Tab_Click" CommandArgument="all">Tất cả
            (<asp:Literal ID="litAll" runat="server" />)</asp:LinkButton>
        <asp:LinkButton ID="tabUpcoming" runat="server" CssClass="match-tab"
            OnClick="Tab_Click" CommandArgument="upcoming">Chưa diễn ra
            (<asp:Literal ID="litUpcoming" runat="server" />)</asp:LinkButton>
        <asp:LinkButton ID="tabOngoing" runat="server" CssClass="match-tab"
            OnClick="Tab_Click" CommandArgument="ongoing">Đang diễn ra
            (<asp:Literal ID="litOngoing" runat="server" />)</asp:LinkButton>
        <asp:LinkButton ID="tabDone" runat="server" CssClass="match-tab"
            OnClick="Tab_Click" CommandArgument="done">Đã kết thúc
            (<asp:Literal ID="litDone" runat="server" />)</asp:LinkButton>
    </div>
</div>

<style>
.match-tab {
    padding: 14px 22px; color: var(--text-muted); text-decoration:none;
    font-weight: 600; border-bottom: 2px solid transparent;
    background: transparent; border-radius: 0;
}
.match-tab:hover { color: var(--text-main); }
.match-tab.active { color: var(--accent-cyan); border-bottom-color: var(--accent-cyan); }
</style>

<asp:Repeater ID="rpt" runat="server" OnItemCommand="rpt_ItemCommand">
    <ItemTemplate>
        <div class="match-card <%# GetCardClass(Container.DataItem) %>">
            <div class="match-status">
                <span class="es-badge <%# GetStatusClass(Container.DataItem) %>"><%# GetStatusText(Container.DataItem) %></span>
                <div class="time" style="margin-top:4px;"><%# ((DateTime)Eval("MatchTime")).ToString("dd/MM · HH:mm") %></div>
                <div class="round"><%# Eval("RoundName") %><%# Eval("GroupName") == null || Eval("GroupName").ToString() == "" ? "" : " · Bảng " + Eval("GroupName") %></div>
            </div>
            <div class="match-team">
                <div class="team-avatar" style="background:<%# Eval("Team1Color") %>;"><%# Eval("Team1Short") %></div>
                <div>
                    <div class="name"><%# Eval("Team1Name") %></div>
                    <div class="group"><%# Eval("MatchFormat") %></div>
                </div>
            </div>
            <div class="match-score <%# Eval("ScoreTeam1") == null ? "pending" : "" %>">
                <%# Eval("ScoreTeam1") == null
                    ? "<span class='vs'>VS</span>"
                    : Eval("ScoreTeam1") + " : " + Eval("ScoreTeam2") %>
            </div>
            <div class="match-team right">
                <div class="team-avatar" style="background:<%# Eval("Team2Color") %>;"><%# Eval("Team2Short") %></div>
                <div style="text-align:right;">
                    <div class="name"><%# Eval("Team2Name") %></div>
                    <div class="group"><%# Eval("MatchFormat") %></div>
                </div>
            </div>
            <div class="match-actions">
                <a class="btn <%# Eval("ScoreTeam1") == null ? "btn-primary" : "btn-success" %> btn-sm"
                   href='<%# ResolveUrl("~/Admin/Results.aspx?match=" + Eval("MatchID")) %>'>
                    <%# Eval("ScoreTeam1") == null ? "Nhập KQ" : "Xem KQ" %>
                </a>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-edit"
                    CommandName="EditItem" CommandArgument='<%# Eval("MatchID") %>'><i class="bi bi-pencil"></i></asp:LinkButton>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-delete"
                    CommandName="DeleteItem" CommandArgument='<%# Eval("MatchID") %>'
                    OnClientClick="return confirm('Xóa trận này?');"><i class="bi bi-trash"></i></asp:LinkButton>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="es-card" style="text-align:center;padding:40px;color:var(--text-muted);">
        <i class="bi bi-calendar-x" style="font-size:48px;"></i>
        <p style="margin-top:14px;">Chưa có trận đấu nào trong giải này.</p>
    </div>
</asp:Panel>

<div class="modal fade" id="mdl" tabindex="-1">
  <div class="modal-dialog"><div class="modal-content">
    <div class="modal-header">
        <h5 class="modal-title"><asp:Literal ID="litTitle" runat="server" Text="Thêm trận đấu" /></h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
        <asp:HiddenField ID="hfId" runat="server" />
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Đội 1 *</label>
                <asp:DropDownList ID="ddlTeam1" runat="server" CssClass="form-select"
                    DataValueField="TeamID" DataTextField="TeamName" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Đội 2 *</label>
                <asp:DropDownList ID="ddlTeam2" runat="server" CssClass="form-select"
                    DataValueField="TeamID" DataTextField="TeamName" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Vòng</label>
                <asp:DropDownList ID="ddlRound" runat="server" CssClass="form-select">
                    <asp:ListItem Value="Vòng bảng" />
                    <asp:ListItem Value="Tứ kết" />
                    <asp:ListItem Value="Bán kết" />
                    <asp:ListItem Value="Chung kết" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Bảng</label>
                <asp:DropDownList ID="ddlGroup" runat="server" CssClass="form-select">
                    <asp:ListItem Value="" Text="-" />
                    <asp:ListItem Value="A" /><asp:ListItem Value="B" />
                    <asp:ListItem Value="C" /><asp:ListItem Value="D" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Format</label>
                <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-select">
                    <asp:ListItem Value="BO1" /><asp:ListItem Value="BO3" />
                    <asp:ListItem Value="BO5" /><asp:ListItem Value="BO7" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-md-8 mb-3">
                <label class="form-label">Thời gian thi đấu *</label>
                <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Trạng thái</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Value="Chưa diễn ra" />
                    <asp:ListItem Value="Đang diễn ra" />
                    <asp:ListItem Value="Đã kết thúc" />
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <asp:Button ID="btnSave" runat="server" Text="Lưu trận đấu" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    </div>
  </div></div>
</div>
<% if (ShowModal) { %>
<script>document.addEventListener('DOMContentLoaded',function(){new bootstrap.Modal(document.getElementById('mdl')).show();});</script>
<% } %>
</asp:Content>
