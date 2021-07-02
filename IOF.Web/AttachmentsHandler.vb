Imports Newtonsoft.Json

Public Class AttachmentsHandler
    Inherits IOFHandler

    Public Property POID As Integer

    Public Property FileName As String

    Public Overrides Sub ProcessRequest(context As HttpContext)
        Try
            SetParameters(context)
            HandleRequest(context)
        Catch httpEx As HttpException
            WriteError(context, httpEx.Message, httpEx.GetHttpCode())
        Catch ex As Exception
            WriteError(context, ex.ToString())
        End Try
    End Sub

    Protected Overridable Sub SetParameters(context As HttpContext)
        Dim path As String = context.Request.Url.AbsolutePath

        POID = 0
        FileName = String.Empty

        Dim matches As MatchCollection = Regex.Matches(path, "attachments/([0-9|a-z|A-Z]+)/?(.+)?")

        If matches.Count > 0 Then
            Dim match = matches(0)

            ' match.Groups[0] Is the entire string

            If match.Groups(1).Success Then
                Dim _POID As Integer
                If Not Integer.TryParse(match.Groups(1).Value, _POID) Then
                    Throw New HttpException(500, "Error: invalid parameter POID")
                Else
                    POID = _POID
                End If

                If match.Groups(2).Success Then
                    FileName = HttpUtility.UrlDecode(match.Groups(2).Value)
                End If
            End If
        End If
    End Sub

    Private Sub HandleRequest(context As HttpContext)
        If POID = 0 Then
            WriteFolders(context)
        Else
            If String.IsNullOrEmpty(FileName) Then
                WriteFolder(context, POID)
            Else
                WriteFile(context, POID, FileName)
            End If
        End If
    End Sub

    Private Sub WriteError(context As HttpContext, errmsg As String, Optional statusCode As Integer = 500)
        context.Response.StatusCode = statusCode
        context.Response.ContentType = "text/plain"
        context.Response.Write(errmsg)
    End Sub

    Private Sub WriteFolders(context As HttpContext)
        Dim folders As IEnumerable(Of Integer) = AttachmentService.GetFolders()
        context.Response.ContentType = "application/json"
        context.Response.Write(JsonConvert.SerializeObject(New With {folders}))
    End Sub

    Private Sub WriteFolder(context As HttpContext, poid As Integer)
        Dim attachments = From x In AttachmentService.GetAttachments(poid)
                          Select New With {x.FileName, x.Url}
        context.Response.ContentType = "application/json"
        context.Response.Write(JsonConvert.SerializeObject(New With {attachments}))
    End Sub

    Private Sub WriteFile(context As HttpContext, poid As Integer, fileName As String)
        If AttachmentService.Exists(poid, fileName) Then
            Dim buffer As Byte() = AttachmentService.GetBytes(poid, fileName)
            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName)
            context.Response.AddHeader("Content-Disposition", "inline; filename=" + fileName)
            context.Response.BinaryWrite(buffer)
        Else
            Throw New HttpException(404, String.Format("Error: file not found {0}", fileName))
        End If
    End Sub
End Class
