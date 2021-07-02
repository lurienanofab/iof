<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="index.aspx.vb" Inherits="IOF.Web.Reports.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .report .datatable > tbody > tr.selected {
            background-color: #ffff99;
        }

        .report .loading {
            height: 50px;
            overflow: hidden;
            visibility: hidden;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>
        <asp:Literal runat="server" ID="litReportTitle"></asp:Literal>
    </h5>

    <asp:PlaceHolder runat="server" ID="phItemReport" Visible="false">
        <div class="report" data-report-type="item-report">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="form-horizontal compact">
                        <div class="form-group form-group-sm">
                            <label class="col-lg-1 col-md-2 control-label">Client</label>
                            <div class="col-md-3">
                                <asp:DropDownList runat="server" ID="ddlClients" DataTextField="DisplayName" DataValueField="ClientID" AutoPostBack="true" OnSelectedIndexChanged="ddlClients_SelectedIndexChanged" CssClass="form-control" />
                            </div>
                        </div>
                    </div>

                    <div style="margin-top: 20px;">
                        <div style="margin-bottom: 10px;">
                            <em class="text-muted">Click an item description link below to view the report.</em>
                        </div>

                        <div class="item-search loading">
                            <table class="iofgrid datatable table table-striped">
                                <thead>
                                    <tr>
                                        <th style="width: 60px;">Item #</th>
                                        <th style="width: 160px;">Part #</th>
                                        <th style="width: 180px;">Vendor</th>
                                        <th>Description</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="rptItems">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%#Eval("ItemID")%></td>
                                                <td><%#Eval("PartNum")%></td>
                                                <td><%#Eval("VendorName")%></td>
                                                <td>
                                                    <asp:LinkButton runat="server" ID="lbtnItem" OnCommand="Item_Command" CommandName="item-report" CommandArgument='<%#Eval("ItemID")%>'><%#Eval("Description")%></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <input type="hidden" runat="server" id="hidSelectedItemID" value="" class="selected-item-id" />

            <asp:PlaceHolder runat="server" ID="phItemInfo" Visible="false">
                <h3>
                    <asp:Literal runat="server" ID="litItemDescription"></asp:Literal></h3>
                <div class="form-horizontal compact">
                    <div class="form-group form-group-lg">
                        <label class="col-lg-1 col-md-2 control-label" style="padding: 0;">Vendor</label>
                        <div class="col-lg-11 col-md-12">
                            <p class="form-control-static" style="padding: 0; height: auto; min-height: inherit;">
                                <asp:Literal runat="server" ID="litItemVendorName"></asp:Literal>
                            </p>
                        </div>
                    </div>
                    <div class="form-group form-group-lg">
                        <label class="col-lg-1 col-md-2 control-label" style="padding: 0;">Part #</label>
                        <div class="col-lg-11 col-md-12">
                            <p class="form-control-static" style="padding: 0; height: auto; min-height: inherit;">
                                <asp:Literal runat="server" ID="litItemPartNum"></asp:Literal>
                            </p>
                        </div>
                    </div>
                </div>

                <div class="row" style="margin-top: 10px;">
                    <div class="col-md-5">
                        <table class="iofgrid table table-striped">
                            <thead>
                                <tr>
                                    <th>Year</th>
                                    <th>Month</th>
                                    <th>Total Units</th>
                                    <th>Total Cost</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptItemReport">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%#Eval("Year")%>
                                            </td>
                                            <td>
                                                <%#Eval("MonthName")%>
                                            </td>
                                            <td class="text-right">
                                                <%#Eval("TotalUnit")%>
                                            </td>
                                            <td class="text-right">
                                                <%#Eval("TotalCost", "{0:C}")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr runat="server" id="trNoData" visible="false">
                                    <td colspan="4" class="text-center">
                                        <em class="text-muted">No data was found.</em>
                                    </td>
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr style="font-weight: bold;">
                                    <td>&nbsp;</td>
                                    <td class="text-right">Total:</td>
                                    <td class="text-right">
                                        <asp:Literal runat="server" ID="litTotalUnit"></asp:Literal>
                                    </td>
                                    <td class="text-right">
                                        <asp:Literal runat="server" ID="litTotalCost"></asp:Literal>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>

                        <asp:Panel runat="server" ID="panItemReportExcelExport" Visible="false">
                            <div style="padding-top: 5px;">
                                <span>Export:</span>
                                <asp:LinkButton runat="server" ID="btnItemReportExportCsv" Text="CSV" OnCommand="Export_Command" CommandName="export-csv"></asp:LinkButton>
                                <span>|</span>
                                <asp:LinkButton runat="server" ID="btnItemReportExportExcel" Text="Excel" OnCommand="Export_Command" CommandName="export-excel"></asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phStoreManagerReport" Visible="false">
        <div class="panel panel-default report" data-report-type="store-manager-report">
            <div class="panel-body">
                <img class="loader" src="//ssel-apps.eecs.umich.edu/static/images/ajax-loader.gif" alt="Loading..." />

                <div class="row">
                    <div class="col-md-9">
                        <div class="table-container loading">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" class="price-filter" />
                                    Only show items with IOF unit price greater than store package price
                                </label>
                            </div>
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" class="purchased-filter" />
                                    Only show items that have been purchased in the store
                                </label>
                            </div>
                            <table class="iofgrid datatable table table-striped">
                                <thead>
                                    <tr>
                                        <th>Last Ordered</th>
                                        <th>Unit</th>
                                        <th>Unit Price</th>
                                        <th>Pkg Price</th>
                                        <th>Pkg Qty</th>
                                        <th>Unit Price</th>
                                        <th>Last Purchased</th>
                                        <th>search-item-id</th>
                                        <th>search-store-item-id</th>
                                        <th>search-vendor</th>
                                        <th>search-iof-description</th>
                                        <th>search-store-description</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>

                            <div style="padding-top: 5px;">
                                <span>Export:</span>
                                <asp:LinkButton runat="server" ID="btnStoreManagerReportExportCsv" Text="CSV" OnCommand="Export_Command" CommandName="export-csv"></asp:LinkButton>
                                <span>|</span>
                                <asp:LinkButton runat="server" ID="btnStoreManagerReportExportExcel" Text="Excel" OnCommand="Export_Command" CommandName="export-excel"></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="scripts">
    <script src="<%=ResolveUrl("~/scripts/report.js")%>"></script>
    <script>
        var rep = $(".report").report();
    </script>
</asp:Content>
