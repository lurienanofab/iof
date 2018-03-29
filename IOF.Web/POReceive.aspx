<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="POReceive.aspx.vb" Inherits="IOF.Web.POReceive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="scripts/jquery.receiving/jquery.receiving.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>IOF Receiving</h5>
    <asp:Repeater runat="server" ID="rptReceive">
        <ItemTemplate>
            <div class="po-receive" data-poid='<%#Eval("POID")%>'>
                <div class="panel po-header"></div>
                <div class="panel po-items"></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" src="scripts/jquery.receiving/jquery.receiving.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".po-receive").receiving();
        });
    </script>
</asp:Content>
