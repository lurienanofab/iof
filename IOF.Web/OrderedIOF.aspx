<%@ Page Title="Orders" Language="vb" MaintainScrollPositionOnPostback="false" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="OrderedIOF.aspx.vb" Inherits="IOF.Web.OrderedIOF" %>

<%@ Import Namespace="LNF.Ordering" %>

<%@ Register Src="~/Controls/PurchaseOrderAttachments.ascx" TagPrefix="uc" TagName="PurchaseOrderAttachments" %>
<%@ Register Src="~/Controls/PurchaseOrderHeaderView.ascx" TagPrefix="uc" TagName="PurchaseOrderHeaderView" %>
<%@ Register Src="~/Controls/PurchaseOrderDetailView.ascx" TagPrefix="uc" TagName="PurchaseOrderDetailView" %>
<%@ Register Src="~/Controls/PurchasingSearch.ascx" TagPrefix="uc" TagName="PurchasingSearch" %>
<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/purchasing.css" rel="stylesheet" />
    <link href="styles/attachments.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hidPOID" Value="" />

    <h5>Orders</h5>

    <asp:PlaceHolder runat="server" ID="phPrivMessage">
        <div style="padding-bottom: 5px;">
            <em>Only authorized purchasers can modify ordered IOFs.</em>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phSearch" Visible="true">
        <uc:PurchasingSearch runat="server" ID="PurchasingSearch1"
            OnSearch="PurchasingSearch1_Search"
            Title="IOF Orders Search"
            DateInputMode="Range"
            OrderStatus="Ordered"
            ClaimStatus="All"
            StatusIdList="4" />

        <em class="text-muted" style="margin-bottom: 10px; display: block;">The default sort is by "Created On" date in descending order.</em>

        <table class="iofgrid datatable table table-striped purchasing-list">
            <thead>
                <tr>
                    <th>POID</th>
                    <th>Created On</th>
                    <th>Created By</th>
                    <th>Claimed By</th>
                    <th>Real PO</th>
                    <th>PO Total</th>
                    <th>Print</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="rptOrdered">
                    <ItemTemplate>
                        <tr>
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
                            <td>
                                <%#Eval("RealPO")%>
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
                    <asp:Label runat="server" ID="lblClaimedBy"></asp:Label>
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
            <asp:HyperLink runat="server" ID="hypPrintIOF" CssClass="btn btn-default print-iof" Target="_blank">Print</asp:HyperLink>
            <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="scripts/detail.js"></script>
    <script>
        $(".purchasing-list").DataTable({
            "pageLength": 10,
            "autoWidth": false,
            "stateSave": true,
            "order": [[1, "desc"]],
            "columns": [
                { "width": "60px" },
                { "width": "150px" },
                null,
                null,
                { "width": "120px" },
                { "width": "80px" },
                { "width": "60px", "orderable": false, "searchable": false }
            ],
            "language": {
                "emptyTable": "No IOFs were found."
            }
        });

        $(".detail").detail();
    </script>
</asp:Content>
