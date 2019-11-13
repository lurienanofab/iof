<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VendorInfo.ascx.vb" Inherits="IOF.Web.Controls.VendorInfo" %>

<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<strong class="title">Vendor Info</strong>

<hr />

<asp:HiddenField runat="server" ID="hidVendorID" Value="" />

<div class="form-horizontal compact">
    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Vendor Name *</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtVendorName" MaxLength="50" CssClass="form-control vendor-name" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Address (line 1) *</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtAddress1" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Address (line 2)</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtAddress2" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Address (line 3)</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtAddress3" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Contact</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtContact" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Phone *</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtPhone" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Fax</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtFax" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">URL</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtURL" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <label class="col-md-2 control-label">Email</label>
        <div class="col-md-3">
            <asp:TextBox runat="server" ID="txtEmail" MaxLength="50" CssClass="form-control" />
        </div>
    </div>

    <div class="form-group form-group-sm">
        <div class="col-md-offset-2 col-md-3">
            <div>
                <asp:Button runat="server" ID="btnUpdateVendor" Text="Update Vendor" OnClick="BtnUpdateVendor_Click" CssClass="btn btn-default" />
                <asp:Button runat="server" ID="btnAddVendor" Text="Add Vendor" OnClick="BtnAddVendor_Click" CssClass="btn btn-default" />
                <asp:Button runat="server" ID="btnVendorCancel" Text="Cancel" OnClick="BtnVendorCancel_Click" CssClass="btn btn-default" />
                <uc:BootstrapAlert runat="server" ID="Alert1" />
            </div>
        </div>
    </div>
</div>
