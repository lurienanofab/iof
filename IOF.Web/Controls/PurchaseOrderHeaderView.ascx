<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurchaseOrderHeaderView.ascx.vb" Inherits="IOF.Web.Controls.PurchaseOrderHeaderView" %>

<div class="panel panel-default">
    <div class="panel-body">
        <strong class="title">
            <asp:LinkButton runat="server" ID="btnPurchaseOrderInfo" ToolTip="Click to edit" OnCommand="btnPurchaseOrderInfo_Command" Visible="false">Internal Order Information</asp:LinkButton>
            <asp:Label runat="server" ID="lblPurchaseOrderInfo" Visible="true">Internal Order Information</asp:Label>
            <span>POID:</span>
            <asp:Literal runat="server" ID="litPOID"></asp:Literal>
            <asp:Label runat="server" ID="lblStatusID" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblCopyIOF" ForeColor="#ff0000" Font-Bold="false" Visible="false"></asp:Label>
        </strong>

        <div class="form-horizontal compact">
            <asp:PlaceHolder runat="server" ID="phRealPO" Visible="false">
                <div class="form-group form-group-sm">
                    <label class="col-md-2 control-label">Real PO</label>
                    <div class="col-md-2">
                        <p class="form-control-static">
                            <asp:Literal runat="server" ID="litRealPO"></asp:Literal>
                        </p>
                    </div>
                </div>
            </asp:PlaceHolder>

            <div class="form-group form-group-sm">
                <label class="col-md-2 control-label">Vendor</label>
                <div class="col-md-2">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litVendorName"></asp:Literal>
                    </p>
                </div>
                <label class="col-lg-1 col-md-2 control-label">Account</label>
                <div class="col-md-3">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litAccount"></asp:Literal>
                    </p>
                </div>
            </div>

            <div class="form-group form-group-sm">
                <label class="col-md-2 control-label">Approved By</label>
                <div class="col-md-2">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litApprovedBy"></asp:Literal>
                    </p>
                </div>
                <label class="col-lg-1 col-md-2 control-label">Date Needed</label>
                <div class="col-md-3">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litNeededDate"></asp:Literal>
                    </p>
                </div>
            </div>

            <div class="form-group form-group-sm">
                <label class="col-md-2 control-label">Oversized</label>
                <div class="col-md-2">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litOversized"></asp:Literal>
                    </p>
                </div>
                <label class="col-lg-1 col-md-2 control-label">Shipping</label>
                <div class="col-md-3">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litShippingMethod"></asp:Literal>
                    </p>
                </div>
            </div>

            <div class="form-group form-group-sm">
                <label class="col-md-2 control-label">Is Inventory Controlled</label>
                <div class="col-md-2">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litIsInventoryControlled">No</asp:Literal>
                    </p>
                </div>
                <label class="col-lg-1 col-md-2 control-label">Created By</label>
                <div class="col-md-6">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litOrderedBy"></asp:Literal>
                    </p>
                </div>
            </div>

            <div class="form-group form-group-sm">
                <label class="col-md-2 control-label">Notes</label>
                <div class="col-md-10">
                    <p class="form-control-static">
                        <asp:Literal runat="server" ID="litNotes"></asp:Literal>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>
