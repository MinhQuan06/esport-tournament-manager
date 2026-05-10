<%@ Page Title="Nhập kết quả" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Results.aspx.cs" Inherits="Admin_Results" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Nhập kết quả trận đấu</h1>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="row g-3">
    <div class="col-md-4">
        <div class="es-card">
            <h6 class="es-card-title"><i class="bi bi-funnel"></i> Chọn trận đấu</h6>
            <label class="form-label">Giải đấu</label>
            <asp:DropDownList ID="ddlTour" runat="server" CssClass="form-select mb-3"
                AutoPostBack="true" OnSelectedIndexChanged="ddlTour_Changed"
                DataValueField="TournamentID" DataTextField="TournamentName" />
            <label class="form-label">Trận đấu</label>
            <asp:DropDownList ID="ddlMatch" runat="server" CssClass="form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddlMatch_Changed"
                DataValueField="MatchID" DataTextField="DisplayName" />
        </div>
    </div>

    <div class="col-md-8">
        <asp:Panel ID="pnlEntry" runat="server" Visible="false" CssClass="es-card">
            <h6 class="es-card-title">
                <i class="bi bi-lightning-charge text-warning"></i>
                Nhập kết quả trận đấu
            </h6>
            <div style="font-size:11px;color:var(--text-muted);margin-bottom:14px;">
                <asp:Literal ID="litRound" runat="server" />
            </div>
            <asp:HiddenField ID="hfMatchId" runat="server" />

            <!-- VS DISPLAY -->
            <div style="display:grid;grid-template-columns:1fr 60px 1fr;gap:14px;align-items:center;
                       background:var(--bg-input);padding:18px;border-radius:10px;margin-bottom:18px;">
                <div style="text-align:center;">
                    <div class="team-avatar lg" style="background:<%= Team1Color %>;margin:0 auto;"><%= Team1Short %></div>
                    <div style="font-weight:700;color:var(--text-main);margin-top:8px;"><asp:Literal ID="litTeam1" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Đội 1</div>
                </div>
                <div style="text-align:center;font-weight:800;color:var(--text-muted);font-size:18px;">VS</div>
                <div style="text-align:center;">
                    <div class="team-avatar lg" style="background:<%= Team2Color %>;margin:0 auto;"><%= Team2Short %></div>
                    <div style="font-weight:700;color:var(--text-main);margin-top:8px;"><asp:Literal ID="litTeam2" runat="server" /></div>
                    <div style="font-size:11px;color:var(--text-muted);">Đội 2</div>
                </div>
            </div>

            <div style="background:var(--bg-input);padding:18px;border-radius:10px;margin-bottom:14px;">
                <div style="text-align:center;font-size:11px;letter-spacing:1.5px;color:var(--text-muted);font-weight:700;margin-bottom:14px;">
                    ĐIỂM SỐ TRẬN ĐẤU
                </div>
                <div style="display:grid;grid-template-columns:1fr 40px 1fr;gap:14px;align-items:center;">
                    <div style="text-align:center;">
                        <div style="font-size:10px;color:var(--text-muted);letter-spacing:1px;"><asp:Literal ID="litTeam1B" runat="server" /></div>
                        <asp:TextBox ID="txtScore1" runat="server" CssClass="form-control" TextMode="Number"
                            Style="font-size:36px;font-weight:800;text-align:center;height:72px;" />
                    </div>
                    <div style="text-align:center;font-size:36px;font-weight:800;color:var(--text-muted);">:</div>
                    <div style="text-align:center;">
                        <div style="font-size:10px;color:var(--text-muted);letter-spacing:1px;"><asp:Literal ID="litTeam2B" runat="server" /></div>
                        <asp:TextBox ID="txtScore2" runat="server" CssClass="form-control" TextMode="Number"
                            Style="font-size:36px;font-weight:800;text-align:center;height:72px;" />
                    </div>
                </div>
                <asp:Panel ID="pnlWinner" runat="server" Visible="false"
                    Style="text-align:center;margin-top:14px;padding:10px;background:rgba(16,185,129,0.1);border-radius:8px;color:var(--accent-green);font-weight:600;">
                    <i class="bi bi-trophy-fill"></i> <asp:Literal ID="litWinner" runat="server" />
                </asp:Panel>
            </div>

            <asp:Panel ID="pnlConfirmed" runat="server" Visible="false" CssClass="es-alert warning">
                <i class="bi bi-lock-fill"></i> Kết quả đã được xác nhận chính thức - không thể chỉnh sửa.
            </asp:Panel>

            <div class="es-alert warning" style="font-size:12px;">
                <i class="bi bi-exclamation-triangle"></i>
                Sau khi <strong>Xác nhận chính thức</strong>, kết quả sẽ bị khóa và không thể chỉnh sửa.
                Bảng xếp hạng sẽ được tự động cập nhật.
            </div>

            <div style="display:flex;gap:10px;justify-content:flex-end;">
                <asp:Button ID="btnEnter" runat="server" Text="💾 Lưu tạm"
                    CssClass="btn btn-secondary" OnClick="btnEnter_Click" />
                <asp:Button ID="btnConfirm" runat="server" Text="✓ Xác nhận chính thức"
                    CssClass="btn btn-success" OnClick="btnConfirm_Click"
                    OnClientClick="return confirm('Sau khi xác nhận sẽ KHÔNG THỂ chỉnh sửa. Tiếp tục?');" />
            </div>
        </asp:Panel>
    </div>
</div>
</asp:Content>
