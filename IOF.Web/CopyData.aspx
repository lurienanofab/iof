<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="CopyData.aspx.vb" Inherits="IOF.Web.CopyData" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h5>Copy Data</h5>

    <div class="alert alert-warning">
        <strong style="color: #ff0000;">WARNING:</strong>
        <p>
            This does not check to see if you have already added the vendor and/or items. Please make sure you are not copying data you already have. If you are unsure contact the system administrator at <a href="mailto:lnf-it@umich.edu">lnf-it@umich.edu</a>. If you make a mistake duplicates will be created and this can be hard to clean up later.
        </p>
    </div>

    <!-- Copy To -->
    <div class="panel panel-default">
        <div class="panel-body">
            <strong class="title">Copy To</strong>
            <div class="form-horizontal compact">
                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Client</label>
                    <div class="col-md-3">
                        <asp:DropDownList runat="server" ID="ddlCopyToClient" DataTextField="DisplayName" DataValueField="ClientID" CssClass="form-control" />
                        <asp:PlaceHolder runat="server" ID="phCopyToClientName" Visible="false">
                            <p class="form-control-static">
                                <asp:Literal runat="server" ID="litCopyToClientName"></asp:Literal>
                                <asp:HiddenField runat="server" ID="hidCopyToClientID" />
                            </p>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Copy From -->
    <div class="panel panel-default">
        <div class="panel-body">
            <strong class="title">Copy From</strong>

            <div class="form-horizontal compact">
                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Client</label>
                    <div class="col-md-3">
                        <asp:DropDownList runat="server" ID="ddlCopyFromClient" DataTextField="DisplayName" DataValueField="ClientID" AutoPostBack="true" OnSelectedIndexChanged="ddlCopyFromClient_SelectedIndexChanged" CssClass="form-control" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Vendor</label>
                    <div class="col-md-3">
                        <asp:DropDownList runat="server" ID="ddlCopyFromVendor" DataTextField="VendorName" DataValueField="VendorID" CssClass="form-control" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <div class="col-md-offset-1 col-md-3">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" runat="server" id="chkCopyFromIncludeItems0" checked />
                                Include Items
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Button runat="server" ID="btnCopyData" Text="Copy Data" OnClick="btnCopyData_Click" CssClass="btn btn-default" />

    <uc:BootstrapAlert runat="server" ID="Alert1" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
