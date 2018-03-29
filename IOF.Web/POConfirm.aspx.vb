Imports IOF.Models

Public Class POConfirm
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ClearPOMessage()
        If Not Page.IsPostBack Then
            If Action = "Copy" Then
                CopyPO()
            Else
                LoadPO()
                PrivCheck()
            End If
        End If
    End Sub

    Private Sub ClearPOMessage()
        phPOMessage.Visible = False
        litPOMessage.Text = String.Empty
    End Sub

    Private Sub SetPOMessage(msg As String)
        If String.IsNullOrEmpty(msg) Then
            phPOMessage.Visible = False
            litPOMessage.Text = String.Empty
        Else
            phPOMessage.Visible = True
            litPOMessage.Text = msg
        End If
    End Sub

    Private Sub SetPOMessage(ex As Exception)
        SetPOMessage($"{ex.Message}<hr /><pre>{ex.StackTrace}</pre>")
    End Sub

    Private Sub HideAllButtons()
        btnDelete.Visible = False
        btnKeepDraft.Visible = False
        hypPrintIOF.Visible = False
        btnMarkCompleteAndPrint.Visible = False
        btnSendToApprover.Visible = False
        btnSendToPurchaser.Visible = False
        btnSetAccountOK.Visible = False
    End Sub

    Private Sub PrivCheck()
        If PurchaseOrderHeaderView1.Order Is Nothing Then
            HideAllButtons()
            Return
        End If

        If User.IsInRole("Staff") And PurchaseOrderDetailView1.RowCount > 0 Then
            ' For Staff, display in all states
            hypPrintIOF.Visible = True
        Else
            ' For regular users, display if Status is Completed
            hypPrintIOF.Visible = (PurchaseOrderHeaderView1.Order.StatusID = Status.Completed AndAlso PurchaseOrderDetailView1.RowCount > 0)
        End If

        hypPrintIOF.Text = "Print IOF"
        If hypPrintIOF.Visible AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft AndAlso Not String.IsNullOrEmpty(PurchaseOrderHeaderView1.AccountName) Then
            hypPrintIOF.Text = "Print IOF and Keep as Draft"
        End If

        Dim currentUser As Client = IOFContext.CurrentUser

        btnSendToApprover.Visible = (currentUser.ClientID = PurchaseOrderHeaderView1.Order.ClientID _
                                     AndAlso IsStaff() _
                                     AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft _
                                     AndAlso PurchaseOrderDetailView1.RowCount > 0 _
                                     AndAlso Not String.IsNullOrEmpty(PurchaseOrderHeaderView1.AccountName))

        btnSendToPurchaser.Visible = (currentUser.ClientID = PurchaseOrderHeaderView1.Order.ClientID _
                                      AndAlso IsApprover() _
                                      AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft _
                                      AndAlso PurchaseOrderDetailView1.RowCount > 0 _
                                      AndAlso Not String.IsNullOrEmpty(PurchaseOrderHeaderView1.AccountName))

        If PurchaseOrderHeaderView1.Order.ClientID = PurchaseOrderHeaderView1.Order.ApproverID Then
            btnSendToApprover.Visible = False
        Else
            btnSendToPurchaser.Visible = False
        End If

        btnMarkCompleteAndPrint.Visible = (currentUser.ClientID = PurchaseOrderHeaderView1.Order.ClientID _
                           AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft _
                           AndAlso PurchaseOrderDetailView1.RowCount > 0) 'Temporary

        btnKeepDraft.Visible = (currentUser.ClientID = PurchaseOrderHeaderView1.Order.ClientID _
                                AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft _
                                AndAlso Not String.IsNullOrEmpty(PurchaseOrderHeaderView1.AccountName))

        btnDelete.Visible = (currentUser.ClientID = PurchaseOrderHeaderView1.Order.ClientID _
                             AndAlso PurchaseOrderHeaderView1.Order.StatusID = Status.Draft)
    End Sub

    Private Sub LoadPO()
        If POID > 0 Then
            If Action = "Copy" Then
                PurchaseOrderHeaderView1.CopyPOID = POID
            ElseIf Action = "UseExisting" AndAlso FromPOID > 0 Then
                PurchaseOrderHeaderView1.CopyPOID = FromPOID
            End If

            Dim clientId As Integer = IOFContext.CurrentUser.ClientID

            PurchaseOrderHeaderView1.POID = POID
            PurchaseOrderHeaderView1.DataBind()
            PurchaseOrderHeaderView1.EnableLink = (clientId = PurchaseOrderHeaderView1.Order.ClientID) AndAlso (PurchaseOrderHeaderView1.Order.StatusID = Status.Draft)

            PurchaseOrderDetailView1.POID = POID
            PurchaseOrderDetailView1.DataBind()
            PurchaseOrderDetailView1.EnableTitleLink = (clientId = PurchaseOrderHeaderView1.Order.ClientID) AndAlso (PurchaseOrderHeaderView1.Order.StatusID = Status.Draft)

            StoreManager = PurchaseOrderHeaderView1.Vendor.ClientID = 0

            PurchaseOrderAttachments1.POID = POID
            PurchaseOrderAttachments1.LoadAttachments()

            hypPrintIOF.NavigateUrl = $"~/PrintIOF.ashx?POID={POID}"

            If Action = "Search" Then
                hypReturn.Visible = True

                If ReturnTo = "Track" Then
                    hypReturn.NavigateUrl = $"~/TrackIOF.aspx?Action=Detail&POID={POID}"
                Else
                    hypReturn.NavigateUrl = $"~/SearchIOF.aspx?Action=Search"
                End If
            Else
                hypReturn.Visible = False
            End If
        End If
    End Sub

    Private Sub CopyPO()

        If POID = 0 Then Return

        Dim order As Order = OrderRepository.Single(POID)

        Dim copyPOID As Integer = CopyPO(POID)

        If copyPOID = 0 Then
            phConfirm.Visible = False
            panSetAccount.Visible = True
            hidSetAccountPOID.Value = order.POID.ToString()
            litShortCode.Text = AccountRepository.GetShortCode(order.AccountID)
            ddlSetAccount.DataSource = CurrentUserAccounts
            ddlSetAccount.DataBind()
        Else
            Response.Redirect($"~/POConfirm.aspx?Action=UseExisting&POID={copyPOID}&FromPOID={POID}")
        End If
    End Sub

    Private Function CopyPO(origPOID As Integer) As Integer
        Dim result As Integer = 0
        Dim origPO As Order = OrderRepository.Single(origPOID)
        If origPO.AccountID IsNot Nothing Then
            Dim accts As IEnumerable(Of Account) = CurrentUserAccounts.Where(Function(x) x.AccountID = origPO.AccountID.Value)
            If accts.Count > 0 Then
                Dim copy As Order = OrderRepository.Copy(origPOID)
                result = copy.POID
            End If
        End If
        Return result
    End Function

    Protected Sub btnSetAccountOK_Click(sender As Object, e As EventArgs)
        Dim origPOID As Integer = Convert.ToInt32(hidSetAccountPOID.Value)
        Dim copy As Order = OrderRepository.Copy(origPOID, Convert.ToInt32(ddlSetAccount.SelectedValue))
        If copy.POID <> 0 Then
            Response.Redirect($"~/POConfirm.aspx?Action=UseExisting&POID={copy.POID}&FromPOID={origPOID}")
        Else
            SetPOMessage($"Failed to create PO using account {ddlSetAccount.SelectedItem.Text}")
        End If
    End Sub

    Protected Sub PurchaseOrderHeader1_Command(sender As Object, e As CommandEventArgs)
        Dim act As String = If(Action = "Copy", "UseExisting", Action)
        Dim url As String = $"~/POInfo.aspx?Action={act}&POID={e.CommandArgument}&FromPOID={FromPOID}&ReturnTo=Confirm"
        Response.Redirect(url)
    End Sub

    Protected Sub PurchaseOrderDetail1_TitleCommand(sender As Object, e As CommandEventArgs)
        Dim act As String = If(Action = "Copy", "UseExisting", Action)
        Dim url As String = $"~/POItems.aspx?Action={act}&POID={e.CommandArgument}&FromPOID={FromPOID}&ReturnTo=Confirm"
        Response.Redirect(url)
    End Sub

    Protected Sub btnSendToApprover_Click(sender As Object, e As EventArgs) Handles btnSendToApprover.Click
        Try
            LoadPO()

            ' Send Email to Approver
            EmailService.SendApproverEmail(POID)

            SetPOMessage($"Thank you. IOF #{POID} has been sent to the approver!")

            ' Update PO Status to "Awaiting Approval"
            OrderRepository.RequestApproval(POID)

            LoadPO()

            PrivCheck()
        Catch ex As Exception
            SetPOMessage(ex)
        End Try
    End Sub

    Protected Sub btnSendToPurchaser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSendToPurchaser.Click
        Try
            LoadPO()

            ' Approve PO - Update Status to Approved
            OrderRepository.Approve(POID, IOFContext.CurrentUser.ClientID)

            ' Send email to purchaser
            Dim filePath As String = PdfService.CreatePDF(POID)
            EmailService.SendPurchaserEmail(POID, filePath)
            SetPOMessage($"Thank you. IOF #{POID} has been sent to the purchaser!")

            LoadPO()
            PrivCheck()
        Catch ex As Exception
            SetPOMessage(ex)
        End Try
    End Sub

    Protected Sub btnMarkCompleteAndPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMarkCompleteAndPrint.Click
        Try
            ' Changes Status to Completed and Print PDF
            If POID > 0 Then
                LoadPO()

                OrderRepository.ManuallyProcess(PurchaseOrderHeaderView1.Order.POID)

                SetPOMessage($"Thank you. IOF #{POID} has been manually processed.")

                RegisterPrintScript()

                LoadPO()

                PrivCheck()
            Else
                Throw New Exception("Invalid POID")
            End If
        Catch ex As Exception
            SetPOMessage(ex)
        End Try
    End Sub

    Private Sub RegisterPrintScript()
        Page.ClientScript.RegisterClientScriptBlock([GetType](), "print_iof", $"window.open('{VirtualPathUtility.ToAbsolute("~/PrintIOF.ashx")}?POID={POID}', 'print_iof')", True)
    End Sub

    Protected Sub btnKeepDraft_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnKeepDraft.Click
        ' Since Status is already Draft, just redirect user to home page
        Response.Redirect("~")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click
        Try
            If OrderRepository.DeleteDraft(POID) Then
                Response.Redirect("~")
            Else
                SetPOMessage("The PO cannot be deleted because it is not a draft.")
            End If
        Catch ex As Exception
            SetPOMessage(ex)
        End Try
    End Sub

End Class