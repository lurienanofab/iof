<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurchaseOrderAttachments.ascx.vb" Inherits="IOF.Web.PurchaseOrderAttachments" %>

<div class="panel panel-default">
    <div class="panel-body">
        <asp:HiddenField runat="server" ID="hidPOID" />
        <asp:HiddenField runat="server" ID="hidReadOnly" />

        <strong class="title">Attachments</strong>

        <asp:PlaceHolder runat="server" ID="phUpload">
            <div>
                <div class="form-inline" style="margin-bottom: 20px;">
                    <div class="form-group">
                        <asp:FileUpload runat="server" ID="fuAttachments" AllowMultiple="true" CssClass="button" />
                    </div>
                    <asp:Button runat="server" ID="btnAddAttachment" Text="Add Attachment" OnClick="btnAddAttachment_Click" CssClass="btn btn-default" />
                </div>
            </div>
        </asp:PlaceHolder>

        <div class="row">
            <div class="col-md-6">
                <asp:Repeater runat="server" ID="rptAttachments">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <a href="<%#Eval("Url")%>" target="_blank"><%#Eval("FileName")%></a>
                            <asp:LinkButton runat="server" ID="lbtnDeleteAttachment" OnCommand="lbtnDeleteAttachment_Command" CommandName="delete" CommandArgument='<%#Eval("FileName")%>' CssClass="attachments-delete pull-right" Visible='<%#Not [ReadOnly]%>'>delete</asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:PlaceHolder runat="server" ID="phNoAttachments" Visible="false">
                    <em class="text-muted">No attachments have been added yet.</em>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</div>
