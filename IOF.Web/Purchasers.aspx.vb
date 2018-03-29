Imports IOF.Models

Public Class Purchasers
    Inherits IOFPage

    Public Property EditIndex As Integer = -1

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadPurchasers()
        End If
    End Sub

    Private Sub LoadPurchasers()
        Dim purchasers As IEnumerable(Of Purchaser) = ClientRepository.GetPurchasers(active:=Nothing)

        If purchasers.Count() = 0 Then
            trNoData.Visible = True
        End If

        rptPurchasers.DataSource = purchasers.OrderBy(Function(x) x.DisplayName)
        rptPurchasers.DataBind()

        Dim available As IEnumerable(Of Purchaser) = ClientRepository.GetAvailablePurchasers()

        If available.Count() = 0 Then
            ddlAvailable.Items.Clear()
            ddlAvailable.Enabled = False
            lbtnAddPurchaser.Visible = False
        Else
            ddlAvailable.Enabled = True
            lbtnAddPurchaser.Visible = True
            ddlAvailable.DataSource = available.OrderBy(Function(x) x.DisplayName)
            ddlAvailable.DataBind()
        End If
    End Sub

    'Protected Sub gvPurchasers_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs) Handles gvPurchasers.RowEditing
    '    If gvPurchasers.DataKeys(e.NewEditIndex).Value IsNot DBNull.Value Then
    '        gvPurchasers.EditIndex = e.NewEditIndex
    '        LoadPurchasers()
    '    Else
    '        e.Cancel = True
    '    End If
    'End Sub

    'Protected Sub gvPurchasers_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs) Handles gvPurchasers.RowCancelingEdit
    '    gvPurchasers.EditIndex = -1
    '    LoadPurchasers()
    'End Sub

    'Protected Sub gvPurchasers_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) 'Handles gvPurchasers.RowCommand
    '    If e.CommandName = "AddPurchaser" Then
    '        Try
    '            lblErr.Text = ""

    '            ' Check if Approver Exists in the list already
    '            'Dim ddlClients As DropDownList = CType(gvPurchasers.FooterRow.FindControl("ddlClients"), DropDownList)
    '            'Dim dt As DataTable = CType(ViewState("Purchasers"), DataTable)
    '            'Dim rows() As DataRow = dt.Select("ClientID=" & ddlClients.SelectedValue)
    '            'If rows.Count > 0 Then
    '            '    lblErr.Text = "You have already added this purchaser."
    '            '    Exit Sub
    '            'End If

    '            'Dim chkNewIsActive As CheckBox = CType(gvPurchasers.FooterRow.FindControl("ckbNewIsActive"), CheckBox)
    '            'ClientRepository.AddOrUpdatePurchaser(Convert.ToInt32(ddlClients.SelectedValue), chkNewIsActive.Checked)
    '            LoadPurchasers()

    '        Catch ex As Exception
    '            lblErr.Text = ex.Message
    '        End Try
    '    End If
    'End Sub

    'Protected Sub gvPurchasers_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs) 'Handles gvPurchasers.RowUpdating
    '    Try
    '        lblErr.Text = ""
    '        'Dim purchaserId As Integer = Convert.ToInt32(gvPurchasers.DataKeys(gvPurchasers.EditIndex).Value)
    '        'Dim chkIsActive As CheckBox = CType(gvPurchasers.Rows(gvPurchasers.EditIndex).FindControl("ckbIsActive"), CheckBox)
    '        'ClientRepository.AddOrUpdatePurchaser(purchaserId, chkIsActive.Checked)
    '        'gvPurchasers.EditIndex = -1
    '        LoadPurchasers()

    '    Catch ex As Exception
    '        lblErr.Text = ex.Message
    '    End Try
    'End Sub

    'Protected Sub gvPurchasers_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles gvPurchasers.RowDeleting
    '    Try
    '        If gvPurchasers.DataKeys(e.RowIndex).Value IsNot DBNull.Value Then
    '            lblErr.Text = ""
    '            Dim purchaserId As Integer = Convert.ToInt32(gvPurchasers.DataKeys(e.RowIndex).Value)
    '            ClientRepository.DeletePurchaser(purchaserId)
    '            LoadPurchasers()
    '        Else
    '            e.Cancel = True
    '        End If
    '    Catch ex As Exception
    '        lblErr.Text = ex.Message
    '    End Try
    'End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        Dim splitter As String() = e.CommandArgument.ToString().Split(":"c)
        Dim arg = New With {.index = Integer.Parse(splitter(0)), .purchaserId = Integer.Parse(splitter(1))}

        If e.CommandName = "edit" Then
            EditIndex = arg.index
        ElseIf e.CommandName = "delete" Then
            ClientRepository.DeletePurchaser(arg.purchaserId)
        ElseIf e.CommandName = "update" Then
            Dim purch As Purchaser = ClientRepository.GetPurchaser(arg.purchaserId)
            Dim item As RepeaterItem = rptPurchasers.Items(arg.index)
            Dim chkActive As CheckBox = CType(item.FindControl("chkActive"), CheckBox)
            ClientRepository.AddOrUpdatePurchaser(purch.ClientID, chkActive.Checked)
            EditIndex = -1
        ElseIf e.CommandName = "cancel" Then
            EditIndex = -1
        End If

        LoadPurchasers()
    End Sub

    Protected Sub lbtnAddPurchaser_Click(sender As Object, e As EventArgs)
        Dim clientId As Integer
        If Integer.TryParse(ddlAvailable.SelectedValue, clientId) Then
            ClientRepository.AddOrUpdatePurchaser(clientId, chkActive.Checked)
            LoadPurchasers()
        End If
    End Sub

    Protected Sub rptPurchasers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            If e.Item.ItemIndex = EditIndex Then
                Dim lblActive As Label = CType(e.Item.FindControl("lblActive"), Label)
                Dim chkActive As CheckBox = CType(e.Item.FindControl("chkActive"), CheckBox)
                Dim lbtnEditUpdate As LinkButton = CType(e.Item.FindControl("lbtnEditUpdate"), LinkButton)
                Dim lbtnDeleteCancel As LinkButton = CType(e.Item.FindControl("lbtnDeleteCancel"), LinkButton)

                lblActive.Visible = False
                chkActive.Visible = True

                lbtnEditUpdate.CommandName = "update"
                lbtnEditUpdate.Text = "Update"
                lbtnDeleteCancel.CommandName = "cancel"
                lbtnDeleteCancel.Text = "Cancel"
            End If
        End If
    End Sub
End Class