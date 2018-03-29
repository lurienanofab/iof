Imports Newtonsoft.Json

Namespace Ajax
    Public Class Purchaser
        Inherits IOFHandler

        Public Overrides Sub ProcessRequest(context As HttpContext)
            context.Response.ContentType = "application/json"
            Try
                If context.Request.Form("command") = "update" Then
                    Dim podid As Integer = 0
                    Dim partNum As String = String.Empty
                    Dim quantity As Double = 0
                    Dim unitPrice As Double = 0

                    If TryGetEditValues(context, podid, partNum, quantity, unitPrice) Then
                        Dim changes As IEnumerable(Of String) = DetailRepository.PurchaserUpdate(podid, partNum, quantity, unitPrice, UpdateInventory(context))

                        If changes.Count() > 0 Then
                            EmailService.SendItemModifiedEmail(podid, changes)
                        End If

                        context.Response.Write(JsonConvert.SerializeObject(New With {.Success = True, .Changes = changes.Count()}))
                    Else
                        context.Response.Write(JsonConvert.SerializeObject(New With {.Success = False, .Changes = 0}))
                    End If
                Else
                    Throw New InvalidOperationException("Unknown command.")
                End If
            Catch ex As Exception
                context.Response.Write(JsonConvert.SerializeObject(New With {.error = ex.Message}))
            End Try
        End Sub

        Private Function UpdateInventory(context As HttpContext) As Boolean
            Return Boolean.Parse(context.Request.Form("UpdateInventory"))
        End Function

        Private Function TryGetEditValues(context As HttpContext, ByRef podid As Integer, ByRef partNum As String, ByRef quantity As Double, ByRef unitPrice As Double) As Boolean
            Dim result As Boolean = True

            result = result AndAlso Integer.TryParse(context.Request.Form("podid"), podid)
            result = result AndAlso TryGetValue(context.Request.Form, "partNum", partNum)
            result = result AndAlso Double.TryParse(context.Request.Form("quantity"), quantity)
            result = result AndAlso Double.TryParse(context.Request.Form("unitPrice"), unitPrice)

            Return result
        End Function

        Private Function TryGetValue(nvc As NameValueCollection, key As String, ByRef result As String) As Boolean
            If nvc.AllKeys.Contains(key) Then
                result = nvc(key)
                Return True
            Else
                result = String.Empty
                Return False
            End If
        End Function
    End Class
End Namespace