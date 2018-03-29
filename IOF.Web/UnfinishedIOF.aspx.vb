Imports IOF.Models

Public Class UnfinishedIOF
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadPO()
        End If
    End Sub

    Private Sub LoadPO()
        Dim items As IEnumerable(Of Order) = OrderRepository.GetDrafts(IOFContext.CurrentUser.ClientID).OrderByDescending(Function(x) x.CreatedDate)

        rptDraft.DataSource = items
        rptDraft.DataBind()

        phNoData.Visible = items.Count() <= 0
        rptDraft.Visible = items.Count() > 0
    End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        If e.CommandName = "delete" Then
            Dim poid As Integer = Convert.ToInt32(e.CommandArgument)
            OrderRepository.DeleteDraft(poid)
            LoadPO()
        End If
    End Sub
End Class