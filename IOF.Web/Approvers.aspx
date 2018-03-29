<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Approvers.aspx.vb" Inherits="IOF.Web.Approvers" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .iofgrid .approver-name-header {
            width: 550px;
        }

        .iofgrid .approver-primary-header {
            width: 66px;
        }

        .iofgrid .approver-primary-item,
        .iofgrid .approver-primary-footer {
            text-align: center;
        }

        .iofgrid .control-header {
            width: 100px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Manage Approvers</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-8">
                    <!-- Approvers List -->
                    <table class="iofgrid table table-striped">
                        <thead>
                            <tr>
                                <th>Approver</th>
                                <th style="width: 80px;">Primary</th>
                                <th style="width: 110px;">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rptApprovers" OnItemDataBound="rptApprovers_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <%#Eval("DisplayName")%>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label runat="server" ID="lblIsPrimary"><%#If(Convert.ToBoolean(Eval("IsPrimary")), "True", String.Empty)%></asp:Label>
                                            <asp:CheckBox runat="server" ID="chkIsPrimary" Visible="false" Checked='<%#Eval("IsPrimary")%>' />
                                        </td>
                                        <td>
                                            <div class="separator">
                                                <asp:PlaceHolder runat="server" ID="phEditDelete">
                                                    <span>
                                                        <asp:LinkButton runat="server" ID="lbtnEdit" OnCommand="Row_Command" CommandName="edit" CommandArgument='<%#Container.ItemIndex%>'>Edit</asp:LinkButton></span>
                                                    <span>
                                                        <asp:LinkButton runat="server" ID="lbtnDelete" OnCommand="Row_Command" CommandName="delete" CommandArgument='<%#Eval("ApproverID")%>'>Delete</asp:LinkButton></span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" ID="phUpdateCancel" Visible="false">
                                                    <span>
                                                        <asp:LinkButton runat="server" ID="lbtnUpdate" OnCommand="Row_Command" CommandName="update" CommandArgument='<%#String.Format("{0}:{1}", Container.ItemIndex, Eval("ApproverID"))%>'>Update</asp:LinkButton></span>
                                                    <span>
                                                        <asp:LinkButton runat="server" ID="lbtnCancel" OnCommand="Row_Command" CommandName="cancel" CommandArgument='<%#Container.ItemIndex%>'>Cancel</asp:LinkButton></span>
                                                </asp:PlaceHolder>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr runat="server" id="trNoData" visible="false">
                                <td>
                                    <em class="text-muted">No approvers have been added.</em>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlClients" DataTextField="DisplayName" DataValueField="ApproverID" CssClass="form-control" />
                                </td>
                                <td class="text-center">
                                    <asp:CheckBox runat="server" ID="chkNewIsPrimary" />
                                </td>
                                <td>
                                    <asp:LinkButton runat="server" ID="lbtnAddApprover" OnCommand="Row_Command" CommandName="add">Add Approver</asp:LinkButton>
                                </td>
                            </tr>
                        </tfoot>
                    </table>

                    <uc:BootstrapAlert runat="server" ID="Alert1" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
