<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Tracking.ascx.vb" Inherits="IOF.Web.Controls.Tracking" %>

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
