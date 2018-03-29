Imports System.IO
Imports StructureMap.Attributes

Public Class PrintIOF
    Inherits IOFHandler

    <SetterProperty>
    Public Property PdfService As IPdfService

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        Dim poid As Integer = GetPOID(context)
        Dim filePath As String = PdfService.CreatePDF(poid)
        context.Response.ContentType = "application/pdf"
        context.Response.AddHeader("Content-Disposition", String.Format("inline;filename=""{0}""", Path.GetFileName(filePath)))
        context.Response.WriteFile(filePath)
    End Sub

    Private Function GetPOID(context As HttpContext) As Integer
        If Not String.IsNullOrEmpty(context.Request.QueryString("POID")) Then
            Dim result As Integer = 0
            If Integer.TryParse(context.Request.QueryString("POID"), result) Then
                Return result
            Else
                Throw New InvalidOperationException("Invalid querystring parameter: POID")
            End If
        Else
            Throw New InvalidOperationException("Missing querystring parameter: POID")
        End If
    End Function
End Class