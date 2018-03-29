<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurchasingSearch.ascx.vb" Inherits="IOF.Web.Controls.PurchasingSearch" %>

<div class="panel panel-default purchsearch">
    <div class="panel-body">
        <asp:HiddenField runat="server" ID="hidStatusIDs" />
        <strong class="title">
            <asp:Literal runat="server" ID="litTitle">Purchasing Search</asp:Literal>
        </strong>

        <div class="form-horizontal compact">
            <asp:PlaceHolder runat="server" ID="phPOID">
                <div class="form-group form-group-sm">
                    <label class="col-lg-1 col-md-2 control-label">PO #</label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="txtPOID" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phPurchaser">
                <div class="form-group form-group-sm">
                    <label class="col-lg-1 col-md-2 control-label">Purchaser</label>
                    <div class="col-md-3">
                        <asp:DropDownList runat="server" ID="ddlPurchaser" DataTextField="DisplayName" DataValueField="ClientID" CssClass="form-control" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phCreator">
                <div class="form-group form-group-sm">
                    <label class="col-lg-1 col-md-2 control-label">Created By</label>
                    <div class="col-md-3">
                        <asp:DropDownList runat="server" ID="ddlCreator" DataTextField="DisplayName" DataValueField="ClientID" CssClass="form-control" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phRealPO">
                <div class="form-group form-group-sm">
                    <label class="col-lg-1 col-md-2 control-label">Real PO</label>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="txtRealPO" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phDateRange">
                <div class="form-group form-group-sm">
                    <label class="col-lg-1 col-md-2 control-label">Date</label>
                    <div class="col-lg-11 col-md-10">
                        <div class="form-inline">
                            <div class="form-group">
                                <asp:DropDownList runat="server" ID="ddlDatePresets" AutoPostBack="false" CssClass="form-control date-preset-select">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Today" Value="today"></asp:ListItem>
                                    <asp:ListItem Text="Yesterday" Value="yesterday"></asp:ListItem>
                                    <asp:ListItem Text="This Week" Value="this-week"></asp:ListItem>
                                    <asp:ListItem Text="Last Week" Value="last-week"></asp:ListItem>
                                    <asp:ListItem Text="This Month" Value="this-month"></asp:ListItem>
                                    <asp:ListItem Text="Last Month" Value="last-month"></asp:ListItem>
                                    <asp:ListItem Text="This Year" Value="this-year"></asp:ListItem>
                                    <asp:ListItem Text="Last Year" Value="last-year"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control start-date" Width="110"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control end-date" Width="110"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <a href="#" class="clear-date" style="display: inline-block; margin-left: 5px;">
                                    <img src="https://ssel-apps.eecs.umich.edu/static/images/delete.png" alt="Clear" title="Clear" />
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phClaimStatus">
                <div class="form-group form-group-sm">
                    <div class="col-lg-offset-1 col-md-offset-2 col-lg-11 col-md-10">
                        <label class="radio-inline">
                            <input type="radio" runat="server" name="ClaimStatus" id="rdoClaimStatusUnclaimed" />
                            Unclaimed
                        </label>
                        <label class="radio-inline">
                            <input type="radio" runat="server" name="ClaimStatus" id="rdoClaimStatusClaimed" />
                            Claimed
                        </label>
                        <label class="radio-inline">
                            <input type="radio" runat="server" name="ClaimStatus" id="rdoClaimStatusClaimedBy" />
                            My IOFs
                        </label>
                        <label class="radio-inline">
                            <input type="radio" runat="server" name="ClaimStatus" id="rdoClaimStatusAll" checked />
                            All
                        </label>
                    </div>
                </div>
            </asp:PlaceHolder>

            <div class="form-group form-group-sm">
                <div class="col-lg-offset-1 col-md-offset-2 col-lg-11 col-md-10">
                    <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-default" />
                    <asp:Button runat="server" ID="btnReset" Text="Reset" OnClick="btnReset_Click" CssClass="btn btn-default" />
                </div>
            </div>
        </div>

        <asp:Literal runat="server" ID="litDebug"></asp:Literal>
    </div>
</div>
