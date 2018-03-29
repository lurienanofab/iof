<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="ApprovalProcess.aspx.vb" Inherits="IOF.Web.ApprovalProcess" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Internal Order Approval</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <strong class="title">
                <span>Approver:</span>
                <asp:Label runat="server" ID="lblApproverName"></asp:Label>
            </strong>

            <div class="row">
                <div class="col-md-7">
                    <uc:BootstrapAlert runat="server" ID="Alert1" />
                    
                    <asp:PlaceHolder runat="server" ID="phReject" Visible="false">
                        <div style="margin-top: 20px; margin-bottom: 10px;">
                            <p>You decided to reject this IOF. Please provide a reason:</p>
                            <asp:TextBox runat="server" ID="txtRejectReason" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                        </div>
                        <asp:Button runat="server" ID="btnSendEmail" Text="Send Email to Owner" CssClass="btn btn-default" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <asp:HyperLink runat="server" ID="hypHome" NavigateUrl="~">Return to Home</asp:HyperLink>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
