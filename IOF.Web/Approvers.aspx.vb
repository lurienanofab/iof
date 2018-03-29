Public Class Approvers
    Inherits IOFPage

    Public Property EditIndex As Integer = -1

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadApprovers()
        End If
    End Sub

    Private Sub LoadApprovers()
        Alert1.Hide()

        Dim clientId As Integer = IOFContext.CurrentUser.ClientID

        rptApprovers.DataSource = ClientRepository.GetActiveApprovers(clientId).OrderBy(Function(x) x.DisplayName)
        rptApprovers.DataBind()

        trNoData.Visible = rptApprovers.Items.Count = 0

        ddlClients.DataSource = ClientRepository.GetAvailableApprovers(clientId).OrderBy(Function(x) x.DisplayName)
        ddlClients.DataBind()

        If ddlClients.Items.Count = 0 Then
            ddlClients.Enabled = False
            lbtnAddApprover.Visible = False
        Else
            ddlClients.Enabled = True
            lbtnAddApprover.Visible = True
        End If
    End Sub

    Protected Sub Row_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Alert1.Hide()

        Try
            If e.CommandName = "add" Then
                If ddlClients.Items.Count > 0 Then
                    Dim approverId As Integer = Convert.ToInt32(ddlClients.SelectedValue)
                    ClientRepository.AddOrUpdateApprover(IOFContext.CurrentUser.ClientID, approverId, chkNewIsPrimary.Checked)
                    LoadApprovers()
                End If
            ElseIf e.CommandName = "edit" Then
                EditIndex = Convert.ToInt32(e.CommandArgument)
                LoadApprovers()
            ElseIf e.CommandName = "delete" Then
                Dim approverId As Integer = Convert.ToInt32(e.CommandArgument)
                ClientRepository.DeleteApprover(IOFContext.CurrentUser.ClientID, approverId)
                LoadApprovers()
            ElseIf e.CommandName = "update" Then
                Dim args As String() = e.CommandArgument.ToString().Split(Char.Parse(":"))
                Dim index As Integer = Integer.Parse(args(0))
                Dim approverId As Integer = Integer.Parse(args(1))
                Dim item = rptApprovers.Items(index)

                Dim chkIsPrimary As CheckBox = CType(item.FindControl("chkIsPrimary"), CheckBox)
                ClientRepository.AddOrUpdateApprover(IOFContext.CurrentUser.ClientID, approverId, chkIsPrimary.Checked)

                EditIndex = -1
                LoadApprovers()
            ElseIf e.CommandName = "cancel" Then
                EditIndex = -1
                LoadApprovers()
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Protected Sub rptApprovers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            If e.Item.ItemIndex = EditIndex Then
                Dim lblIsPrimary As Label = CType(e.Item.FindControl("lblIsPrimary"), Label)
                Dim chkIsPrimary As CheckBox = CType(e.Item.FindControl("chkIsPrimary"), CheckBox)
                Dim phEditDelete As PlaceHolder = CType(e.Item.FindControl("phEditDelete"), PlaceHolder)
                Dim phUpdateCancel As PlaceHolder = CType(e.Item.FindControl("phUpdateCancel"), PlaceHolder)

                lblIsPrimary.Visible = False
                chkIsPrimary.Visible = True

                phEditDelete.Visible = False
                phUpdateCancel.Visible = True
            End If
        End If
    End Sub
End Class