<%@ Page Title="Temp Files" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="index.aspx.vb" Inherits="IOF.Web.Docs.Index" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .bootstrap-alert {
            margin-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>File List</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <asp:LinkButton runat="server" ID="btnDeleteAll" OnClick="btnDeleteAll_Click" CssClass="btn btn-default">Delete All</asp:LinkButton>
            <uc:BootstrapAlert runat="server" ID="Alert1" />
            <asp:PlaceHolder runat="server" ID="phNoData" Visible="false">
                <em class="text-muted">No files were found.</em>
            </asp:PlaceHolder>
        </div>

        <asp:Repeater runat="server" ID="rptFiles">
            <HeaderTemplate>
                <div class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <a href='<%#Eval("FileUrl")%>' class="list-group-item">
                    <%#Eval("FileName")%>
                </a>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
