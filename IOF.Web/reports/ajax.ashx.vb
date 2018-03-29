Imports IOF.Models
Imports Newtonsoft.Json
Imports StructureMap.Attributes

Namespace Reports
    Public Class Ajax
        Inherits IOFHandler

        <SetterProperty>
        Public Property ReportService As IReportService

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            context.Response.ContentType = "application/json"
            Dim data As IEnumerable(Of StoreManagerReportItem) = ReportService.GetStoreManagerReport()
            context.Response.Write(JsonConvert.SerializeObject(New With {data}))
        End Sub
    End Class
End Namespace