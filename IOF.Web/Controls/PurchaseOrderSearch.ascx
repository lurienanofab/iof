<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurchaseOrderSearch.ascx.vb" Inherits="IOF.Web.Controls.PurchaseOrderSearch" %>

<h5>
    <asp:Literal runat="server" ID="litTitle"></asp:Literal>
</h5>

<!-- Search Filter -->
<div class="iofsearch" data-action="<%=Action%>" data-status-id-list="<%=StatusIdList%>" data-display-option="<%=Convert.ToInt32(DisplayOption)%>" data-client-id="<%=IOFContext.CurrentUser.ClientID%>">
    <div class="panel panel-default">
        <div class="panel-body">
            <strong class="title">Filter Options:</strong>

            <div class="form-horizontal compact">
                <asp:PlaceHolder runat="server" ID="phStoreManager" Visible="false">
                    <div class="form-group form-group-sm">
                        <div class="col-md-offset-1 col-md-11">
                            <div class="checkbox">
                                <label>
                                    <input type="checkbox" runat="server" id="chkStoreManager" class="store-manager" tabindex="13" />
                                    Store Manager
                                </label>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Created On</label>
                    <div class="col-md-4">
                        <div class="form-inline">
                            <div class="form-group form-group-sm">
                                <input type="text" runat="server" id="txtStartDate" class="start-date-text form-control" placeholder="MM/DD/YYYY" style="width: 120px;" tabindex="1" />
                            </div>
                            <div class="form-group form-group-sm">
                                <label style="font-weight: normal;">to</label>
                                <input type="text" runat="server" id="txtEndDate" class="end-date-text form-control" placeholder="MM/DD/YYYY" style="width: 120px;" tabindex="2" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Vendor</label>
                    <div class="col-md-4">
                        <div class="input-group">
                            <div class="vendor-list-container">
                                <asp:DropDownList runat="server" ID="ddlVendors" DataTextField="VendorName" DataValueField="VendorID" CssClass="vendor-name-list form-control" TabIndex="3" />
                            </div>
                            <div class="vendor-text-container" style="display: none;">
                                <input type="text" runat="server" id="txtVendorName" class="vendor-name-text form-control" placeholder="Search by vendor name..." tabindex="4" />
                            </div>
                            <span class="input-group-addon">
                                <a href="#" class="vendor-toggle" data-vendor-search-type="text">text</a>
                            </span>
                        </div>
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Keywords</label>
                    <div class="col-md-4">
                        <input type="text" runat="server" id="txtKeywords" class="keywords form-control" maxlength="500" tabindex="5" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Part #</label>
                    <div class="col-md-2">
                        <input type="text" runat="server" id="txtPartNumber" class="part-num form-control" maxlength="50" tabindex="6" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">IOF #</label>
                    <div class="col-md-2">
                        <input type="text" runat="server" id="txtPOID" class="poid form-control" maxlength="50" tabindex="7" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">ShortCode</label>
                    <div class="col-md-2">
                        <input type="text" runat="server" id="txtShortCode" class="shortcode form-control" maxlength="50" tabindex="8" />
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <label class="col-md-1 control-label">Client</label>
                    <div class="col-md-4">
                        <div class="input-group">
                            <asp:DropDownList runat="server" ID="ddlClients" DataSourceID="odsClients" DataTextField="Text" DataValueField="Value" CssClass="other-client-id form-control" TabIndex="9" />
                            <asp:ObjectDataSource runat="server" ID="odsClients" TypeName="IOF.Web.Repository" SelectMethod="GetAllStaff" />
                            <div class="input-group-addon">
                                <input type="checkbox" runat="server" id="chkIncludeSelf" class="include-self" tabindex="10" checked />
                                Include Myself
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-group form-group-sm">
                    <div class="col-md-offset-1 col-md-11">
                        <button type="button" class="btn btn-default search-button" tabindex="11">Search</button>
                        <button type="button" class="btn btn-default clear-button" tabindex="12">Clear</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Search PO Result List -->
    <table class="iofgrid datatable table table-hover">
        <thead>
            <tr>
                <th style="width: 50px;">POID</th>
                <th style="width: 120px;">Created By</th>
                <th style="width: 120px;">Approved By</th>
                <th style="width: 120px;">Part #</th>
                <th style="width: 240px;">Description</th>
                <th style="width: 100px;">Category</th>
                <th style="width: 150px;">Vendor</th>
                <th style="width: 80px;">Created On</th>
                <th style="width: 80px;">ShortCode</th>
                <th style="width: 80px;">Total Price</th>
                <th style="width: 80px;">Status</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>
