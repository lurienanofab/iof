<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="POConfirm.aspx.vb" Inherits="IOF.Web.POConfirm" %>

<%@ Register Src="~/Controls/PurchaseOrderHeaderView.ascx" TagPrefix="uc" TagName="PurchaseOrderHeaderView" %>
<%@ Register Src="~/Controls/PurchaseOrderDetailView.ascx" TagPrefix="uc" TagName="PurchaseOrderDetailView" %>
<%@ Register Src="~/Controls/PurchaseOrderAttachments.ascx" TagPrefix="uc" TagName="PurchaseOrderAttachments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:PlaceHolder runat="server" ID="phConfirm">
        <h5>Internal Order Summary</h5>

        <!-- Purchase Order Information -->
        <uc:PurchaseOrderHeaderView runat="server" ID="PurchaseOrderHeaderView1" OnCommand="PurchaseOrderHeader1_Command" />

        <!-- Purchase Order Items -->
        <uc:PurchaseOrderDetailView runat="server" ID="PurchaseOrderDetailView1" OnTitleCommand="PurchaseOrderDetail1_TitleCommand" />

        <!-- Purchase Order Attachments -->
        <uc:PurchaseOrderAttachments runat="server" ID="PurchaseOrderAttachments1" ReadOnly="false" />

        <div class="btn-spacing">
            <asp:HyperLink runat="server" ID="hypPrintIOF" CssClass="btn btn-default print-iof" Target="_blank">Print IOF</asp:HyperLink>
            <asp:Button runat="server" ID="btnSendToApprover" Text="Send for Approval" Visible="false" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnSendToPurchaser" Text="Send for Purchaser" Visible="false" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnMarkCompleteAndPrint" Text="Mark Complete and Print" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnKeepDraft" Text="Keep as Draft" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnDelete" Text="Delete" CssClass="btn btn-default" OnClientClick="return confirm('Are you sure you want to delete this purchase order permanently?');" />
            <asp:HyperLink runat="server" ID="hypReturn" CssClass="btn btn-default">Return</asp:HyperLink>
        </div>

        <asp:PlaceHolder runat="server" ID="phPOMessage" Visible="false">
            <div style="margin-top: 10px; color: #ff0000;">
                <asp:Literal runat="server" ID="litPOMessage" />
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>

    <asp:Panel runat="server" ID="panSetAccount" Visible="false">
        <asp:HiddenField runat="server" ID="hidSetAccountPOID" />
        <h5>Start From Existing IOF</h5>
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <div style="margin-bottom: 10px;">
                            <span>The IOF you selected uses shortcode</span>
                            <asp:Literal runat="server" ID="litShortCode"></asp:Literal>
                            <span>, however you are not currently authorized to use this account. Please select a different account or contact Sandrine Martin (<a href="mailto:sandrine@umich.edu">sandrine@umich.edu</a>) to request access.</span>
                        </div>
                        <div class="form-group">
                            <label for="exampleInputEmail1">Account</label>
                            <asp:DropDownList runat="server" ID="ddlSetAccount" DataTextField="AccountName" DataValueField="AccountID" CssClass="form-control" />
                        </div>
                        <div style="margin-bottom: 10px;">
                            <asp:Button runat="server" ID="btnSetAccountOK" Text="OK" Width="65" OnClick="btnSetAccountOK_Click" CssClass="btn btn-default" />
                            <button type="button" class="btn btn-default back-button">Cancel</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
