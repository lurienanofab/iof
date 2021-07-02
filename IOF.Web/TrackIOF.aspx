<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="TrackIOF.aspx.vb" Inherits="IOF.Web.TrackIOF" %>

<%@ Register Src="~/Controls/PurchaseOrderSearch.ascx" TagPrefix="uc" TagName="PurchaseOrderSearch" %>
<%@ Register Src="~/Controls/Tracking.ascx" TagPrefix="uc" TagName="Tracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:PlaceHolder runat="server" ID="phSearch" Visible="true">
        <uc:PurchaseOrderSearch runat="server" ID="PurchaseOrderSearch1" Title="IOF Tracking" EnableCopy="false" DisplayOption="Summary" />
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phDetail" Visible="false">
        <h5>IOF Tracking</h5>

        <uc:Tracking runat="server" ID="Tracking1" />

        <div class="btn-spacing">
            <asp:Button runat="server" ID="btnView" Text="View" OnClick="btnView_Click" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
