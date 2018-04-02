<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IOFMaster.Master" CodeBehind="POInfo.aspx.vb" Inherits="IOF.Web.POInfo" %>

<%@ Register Src="~/Controls/VendorInfo.ascx" TagPrefix="uc" TagName="VendorInfo" %>
<%@ Register Src="~/Controls/BootstrapAlert.ascx" TagPrefix="uc" TagName="BootstrapAlert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Internal Order Info -->
    <h5>Internal Order Information</h5>

    <!-- PO Info Form -->
    <div class="panel panel-default" style="min-height: 370px;">
        <div class="panel-body">
            <asp:Panel runat="server" ID="pOrderInfo" Visible="true">

                <div class="form-horizontal compact">
                    <asp:PlaceHolder runat="server" ID="phStoreManager" Visible="false">
                        <div class="form-group form-group-sm">
                            <div class="col-md-offset-2 col-md-10">
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" runat="server" id="chkStoreManager" class="store-manager" />
                                        Store Manager
                                    </label>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Vendor *</label>
                        <div class="col-lg-4 col-md-6">
                            <asp:PlaceHolder runat="server" ID="phVendorName" Visible="false">
                                <p class="form-control-static">
                                    <asp:Literal runat="server" ID="litVendorName"></asp:Literal>
                                </p>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="phVendorList" Visible="false">
                                <div class="input-group">
                                    <asp:DropDownList runat="server" ID="ddlVendor" DataTextField="VendorName" DataValueField="VendorID" AppendDataBoundItems="true" CssClass="form-control" />
                                    <span class="input-group-addon separator">
                                        <span>
                                            <asp:LinkButton runat="server" ID="lbtnEditVendor" OnClick="lbtnEditVendor_Click" Text="Edit" /></span>
                                        <span>
                                            <asp:LinkButton runat="server" ID="lbtnAddVendor" OnClick="lbtnAddVendor_Click" Text="Add" /></span>
                                    </span>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Account *</label>
                        <div class="col-lg-4 col-md-6">
                            <div class="input-group">
                                <asp:DropDownList runat="server" ID="ddlAccount" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                                <span class="input-group-addon">
                                    <asp:LinkButton runat="server" ID="lbtnAddAccount" Text="Add" OnClick="lbtnAddAccount_Click" />
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Approver *</label>
                        <div class="col-lg-4 col-md-6">
                            <div class="input-group">
                                <asp:DropDownList runat="server" ID="ddlApprover" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                                <span class="input-group-addon">
                                    <asp:LinkButton runat="server" ID="lbtnAddApprover" Text="Add" OnClick="lbtnAddApprover_Click" />
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Date Needed *</label>
                        <div class="col-lg-2 col-md-4">
                            <asp:TextBox runat="server" ID="txtNeededDate" CssClass="date-picker form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Shipping</label>
                        <div class="col-lg-2 col-md-4">
                            <asp:DropDownList runat="server" ID="ddlShippingMethod" DataSourceID="odsShippingMethod" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                            <asp:ObjectDataSource runat="server" ID="odsShippingMethod" TypeName="IOF.Web.Repository"  SelectMethod="GetAllShippingMethods" />
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <label class="col-md-2 control-label">Notes</label>
                        <div class="col-lg-4 col-md-6">
                            <asp:TextBox ID="txtNotes" runat="server" Rows="5" TextMode="MultiLine" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-2 col-md-10">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" runat="server" id="chkOversized" />
                                    Oversized
                                </label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-2 col-md-10">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" runat="server" id="chkAttention" />
                                    Is Urgent?
                                </label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-2 col-md-4">
                            <asp:Button runat="server" ID="btnSavePOInfo" Text="Save and Continue" OnClick="btnSavePOInfo_Click" CssClass="btn btn-default" />
                            <button type="reset" class="btn btn-default">Clear</button>
                            <uc:BootstrapAlert runat="server" ID="Alert1" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Vendor Info -->
            <uc:VendorInfo runat="server" ID="VendorInfo1" Visible="false" OnCancelClick="VendorInfo1_CancelClick" OnAddClick="VendorInfo1_AddClick" OnUpdateClick="VendorInfo1_UpdateClick" />

            <!-- Approver Info -->
            <asp:Panel ID="pApprover" runat="server" Visible="false">
                <strong class="title">Add Approver</strong>

                <hr />

                <div class="row">
                    <div class="col-md-4 div-spacing">
                        <div>
                            <asp:DropDownList runat="server" ID="ddlApprovers" DataSourceID="odsClients" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                            <asp:ObjectDataSource runat="server" ID="odsClients" TypeName="IOF.Web.Repository" SelectMethod="GetAvailableApprovers" />
                        </div>

                        <div>
                            <asp:Button runat="server" ID="btnAddApprover" Text="Add Approver" OnClick="btnAddApprover_Click" CssClass="btn btn-default" />
                            <asp:Button runat="server" ID="btnApproverCancel" Text="Cancel" OnClick="btnApproverCancel_Click" CssClass="btn btn-default" />
                        </div>

                        <div>
                            <asp:Label runat="server" ID="lblErrApprover" ForeColor="#ff0000" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Account Info -->
            <asp:Panel ID="pAccount" runat="server" Visible="false">
                <strong class="title">Add Account</strong>

                <hr />

                <div class="row">
                    <div class="col-md-4 div-spacing">
                        <div>
                            <asp:DropDownList runat="server" ID="ddlAccounts" DataSourceID="odsAccounts" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                            <asp:ObjectDataSource runat="server" ID="odsAccounts" TypeName="IOF.Web.Repository" SelectMethod="GetAvailableAccounts" />
                        </div>

                        <div>
                            <asp:Button runat="server" ID="btnAddAccount" Text="Add Account" OnClick="btnAddAccount_Click" CssClass="btn btn-default" />
                            <asp:Button runat="server" ID="btnAccountCancel" Text="Cancel" OnClick="btnAccountCancel_Click" CssClass="btn btn-default" />
                        </div>

                        <div>
                            <asp:Label runat="server" ID="lblErrAccount" ForeColor="#ff0000" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
