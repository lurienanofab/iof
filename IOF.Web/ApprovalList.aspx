<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="ApprovalList.aspx.vb" Inherits="IOF.Web.ApprovalList" %>

<%@ Register Src="~/Controls/PurchaseOrderHeaderView.ascx" TagPrefix="uc" TagName="PurchaseOrderHeaderView" %>
<%@ Register Src="~/Controls/PurchaseOrderDetailView.ascx" TagPrefix="uc" TagName="PurchaseOrderDetailView" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .approval-buttons {
            white-space: nowrap;
            text-align: center;
        }

            .approval-buttons input[type=image] {
                margin-right: 10px;
            }

                .approval-buttons input[type=image]:last-child {
                    margin-right: 0;
                }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hidCurrentPOID" />

    <h5>Approval List</h5>

    <asp:PlaceHolder runat="server" ID="phApprovalList" Visible="true">
        <div class="panel panel-default">
            <div class="panel-body">

                <asp:PlaceHolder runat="server" ID="phFilterAlert" Visible="false">
                    <div class="alert alert-info" role="alert">
                        Showing IOFs for
                        <asp:Literal runat="server" ID="litFilterAlertName"></asp:Literal>.
                        Click
                        <asp:HyperLink runat="server" ID="hypViewAll" NavigateUrl="~/ApprovalList.aspx">here</asp:HyperLink>
                        to view all.
                    </div>
                </asp:PlaceHolder>

                <uc:BootstrapAlert runat="server" ID="ApproveAlert" />

                <asp:PlaceHolder runat="server" ID="phReject" Visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>You decided to reject this IOF. Please provide a reason:</label>
                                <asp:TextBox ID="txtRejectReason" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="10" />
                            </div>
                            <asp:Button runat="server" ID="btnSendEmail" Text="Send Email to Owner" OnClick="btnSendEmail_Click" CssClass="btn btn-default" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <input type="hidden" runat="server" id="hidIsApprover" class="is-approver" value="false" />

                <div class="form-inline clearfix" style="margin-bottom: 10px;">
                    <div class="form-group pull-right">
                        <label>Search:</label>
                        <input type="text" class="form-control search" />
                    </div>
                </div>

                <asp:PlaceHolder runat="server" ID="phMine" Visible="false">
                    <div style="margin-bottom: 20px;">
                        <strong>IOFs Awaiting Approval
                            <asp:Literal runat="server" ID="litApproverName"></asp:Literal></strong>
                        <table class="iofgrid datatable table table-striped mine">
                            <thead>
                                <tr>
                                    <th>POID</th>
                                    <th>Approver</th>
                                    <th>Created On</th>
                                    <th>Created By</th>
                                    <th>Short Code</th>
                                    <th>PO Total</th>
                                    <th runat="server" id="thApproveRejectHeaderMine" visible="false">&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptApprovalPendingMine" OnItemDataBound="Repeater_ItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:LinkButton runat="server" ID="lbtnView" CommandName="view" CommandArgument='<%#Eval("POID")%>' OnCommand="Row_Command"><%#Eval("POID")%></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%#Eval("ApproverName")%>
                                            </td>
                                            <td>
                                                <%#Eval("CreatedDate", "{0:MM/dd/yyyy hh:mm:ss tt}")%>
                                            </td>
                                            <td>
                                                <%#Eval("DisplayName")%>
                                            </td>
                                            <td>
                                                <%#Eval("ShortCode")%>
                                            </td>
                                            <td class="text-right">
                                                <%#Eval("Total", "{0:C}")%>
                                            </td>
                                            <td runat="server" id="tdApproveRejectItem" visible="false" class="approval-buttons">
                                                <asp:ImageButton runat="server" ID="btnApprove" ImageUrl="/static/images/ok.png" ToolTip="Approve" OnCommand="Row_Command" CommandName="approve" CommandArgument='<%#Eval("POID") %>' />
                                                <asp:ImageButton runat="server" ID="btnReject" ImageUrl="/static/images/cancel.png" ToolTip="Reject" OnCommand="Row_Command" CommandName="reject" CommandArgument='<%#Eval("POID") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </asp:PlaceHolder>

                <div>
                    <strong>IOFs Awaiting Approval
                            <asp:Literal runat="server" ID="litOtherApproverName"></asp:Literal></strong>
                    <table class="iofgrid datatable table table-striped other">
                        <thead>
                            <tr>
                                <th>POID</th>
                                <th>Approver</th>
                                <th>Created On</th>
                                <th>Created By</th>
                                <th>Short Code</th>
                                <th>PO Total</th>
                                <th runat="server" id="thApproveRejectHeaderOther" visible="false">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rptApprovalPendingOther" OnItemDataBound="Repeater_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:LinkButton runat="server" ID="lbtnView" CommandName="view" CommandArgument='<%#Eval("POID")%>' OnCommand="Row_Command"><%#Eval("POID")%></asp:LinkButton>
                                        </td>
                                        <td>
                                            <%#Eval("ApproverName")%>
                                        </td>
                                        <td>
                                            <%#Eval("CreatedDate", "{0:MM/dd/yyyy hh:mm:ss tt}")%>
                                        </td>
                                        <td>
                                            <%#Eval("DisplayName")%>
                                        </td>
                                        <td>
                                            <%#Eval("ShortCode")%>
                                        </td>
                                        <td class="text-right">
                                            <%#Eval("Total", "{0:C}")%>
                                        </td>
                                        <td runat="server" id="tdApproveRejectItem" visible="false" class="approval-buttons">
                                            <asp:ImageButton runat="server" ID="btnApprove" ImageUrl="/static/images/ok.png" ToolTip="Approve" OnCommand="Row_Command" CommandName="approve" CommandArgument='<%#Eval("POID") %>' />
                                            <asp:ImageButton runat="server" ID="btnReject" ImageUrl="/static/images/cancel.png" ToolTip="Reject" OnCommand="Row_Command" CommandName="reject" CommandArgument='<%#Eval("POID") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:Panel runat="server" ID="panDetail" Visible="false">
        <uc:PurchaseOrderHeaderView runat="server" ID="PurchaseOrderHeaderView1" />
        <uc:PurchaseOrderDetailView runat="server" ID="PurchaseOrderDetailView1" />
        <asp:HyperLink runat="server" ID="hypPrintIOF" CssClass="btn btn-default print-iof" Target="_blank">Print</asp:HyperLink>
        <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        var isApprover = $(".is-approver").val() === "true";

        var getTableOptions = function (displayLength, emptyTable) {

            var columns = [];

            columns.push({ "width": "70px" });
            columns.push(null);
            columns.push({ "width": "180px" });
            columns.push({ "width": "170px" });
            columns.push({ "width": "110px" });
            columns.push({ "width": "90px" });

            if (isApprover)
                columns.push({ "width": "60px", "orderable": false, "searchable": false });

            var result = {
                "dom": "<'row'<'col-sm-12'tr>><'row nodata-hidden'<'col-sm-5'i><'col-sm-7'p>>",
                "autoWidth": false,
                "stateSave": true,
                "displayLength": displayLength,
                "order": [[1, "asc"]],
                "columns": columns,
                "language": {
                    "emptyTable": emptyTable
                },
                "drawCallback": function (settings) {
                    var nodataHidden = $(this).closest('.dataTables_wrapper').find('.nodata-hidden');
                    nodataHidden.toggle(this.api().page.info().pages > 1);
                }
            };

            return result;
        };

        var tableMine = $(".datatable.mine").DataTable(getTableOptions(-1, "There are no IOFs awaiting your approval."));
        var tableOther = $(".datatable.other").DataTable(getTableOptions(10, "No IOFs are awaiting approval by other managers."));

        $(".search").on("keyup", function (e) {
            var val = $(this).val();
            tableMine.search(val).draw();
            tableOther.search(val).draw();
        });
    </script>
</asp:Content>
