<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="PurchaseIOF.aspx.vb" Inherits="IOF.Web.PurchaseIOF" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="scripts/jquery.purchaser/jquery.purchaser.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>IOF Purchasing</h5>
    <asp:Repeater runat="server" ID="rptPurchaseIOF">
        <ItemTemplate>
            <div class="purchaser" data-poid='<%#Eval("POID")%>'>
                <div class="loader">Loading...</div>
                <div class="panel po-claim" style="display: none;">
                    <strong>Claim Purchase Order</strong>
                    <div style="padding-bottom: 5px; margin-bottom: 5px; border-bottom: solid 1px #DDD; color: #002244;">
                        Once an order is claimed you will be able to modify line items and vendor info, or cancel the order.
                    </div>
                    <table>
                        <tr>
                            <td>Claimed by Purchaser:</td>
                            <td>
                                <span class="claimed-by" style="display: none;"></span>
                                <input type="button" value="Claim" class="claim-button" />
                                <div class="claim-po-messages"></div>
                            </td>
                        </tr>
                        <tr>
                            <td>Real PO:</td>
                            <td>
                                <input type="text" class="real-po" disabled="disabled" />
                                <span class="real-po-error" style="display: none;"></span>
                            </td>
                        </tr>
                    </table>
                    <input type="button" value="Save Real PO" class="save-realpo-button" />
                    <span class="no-data">Saving the Real PO will complete the order and no additional price updates will be possible.</span>
                    <div class="save-realpo-messages"></div>
                </div>
                <div class="panel po-header" style="display: none;"></div>
                <div class="panel po-items" style="display: none;"></div>
                <div class="panel po-vendor" style="display: none;"></div>
                <div class="panel po-cancel" style="display: none;">
                    <strong>Cancel Purchase Order</strong>
                    <div style="padding-bottom: 5px; margin-bottom: 5px; border-bottom: solid 1px #DDD; color: #002244;">
                        Cancelling a PO will set it's status back to Draft and it will no longer be approved. The PO creator and approver will be notified by email. 
                    </div>
                    <div style="margin-bottom: 2px;">Reason (e.g. incorrect vendor)</div>
                    <div style="margin-bottom: 10px;">
                        <textarea class="cancel-reason" style="width: 500px;" cols="20" rows="2"></textarea>
                    </div>
                    <input type="button" class="po-cancel-button" value="Cancel Purchase Order" />
                    <div class="cancel-po-messages"></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="return" style="margin-bottom: 10px; font-weight: bold; display: none;">
        <asp:HyperLink runat="server" ID="hypReturn" Text="&larr; Return to Purchaser List" NavigateUrl="~/PurchaseList.aspx"></asp:HyperLink>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" src="scripts/jquery.purchaser/jquery.purchaser.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".purchaser").purchaser(function () {
                $(".return").show();
            });
        });
    </script>
</asp:Content>
