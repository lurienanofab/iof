<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Items.aspx.vb" Inherits="IOF.Web.Items" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        [data-active='false'],
        [data-active='false'] a {
            color: #ff0000;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="iof-items">
        <h5>Manage Items</h5>

        <!-- Add Item -->
        <div class="panel panel-default">
            <div class="panel-body">
                <strong class="title">Add Item</strong>

                <asp:HiddenField runat="server" ID="hidItemID" />

                <div class="form-horizontal compact">
                    <asp:PlaceHolder runat="server" ID="phStoreManager" Visible="false">
                        <div class="form-group form-group-sm">
                            <div class="col-md-offset-1 col-md-11">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" runat="server" id="chkStoreManager" class="store-manager" tabindex="9" />
                                        Store Manager
                                    </label>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Vendor</label>
                        <div class="col-md-4">
                            <asp:DropDownList runat="server" ID="ddlVendors" DataTextField="VendorName" DataValueField="VendorID" AutoPostBack="true" CssClass="form-control" TabIndex="1" />
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Part #</label>
                        <div class="col-md-4">
                            <asp:TextBox runat="server" ID="txtPartNum" MaxLength="50" CssClass="form-control" TabIndex="2" />
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Description *</label>
                        <div class="col-md-4">
                            <asp:TextBox runat="server" ID="txtDescription" MaxLength="500" CssClass="form-control" TabIndex="3" />
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Unit Price *</label>
                        <div class="col-md-2">
                            <div class="input-group">
                                <span class="input-group-addon">$</span>
                                <asp:TextBox runat="server" ID="txtUnitPrice" MaxLength="50" CssClass="form-control" TabIndex="4" />
                            </div>
                        </div>
                    </div>

                    <asp:PlaceHolder runat="server" ID="phInventoryItem" Visible="false">
                        <div class="form-group form-group-sm">
                            <label class="col-md-1 control-label">Store Item</label>
                            <div class="col-md-4">
                                <asp:DropDownList runat="server" ID="ddlInventoryItem" DataTextField="Description" DataValueField="InventoryItemID" CssClass="form-control" TabIndex="5" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-1 col-md-4">
                            <asp:Button runat="server" ID="btnAddItem" Text="Add Item" CssClass="btn btn-default" TabIndex="6" />
                            <asp:Button runat="server" ID="btnUpdateItem" Text="Modify Item" Visible="false" CssClass="btn btn-default" TabIndex="7" />
                            <asp:Button runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" CssClass="btn btn-default" TabIndex="8" />
                            <uc:BootstrapAlert runat="server" ID="Alert1" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <h4>
            <span>Vendor:</span>
            <asp:Literal runat="server" ID="litVendorName"></asp:Literal>
        </h4>

        <div class="checkbox" style="margin-bottom: 10px;">
            <label>
                <input type="checkbox" runat="server" id="chkShowInactive" class="show-inactive" />
                Show Deleted Items
            </label>
        </div>

        <!-- Items List -->
        <table class="iofgrid datatable table table-striped">
            <thead>
                <tr>
                    <th style="width: 150px;">Part #</th>
                    <th>Description</th>
                    <th style="width: 80px;">Unit Price</th>
                    <th style="width: 60px;">&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="rptItems">
                    <ItemTemplate>
                        <tr data-active='<%#If(Convert.ToBoolean(Eval("Active")), "true", "false")%>'>
                            <td>
                                <%#Eval("PartNum")%>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtnModifyItem" CommandArgument='<%#Eval("ItemID") %>' CommandName="modify" OnCommand="Row_Command"><%#Eval("Description")%></asp:LinkButton>
                            </td>
                            <td class="text-right">
                                <%#Eval("UnitPrice", "{0:C}") %>
                            </td>
                            <td class="text-center">
                                <asp:LinkButton runat="server" ID="lbtnDeleteRestoreItem" CommandArgument='<%#Eval("ItemID") %>' CommandName='<%#If(Convert.ToBoolean(Eval("Active")), "delete", "restore")%>' OnCommand="Row_Command"><%#If(Convert.ToBoolean(Eval("Active")), "Delete", "Restore")%></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <asp:LinkButton ID="lbtnItemPopup" runat="server" />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        $.fn.dataTable.ext.search.push(
            function (settings, searchData, index, rowData, counter) {
                var api = new $.fn.dataTable.Api(settings);

                var row = $(api.row(index).node());

                if ($(".show-inactive").prop("checked"))
                    return true;
                else
                    return row.data("active")
            }
        );

        var table = $(".datatable").DataTable({
            "stateSave": false,
            "autoWidth": false,
            "order": [[1, "asc"]],
            "columns": [
                null,
                null,
                null,
                { "searchable": false, "orderable": false }
            ],
            "language": {
                "emptyTable": "No IOFs were found."
            }
        });

        $(".show-inactive").on("change", function (e) {
            table.draw();
        });
    </script>
</asp:Content>
