Imports IOF.Models
Imports IOF.Web.Controls

Public Class OrderedIOF
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
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

            If Not IsPurchaser() Then
                PurchasingSearch1.CreatorClientID = IOFContext.CurrentUser.ClientID
            End If

            PurchasingSearch1.DateRangePreset = "this-month"

            LoadData(PurchasingSearch1.Criteria)
        End If
    End Sub

    Protected Sub PurchasingSearch1_Search(ByVal sender As Object, ByVal e As PurchasingSearchEventArgs)
        LoadData(e.Criteria)
    End Sub

    Private Sub LoadData(ByVal args As PurchaserSearchArgs)
        Dim result As SearchResult(Of PurchaserSearchItem) = SearchService.PurchaserSearch(args)
        rptOrdered.DataSource = result.Data.OrderByDescending(Function(x) x.CreatedDate)
        rptOrdered.DataBind()
    End Sub

    Private Sub LoadDetail()

        txtRealPO.Text = String.Empty

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
        Dim isPurchaser As Boolean = MyBase.IsPurchaser()

        OrderRepository.IsClaimed(poid, purchaserId, purchaserName, reqNum, realPO, purchNotes)

        lblClaimedBy.Text = purchaserName
        txtReqNum.Text = reqNum
        txtReqNum.Enabled = isPurchaser
        txtRealPO.Text = realPO
        txtRealPO.Enabled = isPurchaser
        txtPurchaserNotes.Text = purchNotes
        txtPurchaserNotes.Enabled = isPurchaser
        btnSave.Enabled = isPurchaser

        PurchaseOrderAttachments1.POID = poid
        PurchaseOrderAttachments1.ReadOnly = Not isPurchaser
        PurchaseOrderAttachments1.LoadAttachments()

        hypPrintIOF.NavigateUrl = String.Format("~/PrintIOF.ashx?POID={0}", poid)
        phCancelOrder.Visible = isPurchaser
        btnCancelOrder.Text = "Cancel IOF #" + poid.ToString()
    End Sub

    Protected Sub Row_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandName = "view" Then
            hidPOID.Value = e.CommandArgument.ToString()
            LoadDetail()
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim poid As Integer = Convert.ToInt32(hidPOID.Value)
        Dim reqNum As String = txtReqNum.Text
        Dim realPO As String = txtRealPO.Text
        Dim purchNotes As String = txtPurchaserNotes.Text

        ' on this page the po has already been ordered so never let the realPO be empty
        If Not String.IsNullOrEmpty(realPO) Then
            OrderRepository.SaveRealPO(poid, reqNum, realPO, purchNotes)
            CancelDetail()
        Else
            ShowRealPoError("Please enter the Real PO#")
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        CancelDetail()
    End Sub

    Private Sub CancelDetail()
        ShowRealPoError(String.Empty)
        phDetail.Visible = False
        phSearch.Visible = True
        hidPOID.Value = String.Empty
        LoadData(PurchasingSearch1.Criteria)
    End Sub

    Protected Sub btnCancelOrder_Click(sender As Object, e As EventArgs)
        Dim poid As Integer = Integer.Parse(hidPOID.Value)

        CancelOrderAlert.Hide()

        If String.IsNullOrEmpty(txtPurchaserNotes.Text) Then
            CancelOrderAlert.Show("You must enter purchaser notes before canceling an order.")
            Return
        End If

        OrderRepository.SaveRealPO(poid, txtReqNum.Text, txtRealPO.Text, txtPurchaserNotes.Text)

        OrderRepository.Cancel(poid)

        EmailService.SendCancelOrderEmail(poid, txtPurchaserNotes.Text)

        CancelDetail()
    End Sub

    Protected Sub PurchaseOrderAttachments1_Upload(sender As Object, e As AttachmentEventArgs)
        Dim uploaded As IList(Of Attachment) = e.Attachments.Where(Function(x) x.Uploaded).ToList()

        If uploaded.Count > 0 Then
            EmailService.SendAddAttachmentsEmail(e.POID, uploaded)
        End If
    End Sub

    Protected Sub PurchaseOrderAttachments1_Delete(sender As Object, e As AttachmentEventArgs)
        Dim poa As Attachment = e.Attachments.First()

        If poa.Deleted Then
            EmailService.SendDeleteAttachmentEmail(e.POID, poa.FileName)
        End If
    End Sub

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