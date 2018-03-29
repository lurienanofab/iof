Imports System.IO
Imports IOF.Models
Imports IOF.Web.Controls

Public Class PurchaseList
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IOFUtility.Settings.UsePurchaseList Then
            Response.Redirect("~")
        End If

        phPrivMessage.Visible = Not IsPurchaser()

        If Not Page.IsPostBack Then
            If Not String.IsNullOrEmpty(Request.QueryString("filter")) Then
                Dim id As Integer
                If Integer.TryParse(Request.QueryString("filter"), id) Then
                    PurchasingSearch1.CreatorClientID = id
                End If
            End If

            If Session("PurchasingSearchCriteria") IsNot Nothing Then
                Dim crit As PurchaserSearchArgs = CType(Session("PurchasingSearchCriteria"), PurchaserSearchArgs)
                PurchasingSearch1.Criteria = crit
                Session.Remove("PurchasingSearchCriteria")
            Else
                PurchasingSearch1.ClaimStatus = PurchaserClaimStatus.All
                PurchasingSearch1.DateRangePreset = GetDefaultDateRangePreset()
            End If

            LoadData(PurchasingSearch1.Criteria)
        End If
    End Sub

    Private Function GetDefaultDateRangePreset() As String
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PurchasingSearchDefaultDateRange")) Then
            Return ConfigurationManager.AppSettings("PurchasingSearchDefaultDateRange")
        Else
            Return "this-year"
        End If
    End Function

    Protected Sub PurchasingSearch1_Search(ByVal sender As Object, ByVal e As PurchasingSearchEventArgs)
        LoadData(e.Criteria)
    End Sub

    Private Sub LoadData(ByVal args As PurchaserSearchArgs)
        Dim result As SearchResult(Of PurchaserSearchItem) = SearchService.PurchaserSearch(args)
        rptUnclaimed.DataSource = result.Data.OrderByDescending(Function(x) x.CreatedDate)
        rptUnclaimed.DataBind()
        phClaim.Visible = IsPurchaser()
    End Sub

    Private Sub LoadDetail()
        Dim poid As Integer = Convert.ToInt32(hidPOID.Value)

        phSearch.Visible = False
        phDetail.Visible = True

        PurchaseOrderHeaderView1.POID = poid
        PurchaseOrderHeaderView1.DataBind()

        PurchaseOrderDetailView1.POID = poid
        PurchaseOrderDetailView1.DataBind()

        Dim purchaserId As Integer = 0
        Dim purchaserName As String = String.Empty
        Dim reqNum As String = String.Empty
        Dim realPO As String = String.Empty
        Dim purchNotes As String = String.Empty
        Dim order As Order = OrderRepository.Single(poid)
        Dim claimed As Boolean = OrderRepository.IsClaimed(poid, purchaserId, purchaserName, reqNum, realPO, purchNotes)
        Dim isPurchaser = purchaserId = IOFContext.CurrentUser.ClientID

        txtReqNum.Text = reqNum
        txtRealPO.Text = String.Empty 'this should always be empty at this point
        txtPurchaserNotes.Text = purchNotes

        If claimed Then
            btnClaimDetail.Visible = False
            lblClaimedBy.Visible = True
            lblClaimedBy.Text = purchaserName
            txtReqNum.Enabled = isPurchaser
            txtRealPO.Enabled = isPurchaser
            txtPurchaserNotes.Enabled = isPurchaser
            btnSave.Enabled = isPurchaser
            PurchaseOrderAttachments1.ReadOnly = Not isPurchaser
            phCancelOrder.Visible = isPurchaser
            btnCancelOrder.Text = $"Cancel IOF #{poid}"
        Else
            btnClaimDetail.Visible = True
            btnClaimDetail.Enabled =
            lblClaimedBy.Visible = False
            lblClaimedBy.Text = String.Empty
            txtReqNum.Enabled = False
            txtRealPO.Enabled = False
            txtRealPO.Enabled = False
            btnSave.Enabled = False
            PurchaseOrderAttachments1.ReadOnly = True
            phCancelOrder.Visible = False
        End If

        PurchaseOrderAttachments1.POID = poid
        PurchaseOrderAttachments1.LoadAttachments()

        Alert1.Hide()

        lnkPrintIOF.NavigateUrl = $"~/PrintIOF.ashx?POID={poid}"
    End Sub

    Protected Sub Row_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandName = "view" Then
            hidPOID.Value = e.CommandArgument.ToString()
            LoadDetail()
        End If
    End Sub

    Protected Sub btnClaimDetail_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim poid As Integer = Convert.ToInt32(hidPOID.Value)
        OrderRepository.Claim(poid, IOFContext.CurrentUser.ClientID)
        LoadDetail()
    End Sub

    Protected Sub btnClaim_Click(ByVal sender As Object, ByVal e As EventArgs)
        For Each i As RepeaterItem In rptUnclaimed.Items
            Dim chkClaim As CheckBox = CType(i.FindControl("chkClaim"), CheckBox)
            If chkClaim.Checked Then
                Dim hidPOID As HiddenField = CType(i.FindControl("hidPOID"), HiddenField)
                Dim poid As Integer = Integer.Parse(hidPOID.Value)
                OrderRepository.Claim(poid, IOFContext.CurrentUser.ClientID)
            End If
        Next

        LoadData(PurchasingSearch1.Criteria)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        CancelDetail()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim poid As Integer = Convert.ToInt32(hidPOID.Value)
        'Dim po As Ordering.PurchaseOrder = DA.Current.Single(Of Ordering.PurchaseOrder)(poid)
        Dim reqNum As String = txtReqNum.Text
        Dim realPO As String = txtRealPO.Text
        Dim purchNotes As String = txtPurchaserNotes.Text

        OrderRepository.SaveRealPO(poid, reqNum, realPO, purchNotes)

        CancelDetail()
    End Sub

    Private Sub CancelDetail()
        ShowRealPoError(String.Empty)
        phDetail.Visible = False
        phSearch.Visible = True
        hidPOID.Value = String.Empty

        ' A redirect is needed here because the data comes from a view. When the PO is updated with a RealPO
        ' the view will be unchanged until the transaction is committed. By redirecting here this will happen
        ' and the view will have the correct data when the page is reloaded.

        'store the criteria in the session
        Session("PurchasingSearchCriteria") = PurchasingSearch1.Criteria

        'redirect
        Response.Redirect("~/PurchaseList.aspx")
    End Sub

    Protected Sub btnCancelOrder_Click(sender As Object, e As EventArgs)
        Dim poid As Integer = Integer.Parse(hidPOID.Value)

        CancelOrderAlert.Hide()

        If String.IsNullOrEmpty(txtPurchaserNotes.Text) Then
            CancelOrderAlert.Show("You must enter purchaser notes before canceling an order.")
            Return
        End If

        OrderRepository.SaveRealPO(poid, txtReqNum.Text, String.Empty, txtPurchaserNotes.Text)

        OrderRepository.Cancel(poid)

        EmailService.SendCancelOrderEmail(poid, txtPurchaserNotes.Text)

        CancelDetail()
    End Sub

    Protected Sub PurchaseOrderAttachments1_Upload(sender As Object, e As AttachmentEventArgs)
        Dim uploaded As IEnumerable(Of Attachment) = From x In e.Attachments
                                                     Where x.Uploaded
        If uploaded.Count() > 0 Then
            EmailService.SendAddAttachmentsEmail(e.POID, uploaded)
        End If
    End Sub

    Protected Sub PurchaseOrderAttachments1_Delete(sender As Object, e As AttachmentEventArgs)
        Dim att As Attachment = e.Attachments.First()
        If att.Deleted Then
            EmailService.SendDeleteAttachmentEmail(e.POID, att.FileName)
        End If
    End Sub

    Protected Sub btnResendPurchaserEmail_Click(sender As Object, e As EventArgs)
        Alert1.Hide()

        Try
            Dim id As Integer = 0

            If Integer.TryParse(hidPOID.Value, id) Then
                If id > 0 Then
                    Dim filePath As String = PdfService.CreatePDF(id)
                    EmailService.SendPurchaserEmail(id, filePath)
                    Alert1.Show($"Purchaser email sent OK. PDF file created: {Path.GetFileName(filePath)}", BootstrapAlertType.Success)
                Else
                    Throw New Exception(String.Format("Unable to find IOF #{0}", id))
                End If
            Else
                Throw New Exception("Invalid POID: value must be an integer")
            End If
        Catch ex As Exception
            Alert1.Show($"<pre>{ex.ToString()}</pre>")
        End Try
    End Sub

    Protected Function IsClaimed(item As PurchaserSearchItem) As Boolean
        Return item.PurchaserID.HasValue
    End Function

    Private Sub ShowRealPoError(msg As String)
        If String.IsNullOrEmpty(msg) Then
            divRealPoFormGroup.Attributes("class") = "form-group form-group-sm"
            lblRealPOSaveError.Text = msg
            lblRealPOSaveError.Visible = False
        Else
            divRealPoFormGroup.Attributes("class") = "form-group form-group-sm has-error"
            lblRealPOSaveError.Text = msg
            lblRealPOSaveError.Visible = True
        End If
    End Sub
End Class