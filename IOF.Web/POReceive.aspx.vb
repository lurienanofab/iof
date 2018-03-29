Public Class POReceive
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        rptReceive.DataSource = {New With {.POID = GetPOID()}}
        rptReceive.DataBind()
    End Sub

    Protected Function GetPOID() As Integer
        If Not String.IsNullOrEmpty(Request.QueryString("poid")) Then
            Dim result As Integer
            If Integer.TryParse(Request.QueryString("poid"), result) Then
                Return result
            End If
        End If
        Return 0
    End Function
End Class