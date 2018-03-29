<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurchaseOrderDetailView.ascx.vb" Inherits="IOF.Web.Controls.PurchaseOrderDetailView" %>

<div class="panel panel-default detail">
    <div class="panel-body">
        <asp:HiddenField runat="server" ID="hidPOID" />

        <strong class="title">
            <asp:LinkButton runat="server" ID="btnPurchaseOrderItems" OnCommand="btnPurchaseOrderItems_Command" CommandName='<%=TitleCommandName%>' Visible="false">Internal Order Items</asp:LinkButton>
            <asp:Label runat="server" ID="lblPurchaseOrderItems" Visible="true">Interal Order Items</asp:Label>
        </strong>

        <asp:Repeater runat="server" ID="rptItems" OnItemDataBound="rptItems_ItemDataBound">
            <HeaderTemplate>
                <table class="iofgrid table table-striped po-items">
                    <thead>
                        <tr>
                            <th style="width: 150px;">Part #</th>
                            <th>Description</th>
                            <th style="width: 150px;">Category</th>
                            <th style="width: 80px;">Qty</th>
                            <th style="width: 100px;">Unit Price</th>
                            <th style="width: 100px;">Ext Price</th>
                            <th runat="server" style="width: 60px;" visible='<%#CanDeleteItems%>'>&nbsp;</th>
                            <th runat="server" style="width: 130px;" visible='<%#PurchaserCanModify()%>'>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="partnum" data-value='<%#Eval("PartNum")%>'>
                        <span class="value"><%#Eval("PartNum")%></span>
                        <input type="text" class="form-control" style="display: none;" />
                    </td>
                    <td class="description" data-value='<%#Eval("Description")%>'>
                        <asp:Label runat="server" ID="lblItemDescription" Visible='<%#Not EnableItemDescriptionLink%>' CssClass="value"><%#Eval("Description")%></asp:Label>
                        <asp:LinkButton runat="server" ID="btnItemDescriptionLink" Text='<%#Eval("Description")%>' OnCommand="btnItemDescriptionLink_Command" CommandArgument='<%# Eval("PODID")%>' CommandName='<%#ItemDescriptionCommandName%>' Visible='<%#EnableItemDescriptionLink%>'></asp:LinkButton>
                    </td>
                    <td class="category" data-value='<%#Eval("CategoryName")%>'>
                        <span class="value"><%#Eval("CategoryName")%></span>
                    </td>
                    <td class="quantity text-right" data-value='<%#Eval("Quantity")%>' data-unit='<%#Eval("Unit")%>'>
                        <span class="value"><%#String.Format("{0} {1}", Eval("Quantity"), Eval("Unit")).Trim()%></span>
                        <input type="text" class="form-control text-right" style="display: none;" />
                    </td>
                    <td class="unit-price text-right" data-value='<%#Eval("UnitPrice")%>'>
                        <span class="value"><%#Eval("UnitPrice", "{0:C}")%></span>
                        <input type="text" class="form-control text-right" style="display: none;" />
                    </td>
                    <td class="ext-price text-right" data-value='<%#Eval("ExtPrice")%>'>
                        <span class="value"><%#Eval("ExtPrice", "{0:C}")%></span>
                    </td>
                    <td runat="server" visible='<%#CanDeleteItems%>' class="text-center">
                        <asp:LinkButton runat="server" ID="lbtnDeletePurchaseOrderDetailItem" OnCommand="Row_Command" CommandName="delete" CommandArgument='<%#String.Format("{0}:{1}", Container.ItemIndex, Eval("PODID"))%>'>Delete</asp:LinkButton>
                    </td>
                    <td runat="server" visible='<%#PurchaserCanModify()%>'>
                        <div class="item-edit">
                            <a href="#" data-command="edit" data-index='<%#Container.ItemIndex%>' data-podid='<%#Eval("PODID")%>'>Edit</a>
                        </div>
                        <div class="item-update-cancel separator" style="display: none;">
                            <span>
                                <a href="#" data-command="update" data-index='<%#Container.ItemIndex%>' data-podid='<%#Eval("PODID")%>'>Update</a></span>
                            <span>
                                <a href="#" data-command="cancel" data-index='<%#Container.ItemIndex%>' data-podid='<%#Eval("PODID")%>'>Cancel</a></span>
                        </div>
                        <div class="working" style="display: none;">
                            <em class="text-muted">working...</em>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                <tfoot>
                    <tr style="font-weight: bold;">
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td class="text-right">Total:</td>
                        <td class="text-right total-ext-price">
                            <span class="value">
                                <asp:Literal runat="server" ID="litTotalExtPrice"></asp:Literal></span>
                        </td>
                        <td runat="server" visible='<%#CanDeleteItems%>'>&nbsp;</td>
                        <td runat="server" visible='<%#PurchaserCanModify()%>'>&nbsp;</td>
                    </tr>
                </tfoot>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder runat="server" ID="phNoData" Visible="false">
            <em class="text-muted">There are no items in this IOF.</em>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="phUpdateOption" Visible="true">
            <div class="checkbox">
                <label>
                    <input type="checkbox" class="update-po-only" />
                    Update prices for this PO only (do not update prices in inventory)
                </label>
            </div>
        </asp:PlaceHolder>
    </div>
</div>
