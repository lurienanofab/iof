<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="TrackIOF.aspx.vb" Inherits="IOF.Web.TrackIOF" %>

<%@ Register Src="~/Controls/PurchaseOrderSearch.ascx" TagPrefix="uc" TagName="PurchaseOrderSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:PlaceHolder runat="server" ID="phSearch" Visible="true">
        <uc:PurchaseOrderSearch runat="server" ID="PurchaseOrderSearch1" Title="IOF Tracking" EnableCopy="false" DisplayOption="Summary" />
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phDetail" Visible="false">
        <h5>IOF Tracking</h5>

        <div class="panel panel-default">
            <div class="panel-body">
                <strong class="title">
                    <span>Tracking Information for POID</span>
                    <asp:Label runat="server" ID="lblPOID"></asp:Label>
                </strong>

                <div class="row">
                    <div class="col-md-8">
                        <asp:Repeater runat="server" ID="rptTracking">
                            <HeaderTemplate>
                                <table class="iofgrid table table-striped">
                                    <thead>
                                        <tr>
                                            <th style="width: 180px;">Date/Time</th>
                                            <th style="width: 340px;">Description</th>
                                            <th>Notes</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("TrackingDateTime", "{0:MM/dd/yyyy hh:mm:ss tt}")%></td>
                                    <td><%#Eval("CheckpointText")%></td>
                                    <td><%#GetNotes(Container.DataItem)%></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>

        <div class="btn-spacing">
            <asp:Button runat="server" ID="btnView" Text="View" OnClick="btnView_Click" CssClass="btn btn-default" />
            <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
