<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="UnfinishedIOF.aspx.vb" Inherits="IOF.Web.UnfinishedIOF" %>

<%@ Import Namespace="LNF.Ordering" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Draft IOF List -->
    <h5>Draft IOF</h5>

    <div class="panel panel-default">
        <div class="panel-body">

            <div class="row">
                <div class="col-md-6">

                    <asp:Repeater runat="server" ID="rptDraft">
                        <HeaderTemplate>
                            <table class="iofgrid table table-striped">
                                <thead>
                                    <tr>
                                        <th style="width: 80px;">POID</th>
                                        <th>Vendor</th>
                                        <th style="width: 160px;">Created On</th>
                                        <th style="width: 120px;">Total Price</th>
                                        <th style="width: 80px;">&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HyperLink runat="server" ID="hypPOID" NavigateUrl='<%#Eval("POID", "~/POConfirm.aspx?Action=Unfinished&POID={0}")%>'><%#Eval("POID")%></asp:HyperLink>
                                </td>
                                <td><%#Eval("VendorName")%></td>
                                <td class="text-center"><%#Eval("CreatedDate", "{0:MM/dd/yyyy hh:mm tt}")%></td>
                                <td class="text-right"><%#Eval("TotalPrice", "{0:C}")%></td>
                                <td class="text-center">
                                    <asp:LinkButton runat="server" ID="lbtnDeleteDraft" OnCommand="Row_Command" CommandName="delete" CommandArgument='<%#Eval("POID")%>'>Delete</asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:PlaceHolder runat="server" ID="phNoData" Visible="false">
                        <em class="text-muted">You have no draft IOFs.</em>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
