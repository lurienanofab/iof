Imports Newtonsoft.Json

Namespace Ajax
    Public Class Vendor
        Inherits IOFHandler

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            context.Response.ContentType = "application/json"

            Dim command As String = context.Request.QueryString("command")

            Dim clientId As Integer

            If command = "get-vendor-names" Then
                If Integer.TryParse(context.Request.QueryString("clientId"), clientId) Then
                    context.Response.Write(JsonConvert.SerializeObject(GetVendorNames(0)))
                Else
                    Throw New Exception("Invalid parameter: clientId")
                End If
            Else
                Throw New Exception("Invalid parameter: command")
            End If
        End Sub

        Private Function GetVendorNames(clientId As Integer) As String()
            Dim vendors As IEnumerable(Of Models.Vendor) = VendorRepository.GetActiveVendors(clientId)
            Dim result As String() = vendors.OrderBy(Function(x) x.VendorName).Select(Function(x) x.VendorName).ToArray()
            Return result
        End Function
    End Class
End Namespace