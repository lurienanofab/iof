Imports IOF.Models

Public Class POInfo
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Alert1.Hide()
        lblErrAccount.Text = String.Empty
        lblErrApprover.Text = String.Empty

        If Not Page.IsPostBack Then
            txtNeededDate.Text = DateTime.Now.AddDays(7).Date.ToString("MM/dd/yyyy")
            phStoreManager.Visible = IsStoreManager()
            chkStoreManager.Checked = StoreManager
            LoadAccounts()
            LoadApprovers()
            LoadVendors()
            LoadPOInfo()
        End If

        txtNeededDate.Attributes.Add("placeholder", "MM/DD/YYYY")
    End Sub

    Private Sub LoadAccounts()
        Dim items As IEnumerable(Of ListItem) = AccountSelectItems()
        ddlAccount.DataSource = items
        ddlAccount.DataBind()
        ddlAccount.Enabled = items.Count() > 0
    End Sub

    Private Sub LoadApprovers()
        Dim items As IEnumerable(Of ListItem) = ApproverSelectItems()
        ddlApprover.DataSource = items
        ddlApprover.DataBind()
        ddlApprover.Enabled = items.Count() > 0
    End Sub

    Private Function IsNoAccountClient() As Boolean
        Return IOFUtility.Settings.NoAccountClients.Contains(IOFContext.CurrentUser.ClientID)
    End Function

    ''' <summary>
    ''' Gets a collection of SelectItem objects for a given list of accounts.
    ''' Also "No Account" is added to the top of list (if the user has permission), so users can create temporary IOFs.
    ''' </summary>
    Public Function AccountSelectItems() As IEnumerable(Of ListItem)
        Dim result As New List(Of ListItem)

        If IsNoAccountClient() Then
            result.Add(New ListItem("-- No Account --", "-1"))
        End If

        result.AddRange(From x In CurrentUserAccounts
                        Order By x.AccountName
                        Select New ListItem(x.AccountDisplayName, x.AccountID.ToString()))

        Return result
    End Function

    Public Function ApproverSelectItems() As IEnumerable(Of ListItem)
        Dim approvers As IEnumerable(Of Approver) = ClientRepository.GetActiveApprovers(IOFContext.CurrentUser.ClientID)
        Dim primaryAppr As Approver = approvers.FirstOrDefault(Function(x) x.IsPrimary)

        Dim result As New List(Of ListItem)

        If primaryAppr IsNot Nothing Then
            result.Add(New ListItem(primaryAppr.DisplayName, primaryAppr.ApproverID.ToString()))
        End If

        result.AddRange(From x In approvers
                        Where Not x.IsPrimary
                        Order By x.LName, x.FName
                        Select New ListItem(x.DisplayName, x.ApproverID.ToString()))

        Return result
    End Function

    Private Sub LoadVendors()
        If POID = 0 Then
            ddlVendor.Items.Clear()
            ddlVendor.DataSource = VendorRepository.GetActiveVendors(ActiveClientID).OrderBy(Function(x) x.VendorName)
            ddlVendor.DataBind()
            ddlVendor.Items.Insert(0, New ListItem(String.Empty, String.Empty))

            phVendorName.Visible = False
            litVendorName.Text = String.Empty

            phVendorList.Visible = ddlVendor.Items.Count > 0
            lbtnAddVendor.Visible = True
            lbtnEditVendor.Visible = (ddlVendor.Items.Count > 1)
        Else
            phVendorName.Visible = True
            phVendorList.Visible = False
        End If
    End Sub

    Private Sub LoadPOInfo()
        If POID > 0 Then
            Dim po As Order = OrderRepository.Single(POID)
            Dim ven As Vendor = VendorRepository.Single(po.VendorID)
            If po IsNot Nothing Then
                litVendorName.Text = ven.VendorName
                If ven.ClientID = 0 Then
                    StoreManager = True
                    chkStoreManager.Checked = True
                    chkStoreManager.Attributes.Add("disabled", "disabled")
                End If
                ddlVendor.SelectedValue = ven.VendorID.ToString()
                If po.AccountID <= 0 Then
                    ddlAccount.SelectedIndex = 0
                Else
                    If po.AccountID IsNot Nothing Then
                        If ddlAccount.Items.FindByValue(po.AccountID.Value.ToString()) IsNot Nothing Then
                            ddlAccount.SelectedValue = po.AccountID.Value.ToString()
                        End If
                    End If
                End If
                ddlApprover.SelectedValue = po.ApproverID.ToString()
                txtNeededDate.Text = po.NeededDate.ToShortDateString()
                chkOversized.Checked = po.Oversized
                ddlShippingMethod.SelectedValue = po.ShippingMethodID.ToString()
                txtNotes.Text = po.Notes
                chkAttention.Checked = po.Attention.GetValueOrDefault()
            End If
        End If
    End Sub

    Protected Sub VendorInfo1_AddClick(ByVal sender As Object, ByVal e As EventArgs)
        LoadVendors()
        ddlVendor.SelectedValue = VendorInfo1.VendorID.ToString()
        VendorInfo1.Visible = False
        pOrderInfo.Visible = True
    End Sub

    Protected Sub VendorInfo1_UpdateClick(ByVal sender As Object, ByVal e As EventArgs)
        LoadVendors()
        ddlVendor.SelectedValue = VendorInfo1.VendorID.ToString()
        VendorInfo1.Visible = False
        pOrderInfo.Visible = True
    End Sub

    Protected Sub VendorInfo1_CancelClick(ByVal sender As Object, ByVal e As EventArgs)
        VendorInfo1.Visible = False
        pOrderInfo.Visible = True
    End Sub

    Protected Overrides Sub OnStoreManagerCheckChanged(checked As Boolean)
        LoadVendors()
    End Sub

    Protected Sub lbtnAddAccount_Click(ByVal sender As Object, ByVal e As EventArgs)
        pAccount.Visible = True
        pOrderInfo.Visible = False
    End Sub

    Protected Sub btnAddAccount_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim accountId As Integer = Convert.ToInt32(ddlAccounts.SelectedValue)
            AccountRepository.AddOrUpdate(IOFContext.CurrentUser.ClientID, accountId)
            LoadAccounts()
            ddlAccount.SelectedValue = accountId.ToString()
            pAccount.Visible = False
            pOrderInfo.Visible = True
        Catch ex As Exception
            lblErrAccount.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnAccountCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        pAccount.Visible = False
        pOrderInfo.Visible = True
    End Sub

    Protected Sub lbtnAddApprover_Click(ByVal sender As Object, ByVal e As EventArgs)
        pApprover.Visible = True
        pOrderInfo.Visible = False
    End Sub

    Protected Sub btnApproverCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        pApprover.Visible = False
        pOrderInfo.Visible = True
    End Sub

    Protected Sub btnAddApprover_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim approverId As Integer = Convert.ToInt32(ddlApprovers.SelectedValue)

            If approverId = 0 Then
                Throw New Exception("Please select an approver.")
            End If

            ClientRepository.AddOrUpdateApprover(IOFContext.CurrentUser.ClientID, approverId, False)
            LoadApprovers()
            ddlApprover.SelectedValue = approverId.ToString()
            pApprover.Visible = False
            pOrderInfo.Visible = True
        Catch ex As Exception
            lblErrApprover.Text = ex.Message
        End Try
    End Sub

    Protected Sub lbtnAddVendor_Click(ByVal sender As Object, ByVal e As EventArgs)
        VendorInfo1.VendorID = 0
        VendorInfo1.DataBind()
        VendorInfo1.Visible = True
        pOrderInfo.Visible = False
    End Sub

    Protected Sub lbtnEditVendor_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Not String.IsNullOrEmpty(ddlVendor.SelectedValue) Then
            VendorInfo1.VendorID = Convert.ToInt32(ddlVendor.SelectedValue)
            VendorInfo1.DataBind()
            VendorInfo1.Visible = True
            pOrderInfo.Visible = False
        End If
    End Sub

    Protected Sub BtnSavePOInfo_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Alert1.Hide()

            ' Check Vendor Info
            If ddlVendor.SelectedIndex < 1 AndAlso String.IsNullOrEmpty(litVendorName.Text) Then
                Alert1.Show("You must select or add a vendor.")
                Exit Sub
            End If

            ' Check Account
            Dim accountId As Integer? = GetAccountID()
            If Not IsNoAccountClient() AndAlso Not accountId.HasValue Then
                Alert1.Show("You must select or add an account.")
                Exit Sub
            End If

            ' Check Approver
            If ddlApprover.SelectedIndex = -1 Then
                Alert1.Show("You must select an approver.")
                Exit Sub
            End If

            Dim dateNeeded As DateTime
            If Not Date.TryParse(txtNeededDate.Text, dateNeeded) Then
                Alert1.Show("Invalid Date Needed value.")
                Exit Sub
            End If

            Dim thisPOID As Integer
            If POID = 0 Then
                Dim po = OrderRepository.Add(IOFContext.CurrentUser.ClientID,
                                    Convert.ToInt32(ddlVendor.SelectedValue),
                                    accountId,
                                    Convert.ToInt32(ddlApprover.SelectedValue),
                                    Convert.ToDateTime(txtNeededDate.Text),
                                    chkOversized.Checked,
                                    Convert.ToInt32(ddlShippingMethod.SelectedValue),
                                    txtNotes.Text,
                                    chkAttention.Checked)

                If po.POID = 0 Then
                    Throw New Exception("Error: Insert failed, invalid POID returned.")
                End If

                thisPOID = po.POID
            Else
                OrderRepository.Update(POID, GetAccountID(), Convert.ToInt32(ddlApprover.SelectedValue), Convert.ToDateTime(txtNeededDate.Text), chkOversized.Checked, Convert.ToInt32(ddlShippingMethod.SelectedValue), txtNotes.Text, chkAttention.Checked)
                thisPOID = POID
            End If

            If ReturnTo = "Confirm" Then
                Response.Redirect($"~/POConfirm.aspx?Action={Action}&POID={thisPOID}&FromPOID={FromPOID}")
            Else
                Response.Redirect($"~/POItems.aspx?Action={Action}&POID={thisPOID}")
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Private Function GetAccountID() As Integer?
        If ddlAccount.Items.Count = 0 Then
            Return Nothing
        End If

        Dim val As Integer = Convert.ToInt32(ddlAccount.SelectedValue)

        If val > 0 Then
            Return val
        Else
            Return Nothing
        End If
    End Function
End Class