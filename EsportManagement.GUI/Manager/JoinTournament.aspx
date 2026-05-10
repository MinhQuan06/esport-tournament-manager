<%@ Page Title="Đăng ký giải đấu" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="JoinTournament.aspx.cs" Inherits="Manager_JoinTournament" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Đăng ký đội tham gia giải đấu</h1>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<div class="es-card">
    <div class="es-alert info" style="font-size:12px;margin-bottom:18px;">
        <i class="bi bi-info-circle"></i> Chỉ có thể đăng ký vào giải đấu đang ở trạng thái "Sắp diễn ra" (BR-TEAM-02).
    </div>

    <div class="row">
        <div class="col-md-6 mb-3">
            <label class="form-label">Giải đấu *</label>
            <asp:DropDownList ID="ddlTour" runat="server" CssClass="form-select"
                DataValueField="TournamentID" DataTextField="TournamentName" />
        </div>
        <div class="col-md-6 mb-3">
            <label class="form-label">Game</label>
            <asp:DropDownList ID="ddlGame" runat="server" CssClass="form-select">
                <asp:ListItem Value="League of Legends" />
                <asp:ListItem Value="CS:GO" />
                <asp:ListItem Value="Valorant" />
                <asp:ListItem Value="Dota 2" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="row">
        <div class="col-md-8 mb-3">
            <label class="form-label">Tên đội *</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-4 mb-3">
            <label class="form-label">Tên ngắn</label>
            <asp:TextBox ID="txtShort" runat="server" CssClass="form-control" placeholder="T1, GAM..." MaxLength="10" />
        </div>
    </div>
    <div class="mb-3">
        <label class="form-label">Mô tả đội</label>
        <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
    </div>
    <asp:Button ID="btnRegister" runat="server" Text="Đăng ký tham gia"
        CssClass="btn btn-primary" OnClick="btnRegister_Click" />
</div>
</asp:Content>
