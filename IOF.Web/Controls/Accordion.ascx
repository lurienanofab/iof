<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Accordion.ascx.vb" Inherits="IOF.Web.Accordion" %>

<!-- Navigation -->
<div class="iofnav panel-group" id="accordion" role="tablist" aria-multiselectable="true">
    <div class="panel panel-default">
        <div class="panel-heading" role="tab" id="headingHome">
            <h4 class="panel-title">
                <a runat="server" id="lnkHome" data-parent="#accordion" aria-expanded="false">Home</a>
            </h4>
        </div>
    </div>

    <asp:Repeater runat="server" ID="rptAccordionGroups" OnItemDataBound="rptAccordion_ItemDataBound">
        <ItemTemplate>
            <div class="panel panel-default">
                <div class="panel-heading" role="tab" id='<%#Eval("HeadingID")%>'>
                    <h4 class="panel-title">
                        <a class='<%#Eval("TitleCssClass")%>' role="button" data-toggle="collapse" data-parent="#accordion" href='<%#Eval("CollapseID", "#{0}") %>' aria-expanded='<%#Eval("AriaExpanded")%>' aria-controls='<%#Eval("CollapseID")%>'><%#Eval("Title")%></a>
                    </h4>
                </div>
                <div id='<%#Eval("CollapseID")%>' class='<%#Eval("CollapseCssClass")%>' role="tabpanel" aria-labelledby='<%#Eval("HeadingID")%>'>
                    <div class="panel-body">
                        <asp:Repeater runat="server" ID="rptNavLinks">
                            <ItemTemplate>
                                <a href='<%#VirtualPathUtility.ToAbsolute(Eval("Url"))%>' class='<%#Eval("CssClass")%>' target='<%#Eval("Target")%>'><%#Eval("Text")%></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
