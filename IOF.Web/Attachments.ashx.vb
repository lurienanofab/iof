Public Class Attachments
    Inherits AttachmentsHandler

    Protected Overrides Sub SetParameters(context As HttpContext)
        POID = 0
        FileName = String.Empty

        If Not String.IsNullOrEmpty(context.Request.QueryString("POID")) Then
            Dim _poid As Integer
            If Not Integer.TryParse(context.Request.QueryString("POID"), _poid) Then
                Throw New HttpException(500, "Error: invalid parameter POID")
            Else
                POID = _poid
            End If
        End If

        FileName = HttpUtility.UrlDecode(context.Request.QueryString("FileName"))
    End Sub
End Class