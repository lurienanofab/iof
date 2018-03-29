<%@ Page Title="LNF Ordering" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="index.aspx.vb" Inherits="IOF.Web.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Welcome to IOF v2.1</h5>

    <div>
        You are
        <asp:Label ID="lblUserName" runat="server"></asp:Label>
    </div>

    <div style="margin-top: 20px;">
        <span>You currently have:</span>
        <ul style="margin-top: 2px;">
            <li>
                <asp:Label ID="lblUnfinishedIOF" runat="server" ForeColor="Red" />
                draft IOFs. Click
                <asp:HyperLink runat="server" ID="hypUnfinishedIOF" NavigateUrl="~/UnfinishedIOF.aspx">here</asp:HyperLink>
                to view details.
            </li>
            <li>
                <asp:Label ID="lblApprovalPendingIOF" runat="server" ForeColor="Red" />
                submitted IOFs waiting for approval. Click
                <asp:HyperLink runat="server" ID="hypApprovalList" NavigateUrl="~/ApprovalList.aspx">here</asp:HyperLink>
                to view details.
            </li>
            <li>
                <asp:Label ID="lblApprovedIOF" runat="server" ForeColor="Red" />
                approved IOFs waiting to be ordered. Click
                <asp:HyperLink runat="server" ID="hypPurchaseList" NavigateUrl="~/PurchaseList.aspx">here</asp:HyperLink>
                to view details.
            </li>
            <li>
                <asp:Label ID="lblOrderedIOF" runat="server" ForeColor="Red" />
                ordered IOFs. Click
                <asp:HyperLink runat="server" ID="hypOrderedIOF" NavigateUrl="~/OrderedIOF.aspx">here</asp:HyperLink>
                to view details.
            </li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
