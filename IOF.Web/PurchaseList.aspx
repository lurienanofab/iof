<%@ Page Title="Purchaser List" Language="vb" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="PurchaseList.aspx.vb" Inherits="IOF.Web.PurchaseList" %>

<%@ Import Namespace="LNF.Ordering" %>

<%@ Register Src="~/Controls/PurchaseOrderAttachments.ascx" TagPrefix="uc" TagName="PurchaseOrderAttachments" %>
<%@ Register Src="~/Controls/PurchaseOrderHeaderView.ascx" TagPrefix="uc" TagName="PurchaseOrderHeaderView" %>
<%@ Register Src="~/Controls/PurchaseOrderDetailView.ascx" TagPrefix="uc" TagName="PurchaseOrderDetailView" %>
<%@ Register Src="~/Controls/PurchasingSearch.ascx" TagPrefix="uc" TagName="PurchasingSearch" %>
<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="purchasing">
        <asp:HiddenField runat="server" ID="hidPOID" Value="" />

        <h5>IOF Purchasing List</h5>

        <asp:Panel runat="server" ID="phPrivMessage">
            <div style="padding-bottom: 5px;">
                <em>Only authorized purchasers can claim IOFs.</em>
            </div>
        </asp:Panel>

        <asp:PlaceHolder runat="server" ID="phSearch" Visible="true">
            <uc:PurchasingSearch runat="server" ID="PurchasingSearch1"
                OnSearch="PurchasingSearch1_Search"
                Title="Purchasing Search"
                OrderStatus="Unordered"
                ClaimStatus="All"
                PurchaserVisible="false"
                RealPoVisible="false"
                StatusIdList="3" />

            <asp:PlaceHolder runat="server" ID="phClaim" Visible="false">
                <div style="margin-bottom: 10px;">
                    <a class="check-all" href="#" style="display: inline-block; margin-right: 10px;">Check All</a>
                    <asp:Button runat="server" ID="btnClaim" Text="Claim" OnClick="btnClaim_Click" CssClass="btn btn-default btn-sm" />
                </div>
            </asp:PlaceHolder>

            <em class="text-muted" style="margin-bottom: 10px; display: block;">The default sort is by "Created On" date in descending order.</em>

            <table class="iofgrid datatable table table-striped purchasing-list">
                <thead>
                    <tr>
                        <th>Claim</th>
                        <th>POID</th>
                        <th>Created On</th>
                        <th>Created By</th>
                        <th>Claimed By</th>
                        <th>PO Total</th>
                        <th>Print</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="rptUnclaimed">
                        <ItemTemplate>
                            <tr>
                                <td class="text-center claim">
                                    <asp:HiddenField runat="server" ID="hidPOID" Value='<%#Eval("POID")%>' />
                                    <asp:CheckBox runat="server" ID="chkClaim" Visible='<%#Not IsClaimed(Container.DataItem)%>' Enabled='<%#IsPurchaser()%>' />
                                </td>
                                <td>
                                    <asp:LinkButton runat="server" ID="btnView" OnCommand="Row_Command" CommandName="view" CommandArgument='<%#Eval("POID")%>'><%#Eval("POID")%></asp:LinkButton>
                                </td>
                                <td>
                                    <%#Eval("CreatedDate", "{0:MM/dd/yyyy hh:mm:ss tt}")%>
                                </td>
                                <td>
                                    <%#Eval("DisplayName")%>
                                </td>
                                <td>
                                    <%#Eval("PurchaserName")%>
                                </td>
                                <td class="text-right">
                                    <%#Eval("TotalPrice", "{0:C}")%>
                                </td>
                                <td class="text-center">
                                    <asp:HyperLink runat="server" ID="hypRowPrint" ImageUrl="https://ssel-apps.eecs.umich.edu/static/images/print.png" ToolTip="Print" NavigateUrl='<%#Eval("POID", "~/PrintIOF.ashx?POID={0}")%>' Target="_blank"></asp:HyperLink>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="phDetail" Visible="false">
            <div class="panel panel-default">
                <div class="panel-body">
                    <strong class="title">
                        <span>Claimed by Purchaser:</span>
                        <asp:Label runat="server" ID="lblClaimedBy" Visible="false"></asp:Label>
                        <asp:Button runat="server" ID="btnClaimDetail" Visible="true" Text="Claim" OnClick="btnClaimDetail_Click" CssClass="btn btn-default" />
                    </strong>

                    <div class="form-horizontal compact">
                        <div class="form-group form-group-sm">
                            <label class="col-lg-1 col-md-2 control-label">Req #</label>
                            <div class="col-lg-2 col-md-3">
                                <asp:TextBox runat="server" ID="txtReqNum" Enabled="false" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div runat="server" id="divRealPoFormGroup" class="form-group form-group-sm">
                            <label class="col-lg-1 col-md-2 control-label">Real PO</label>
                            <div class="col-lg-2 col-md-3">
                                <asp:TextBox runat="server" ID="txtRealPO" Enabled="false" CssClass="form-control"></asp:TextBox>
                                <asp:Label runat="server" ID="lblRealPOSaveError" Visible="false" CssClass="help-block"></asp:Label>
                            </div>
                            <div class="col-lg-9 col-md-7">
                                <span class="help-block">
                                    <span class="visible-lg-inline visible-md-inline">&larr;</span>
                                    <span>When this is saved the PO will appear on the ledger.</span>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <uc:PurchaseOrderHeaderView runat="server" ID="PurchaseOrderHeaderView1" EnableLink="false" />

            <uc:PurchaseOrderDetailView runat="server" ID="PurchaseOrderDetailView1" EnableTitleLink="false" PurchaserMode="true" />

            <div class="panel panel-default">
                <div class="panel-body">
                    <strong class="title">Purchaser Notes</strong>
                    <asp:TextBox runat="server" ID="txtPurchaserNotes" TextMode="MultiLine" Rows="5" Columns="5" CssClass="form-control purchaser-notes"></asp:TextBox>
                </div>
            </div>

            <uc:PurchaseOrderAttachments runat="server" ID="PurchaseOrderAttachments1" OnUpload="PurchaseOrderAttachments1_Upload" OnDelete="PurchaseOrderAttachments1_Delete" />

            <asp:PlaceHolder runat="server" ID="phCancelOrder" Visible="false">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <asp:Button runat="server" ID="btnCancelOrder" Text="Cancel Order" OnClick="btnCancelOrder_Click" CssClass="btn btn-danger" />
                        <uc:BootstrapAlert runat="server" ID="CancelOrderAlert" Visible="false" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <div class="btn-spacing" style="margin-bottom: 10px;">
                <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" Enabled="false" CssClass="btn btn-default" />
                <asp:HyperLink runat="server" ID="lnkPrintIOF" Target="_blank" CssClass="btn btn-default print-iof">Print</asp:HyperLink>
                <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
                <asp:Button runat="server" ID="btnResendPurchaserEmail" Text="Resend Purchaser Email" OnClick="btnResendPurchaserEmail_Click" CssClass="btn btn-default" />
                <uc:BootstrapAlert runat="server" ID="Alert1" Visible="false" />
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="scripts/detail.js"></script>
    <script>
        $.fn.dataTable.ext.order['dom-checkbox'] = function (settings, col) {
            return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
                return $('input', td).prop('checked') ? '1' : '0';
            });
        };

        $(".purchasing-list").DataTable({
            "pageLength": 10,
            "autoWidth": false,
            "stateSave": true,
            "order": [[2, "desc"]],
            "columns": [
                { "width": "60px", "orderable": false, "searchable": false },
                { "width": "60px" },
                { "width": "150px" },
                null,
                null,
                { "width": "80px" },
                { "width": "60px", "orderable": false, "searchable": false }
            ],
            "language": {
                "emptyTable": "No IOFs were found."
            }
        });

        $(".check-all").on("click", function (e) {
            e.preventDefault();

            $(".purchasing-list .claim input[type='checkbox']").prop("checked", true)
        });

        $(".detail").detail();
    </script>

    <%--<script src="scripts/jquery.podview.js"></script>
    <script type="text/javascript">
        $(".check-all").click(function () {
            $(".chk input[type='checkbox']").prop('checked', true);
        });

        $(".purchase-order-detail-view").podview();
    </script>--%>
</asp:Content>
