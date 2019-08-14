<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="POItems.aspx.vb" Inherits="IOF.Web.POItems" %>

<%@ Register Src="~/Controls/PurchaseOrderDetailView.ascx" TagName="PurchaseOrderDetailView" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .modal-dialog {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Internal Order Items</h5>

    <!-- Add Item -->
    <div class="panel panel-default">
        <div class="panel-body">
            <strong class="title">
                <asp:Label ID="lblItemDetailsTitle" runat="server" Text="Add Item" />
            </strong>

            <div class="form-horizontal compact">
                <asp:PlaceHolder runat="server" ID="phItemsList">
                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Items</label>
                        <div class="col-md-4">
                            <asp:DropDownList runat="server" ID="ddlItems" DataTextField="Description" DataValueField="ItemID" AutoPostBack="true" CssClass="form-control" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Part #</label>
                    <div class="col-md-4">
                        <asp:HiddenField runat="server" ID="hidPartNum" />
                        <asp:TextBox runat="server" ID="txtPartNum" MaxLength="50" CssClass="form-control" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Description *</label>
                    <div class="col-md-4">
                        <asp:HiddenField runat="server" ID="hidDescription" />
                        <asp:TextBox runat="server" ID="txtDescription" MaxLength="500" CssClass="form-control" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Category *</label>
                    <div class="col-md-4">
                        <asp:DropDownList runat="server" ID="ddlCat1" DataTextField="DisplayName" DataValueField="CategoryID" AutoPostBack="true" CssClass="form-control" />
                    </div>
                </div>

                <asp:PlaceHolder runat="server" ID="phChildCategory" Visible="false">
                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-1 col-md-3">
                            <asp:DropDownList runat="server" ID="ddlCat2" DataTextField="DisplayName" DataValueField="CategoryID" CssClass="form-control" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Quantity *</label>
                    <div class="col-md-11">
                        <div class="form-inline">
                            <asp:TextBox runat="server" ID="txtQuantity" MaxLength="10" CssClass="form-control" Width="80">1</asp:TextBox>
                            <label style="margin-left: 7px; font-size: 12px;">Unit</label>
                            <asp:TextBox runat="server" ID="txtUnit" MaxLength="5" CssClass="form-control" Width="80"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Unit Price *</label>
                    <div class="col-md-2">
                        <div class="input-group">
                            <span class="input-group-addon">$</span>
                            <asp:TextBox runat="server" ID="txtUnitPrice" MaxLength="20" CssClass="form-control" />
                        </div>
                    </div>
                </div>

                <asp:PlaceHolder runat="server" ID="phInventoryItem" Visible="false">
                    <div class="form-group form-group-sm">
                        <label class="col-md-1 control-label">Store Item *</label>
                        <div class="col-md-4">
                            <asp:DropDownList runat="server" ID="ddlInventoryItem" DataTextField="Description" DataValueField="InventoryItemID" CssClass="form-control" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="form-group form-group-sm">
                    <div class="col-md-offset-1 col-md-11">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" runat="server" id="chkAutoOverwrite" checked />
                                Automatically overwrite previously ordered items
                            </label>
                        </div>
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <div class="col-md-offset-1 col-md-11 div-spacing">
                        <div>
                            <asp:Button runat="server" ID="btnAddItem" Text="Add Item" CssClass="btn btn-default" />
                            <asp:Button runat="server" ID="btnUpdateItem" Text="Modify Item" Visible="false" OnClick="BtnUpdateItem_Click" CssClass="btn btn-default" />
                            <asp:Button runat="server" ID="btnCancelUpdate" Text="Cancel" Visible="false" CssClass="btn btn-default" />
                        </div>

                        <asp:PlaceHolder runat="server" ID="phErrItem" Visible="false">
                            <div style="color: #ff0000;">
                                <asp:Literal runat="server" ID="litErrItem"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Items List -->
    <uc:PurchaseOrderDetailView runat="server" ID="PurchaseOrderDetailView1" HideTitle="true" EnableItemDescriptionLink="true" OnItemDescriptionCommand="GvItems_RowCommand" ItemDescriptionCommandName="EditItem" CanDeleteItems="true" />
    
    <asp:HiddenField runat="server" ID="hidSelectedPODID" />
    <asp:Button runat="server" ID="btnSavePOItems" Text="Save and Continue" CssClass="btn btn-default" />
    <asp:Button runat="server" ID="btnCancelPOItems" Text="Cancel" OnClientClick="return alert('This purchase order is saved and you can continue later.');" CssClass="btn btn-default" />
    <br />
    <asp:Label ID="lblErrItems" runat="server" ForeColor="Red" />

    <div id="divDuplicateInDB" class="modal-dialog" title="Previously Ordered Item">
        <div style="text-align: center; padding: 10px;">
            It appears that you've ordered this item before, would you like to overwrite it (all fields except price will be affected in your historical IOFs)?<br />
            <br />
            <asp:Button runat="server" ID="btnOverwriteDB" Text="Overwrite" OnClick="BtnOverwriteDB_Click" CssClass="button" />
            <input type="button" id="btnCancel" value="Cancel" class="button" />
        </div>
    </div>

    <asp:LinkButton ID="lbtnDuplicateInDB" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        var showModalDialog = ('<%= Me.ShowModalDialog.ToString().ToLower()%>' == 'true');

        if (showModalDialog) {
            $('#divDuplicateInDB').dialog({ 'position': [200, 200], 'width': 850, 'height': 110, 'modal': true });
            $('#divDuplicateInDB').parent().appendTo($('form:first'));
        }

        $('#btnCancel').click(function () {
            $('#divDuplicateInDB').dialog("close");
        });
    </script>
</asp:Content>
