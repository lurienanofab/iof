<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Vendors.aspx.vb" Inherits="IOF.Web.Vendors" %>

<%@ Register Src="~/Controls/VendorInfo.ascx" TagName="VendorInfo" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Manage Vendors</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <!-- Vendors List -->
            <asp:PlaceHolder runat="server" ID="phVendorList" Visible="true">
                <div class="row">
                    <div class="col-md-10">
                        <asp:PlaceHolder runat="server" ID="phStoreManager" Visible="false">
                            <div class="checkbox" style="margin-bottom: 20px;">
                                <label>
                                    <input type="checkbox" runat="server" id="chkStoreManager" class="store-manager" />
                                    Store Manager
                                </label>
                            </div>
                        </asp:PlaceHolder>

                        <table class="iofgrid datatable table table-striped">
                            <thead>
                                <tr>
                                    <th style="width: 50px;">ID</th>
                                    <th>Vendor Name</th>
                                    <th style="width: 200px;">Contact</th>
                                    <th style="width: 110px;">&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptVendors">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%#Eval("VendorID")%>
                                            </td>
                                            <td>
                                                <asp:LinkButton runat="server" ID="lbtnVendorName" OnCommand="Row_Command" CommandName="edit" CommandArgument='<%#Eval("VendorID")%>'><%#Eval("VendorName")%></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%#Eval("Contact")%>
                                            </td>
                                            <td>
                                                <asp:LinkButton runat="server" ID="lbtnDeleteVendor" OnCommand="Row_Command" CommandName="delete" CommandArgument='<%#Eval("VendorID")%>'>Delete</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:LinkButton runat="server" ID="lbtnAddVendor" OnCommand="Row_Command" CommandName="add">Add Vendor</asp:LinkButton>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- Vendor Info -->
            <uc:VendorInfo runat="server" ID="VendorInfo1" Visible="false" OnCancelClick="VendorInfo1_CancelClick" OnUpdateClick="VendorInfo1_UpdateClick" OnAddClick="VendorInfo1_AddClick" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        $(".datatable").DataTable({
            "stateSave": true,
            "autoWidth": false,
            "order": [[1, "asc"]],
            "columns": [
                null,
                null,
                null,
                { "orderable": false, "searchable": false }
            ]
        });
    </script>
</asp:Content>
