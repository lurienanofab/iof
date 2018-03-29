<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="SearchIOF.aspx.vb" Inherits="IOF.Web.SearchIOF" %>

<%@ Register Src="~/Controls/PurchaseOrderSearch.ascx" TagName="PurchaseOrderSearch" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<link rel="stylesheet" href="scripts/iofsearch/iofsearch.css?v=20171017" />--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc:PurchaseOrderSearch runat="server" ID="PurchaseOrderSearch1" StatusIdList="3,4,5" />
    <asp:Literal runat="server" ID="litDebug"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
