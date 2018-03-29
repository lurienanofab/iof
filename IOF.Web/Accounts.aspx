<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="Accounts.aspx.vb" Inherits="IOF.Web.Accounts" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Manage Accounts</h5>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-8">
                    <!-- Accounts List -->
                    <table class="iofgrid table table-striped">
                        <thead>
                            <tr>
                                <th>Account</th>
                                <th style="width: 110px;">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rptAccounts">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("AccountDisplayName")%></td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="lbtnDeleteAccount" OnCommand="Row_Command" CommandName="delete" CommandArgument='<%#Eval("AccountID")%>'>Delete</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr runat="server" id="trNoData" visible="false">
                                <td>
                                    <em class="text-muted">No accounts have been added.</em>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlAccounts" DataTextField="AccountDisplayName" DataValueField="AccountID" CssClass="form-control" />
                                </td>
                                <td>
                                    <asp:LinkButton runat="server" ID="lbtnAddAccount" OnCommand="Row_Command" CommandName="add" CommandArgument='<%#Eval("AccountID")%>'>Add Account</asp:LinkButton>
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
