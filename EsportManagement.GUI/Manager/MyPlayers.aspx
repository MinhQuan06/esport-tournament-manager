<%@ Page Title="Người chơi của đội" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="MyPlayers.aspx.cs" Inherits="Manager_MyPlayers" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header">
    <h1 class="page-title">Người chơi - <asp:Literal ID="litTeam" runat="server" /></h1>
    <div class="page-actions">
        <span style="color:var(--text-muted);font-size:13px;">Sĩ số: <strong style="color:var(--accent-cyan);"><asp:Literal ID="litCount" runat="server" />/10</strong></span>
        <asp:Button ID="btnAdd" runat="server" Text="+ Thêm người chơi"
            CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" Visible="false" />

<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
    CssClass="es-table" GridLines="None" DataKeyNames="PlayerID"
    OnRowCommand="gv_RowCommand"
    EmptyDataText="<div style='padding:30px;text-align:center;color:var(--text-muted);'>Chưa có người chơi.</div>">
    <Columns>
        <asp:BoundField DataField="PlayerID"   HeaderText="ID" />
        <asp:BoundField DataField="PlayerName" HeaderText="Họ tên" />
        <asp:BoundField DataField="Nickname"   HeaderText="Nick name" />
        <asp:BoundField DataField="Position"   HeaderText="Vị trí" />
        <asp:BoundField DataField="ContactInfo" HeaderText="Liên hệ" />
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton runat="server" CssClass="btn-icon btn-icon-delete"
                    CommandName="DeleteItem" CommandArgument='<%# Eval("PlayerID") %>'
                    OnClientClick="return confirm('Xóa người chơi?');">
                    <i class="bi bi-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="es-card mt-3">
    <h6 class="es-card-title">Thêm người chơi mới</h6>
    <div class="row">
        <div class="col-md-4 mb-3">
            <label class="form-label">Họ tên *</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-4 mb-3">
            <label class="form-label">Nickname</label>
            <asp:TextBox ID="txtNick" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-4 mb-3">
            <label class="form-label">Vị trí</label>
            <asp:DropDownList ID="ddlPos" runat="server" CssClass="form-select">
                <asp:ListItem Value="" Text="-- chọn --" />
                <asp:ListItem Value="Top" /><asp:ListItem Value="Jungle" />
                <asp:ListItem Value="Mid" /><asp:ListItem Value="ADC" />
                <asp:ListItem Value="Support" /><asp:ListItem Value="Duelist" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="mb-3">
        <label class="form-label">Liên hệ</label>
        <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" />
    </div>
    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn btn-primary" OnClick="btnSave_Click" />
</asp:Panel>
</asp:Content>
