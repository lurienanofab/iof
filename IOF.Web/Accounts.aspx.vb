Public Class Accounts
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadAccounts()
        End If
    End Sub

    Private Sub LoadAccounts()
        Alert1.Hide()

        Dim clientId As Integer = IOFContext.CurrentUser.ClientID

        rptAccounts.DataSource = AccountRepository.GetActiveAccounts(clientId).OrderBy(Function(x) x.AccountName)
        rptAccounts.DataBind()

        trNoData.Visible = rptAccounts.Items.Count = 0

        ddlAccounts.DataSource = AccountRepository.GetAvailableAccounts(clientId).OrderBy(Function(x) x.AccountName)
        ddlAccounts.DataBind()

        If ddlAccounts.Items.Count = 0 Then
            ddlAccounts.Enabled = False
            lbtnAddAccount.Visible = False
        Else
            ddlAccounts.Enabled = True
            lbtnAddAccount.Visible = True
        End If
    End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        Try
            If e.CommandName = "add" Then
                If ddlAccounts.Items.Count > 0 Then
                    Dim accountId As Integer = Convert.ToInt32(ddlAccounts.SelectedValue)
                    AccountRepository.AddOrUpdate(IOFContext.CurrentUser.ClientID, accountId)
                    LoadAccounts()
                End If
            ElseIf e.CommandName = "delete" Then
                Dim accountId As Integer = Convert.ToInt32(e.CommandArgument)
                AccountRepository.Delete(IOFContext.CurrentUser.ClientID, accountId)
                LoadAccounts()
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub
End Class