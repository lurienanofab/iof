<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Purchasers.aspx.vb" Inherits="IOF.Web.Purchasers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Manage Purchasers</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-5">
                    <table class="iofgrid table table-striped">
                        <thead>
                            <tr>
                                <th>Purchaser</th>
                                <th style="width: 80px;">Active</th>
                                <th style="width: 130px;">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rptPurchasers" OnItemDataBound="rptPurchasers_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <%#Eval("DisplayName")%>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label runat="server" ID="lblActive"><%#If(Convert.ToBoolean(Eval("Active")), "True", "False")%></asp:Label>
                                            <asp:CheckBox runat="server" ID="chkActive" Checked='<%#Convert.ToBoolean(Eval("Active"))%>' Visible="false" />
                                        </td>
                                        <td class="separator">
                                            <span>
                                                <asp:LinkButton runat="server" ID="lbtnEditUpdate" CommandName="edit" CommandArgument='<%#String.Format("{0}:{1}", Container.ItemIndex, Eval("PurchaserID"))%>' OnCommand="Row_Command">Edit</asp:LinkButton></span>
                                            <span>
                                                <asp:LinkButton runat="server" ID="lbtnDeleteCancel" CommandName="delete" CommandArgument='<%#String.Format("{0}:{1}", Container.ItemIndex, Eval("PurchaserID"))%>' OnCommand="Row_Command">Delete</asp:LinkButton></span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr runat="server" id="trNoData" visible="false">
                                <td>
                                    <em class="text-muted">No purchasers were found.</em>
                                </td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlAvailable" DataTextField="DisplayName" DataValueField="ClientID" CssClass="form-control" />
                                </td>
                                <td class="text-center">
                                    <asp:CheckBox runat="server" ID="chkActive" Checked="true" />
                                </td>
                                <td>
                                    <asp:LinkButton runat="server" ID="lbtnAddPurchaser" OnClick="lbtnAddPurchaser_Click">Add Purchaser</asp:LinkButton>
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <div>
        <!-- Approvers List -->
        <asp:GridView runat="server" ID="zgvPurchasers" ShowFooter="true" DataKeyNames="PurchaserID" CssClass="iofgrid" AutoGenerateColumns="false">
            <RowStyle VerticalAlign="Middle" CssClass="row" />
            <AlternatingRowStyle VerticalAlign="Middle" CssClass="altrow" />
            <EditRowStyle VerticalAlign="Middle" />
            <FooterStyle VerticalAlign="Middle" CssClass="footer" />
            <PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" CssClass="pager" />
            <PagerSettings Position="Bottom" FirstPageText="<<" LastPageText=">>" PreviousPageText="<" NextPageText=">" Mode="NumericFirstLast" />
            <Columns>
                <asp:TemplateField HeaderText="Purchaser">
                    <ItemTemplate>
                        <asp:HiddenField runat="server" ID="hidClientID" />
                        <%# Eval("DisplayName")%>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Active" HeaderStyle-Width="80" FooterStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# If(Eval("Active").ToString() = "True", "True", "")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="ckbIsActive" runat="server" Checked='<%#If(Eval("Active").ToString() = "True", "true", "false")%>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:CheckBox ID="ckbNewIsActive" runat="server" Checked="true" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-Width="90" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" Text="Edit" CommandName="Edit" OnClientClick='<%# "return " + (Not (Eval("PurchaserID") Is DBNull.Value)).ToString().ToLower() + ";" %>' />
                        <asp:LinkButton ID="lbtnDelete" runat="server" Text="Delete" CommandName="Delete" OnClientClick='<%# "return (" + (Eval("PurchaserID") Is DBNull.Value).ToString().ToLower() + ") ? false : checkDelete();"  %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lbtnUpdate" runat="server" Text="Update" CommandName="Update" ValidationGroup="EditPurchaser" />
                        <asp:LinkButton ID="lbtnCancel" runat="server" Text="Cancel" CommandName="Cancel" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="lbtnAdd" runat="server" Text="Add Purchaser" CommandName="AddPurchaser" ValidationGroup="NewPurchaser" />
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Label ID="lblErr" runat="server" ForeColor="Red" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        function checkDelete() {
            return confirm('Are you sure you want to delete this approver?');
        }
    </script>
</asp:Content>
