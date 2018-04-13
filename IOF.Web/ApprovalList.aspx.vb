Imports IOF.Models

Public Class ApprovalList
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadData()
        End If
    End Sub

    Private Sub LoadData()
        Dim filter As Func(Of Order, Boolean) = Function(x) True

        Dim id As Integer = -1

        phFilterAlert.Visible = False

        If Not String.IsNullOrEmpty(Request.QueryString("filter")) Then
            If Integer.TryParse(Request.QueryString("filter"), id) Then
                Dim c As Client = ClientRepository.Single(id)
                filter = Function(x) x.ClientID = id
                litFilterAlertName.Text = $"{c.FName} {c.LName}"
                phFilterAlert.Visible = True
            End If
        End If

        Dim clientId As Integer = IOFContext.CurrentUser.ClientID
        Dim query As IEnumerable(Of Order) = OrderRepository.GetAwaitingApproval()

        Dim mine As IEnumerable(Of Order) = From x In query
                                            Where x.ApproverID = clientId AndAlso filter(x)

        Dim others As IEnumerable(Of Order) = From x In query
                                              Where x.ApproverID <> clientId AndAlso filter(x)

        Dim isApprover As Boolean = MyBase.IsApprover()

        hidIsApprover.Value = If(isApprover, "true", "false")

        phMine.Visible = isApprover

        thApproveRejectHeaderMine.Visible = isApprover
        thApproveRejectHeaderOther.Visible = isApprover

        litApproverName.Text = $"by {IOFContext.CurrentUser.DisplayName}"

        If isApprover Then
            litOtherApproverName.Text = $"by Other Managers"
        Else
            litOtherApproverName.Text = String.Empty
        End If


        If mine.Count() > 0 Then
            rptApprovalPendingMine.Visible = True
            rptApprovalPendingMine.DataSource = CreateApprovalListItems(mine).OrderBy(Function(x) x.Sort)
            rptApprovalPendingMine.DataBind()
        Else
            If isApprover Then
                rptApprovalPendingMine.Visible = True
                rptApprovalPendingMine.DataSource = CreateApprovalListItems(mine).OrderBy(Function(x) x.Sort)
                rptApprovalPendingMine.DataBind()
            Else
                rptApprovalPendingMine.Visible = False
            End If
        End If

        rptApprovalPendingOther.DataSource = CreateApprovalListItems(others).OrderBy(Function(x) x.Sort)
        rptApprovalPendingOther.DataBind()
    End Sub

    Private Function CreateApprovalListItems(orders As IEnumerable(Of Order)) As IEnumerable(Of ApprovalListItem)
        Dim result As IEnumerable(Of ApprovalListItem)

        result = From x In orders
                 Select New ApprovalListItem() With {
                    .ApproverName = x.ApproverName,
                    .DisplayName = x.DisplayName,
                    .ShortCode = AccountRepository.GetShortCode(x.AccountID),
                    .POID = x.POID,
                    .CreatedDate = x.CreatedDate,
                    .Total = x.TotalPrice,
                    .Sort = If(x.ApproverID = IOFContext.CurrentUser.ClientID, $"_{x.ApproverName}", x.ApproverName)}

        Return result
    End Function

    Protected Sub gv_RowCreated(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim panRowButtons As Panel = CType(e.Row.FindControl("panRowButtons"), Panel)
            If Convert.ToBoolean(ViewState("IsApprover")) Then
                panRowButtons.Visible = True
            End If
        End If
    End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        ApproveAlert.Hide()
        hidCurrentPOID.Value = e.CommandArgument.ToString()
        Dim poid As Integer = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName
            Case "view"
                PurchaseOrderHeaderView1.POID = poid
                PurchaseOrderHeaderView1.DataBind()

                PurchaseOrderDetailView1.POID = poid
                PurchaseOrderDetailView1.DataBind()

                hypPrintIOF.NavigateUrl = $"~/PrintIOF.ashx?POID={poid}"

                phApprovalList.Visible = False
                panDetail.Visible = True
            Case "approve"
                Try
                    OrderRepository.Approve(poid, IOFContext.CurrentUser.ClientID)
                    Dim filePath As String = PdfService.CreatePDF(poid)
                    EmailService.SendPurchaserEmail(poid, filePath)
                    ApproveAlert.Show($"Thank you. IOF #{poid} has been approved. An email has been sent to purchasing.", BootstrapAlertType.Success)
                Catch ex As Exception
                    ApproveAlert.Show($"A problem occurred and your IOF was not approved. {ex.Message}")
                Finally
                    LoadData()
                End Try
            Case "reject"
                OrderRepository.Reject(poid, IOFContext.CurrentUser.ClientID)
                ApproveAlert.Show($"IOF #{poid} has been rejected.", BootstrapAlertType.Success)
                phReject.Visible = True
                LoadData()
        End Select
    End Sub

    'Protected Sub btnPrint_Click(sender As Object, e As EventArgs)
    '    litApproveMessage.Text = String.Empty
    '    Dim poid As Integer = Convert.ToInt32(hidCurrentPOID.Value)
    '    Dim po As Ordering.PurchaseOrder = DA.Current.Single(Of Ordering.PurchaseOrder)(poid)
    '    Dim fn As String = PurchaseOrderPDF.CreatePDF(po)
    '    hypPrintIOF.NavigateUrl = String.Format("~/PrintIOF.ashx?POID={0}", poid)
    'End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ApproveAlert.Hide()
        phApprovalList.Visible = True
        panDetail.Visible = False
        LoadData()
    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs)
        Dim poid As Integer = Convert.ToInt32(hidCurrentPOID.Value)
        EmailService.SendRejectEmail(poid, txtRejectReason.Text)
        ApproveAlert.Show("Thank you. Your comments have been sent to the IOF owner.", BootstrapAlertType.Success)
        phReject.Visible = False
        LoadData()
    End Sub

    Protected Sub Repeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim tdApproveRejectItem As HtmlTableCell = CType(e.Item.FindControl("tdApproveRejectItem"), HtmlTableCell)
            tdApproveRejectItem.Visible = IsApprover()
        End If
    End Sub
End Class

Public Class ApprovalListItem
    Public Property POID As Integer
    Public Property ApproverName As String
    Public Property DisplayName As String
    Public Property CreatedDate As DateTime
    Public Property ShortCode As String
    Public Property Total As Double
    Public Property Sort As String
End Class