Imports IOF.Models
Imports LNF.Web
Imports Newtonsoft.Json
Imports StructureMap.Attributes

Namespace Ajax
    Public Class Datatables
        Inherits IOFHandler
        Implements IReadOnlySessionState

        <SetterProperty>
        Public Property SearchService As ISearchService

        Public Overrides Sub ProcessRequest(context As HttpContext)
            context.Response.ContentType = "application/json"

            If context.Request.QueryString("command") = "clear-criteria" Then
                SearchCriteria.Clear()
                context.Response.Write(JsonConvert.SerializeObject(New With {.clear = True}))
                Return
            ElseIf context.Request.QueryString("command") = "get-criteria" Then
                Dim criteria = New With
                {
                    .startDate = SearchCriteria.StartDate,
                    .endDate = SearchCriteria.EndDate,
                    .vendorId = SearchCriteria.VendorID,
                    .vendorName = SearchCriteria.VendorName,
                    .keywords = SearchCriteria.Keywords,
                    .partNumber = SearchCriteria.PartNumber,
                    .poid = SearchCriteria.POID,
                    .shortcode = SearchCriteria.ShortCode,
                    .otherClientId = SearchCriteria.OtherClientID,
                    .includeSelf = SearchCriteria.IncludeSelf
                }

                context.Response.Write(JsonConvert.SerializeObject(criteria))

                Return
            End If

            Dim args As OrderSearchArgs = context.Request.GetDocumentContents(Of OrderSearchArgs)()

            SearchCriteria.Save(args.StartDate, args.EndDate, args.VendorID, args.VendorName, args.Keywords, args.PartNumber, args.POID, args.ShortCode, args.OtherClientID, args.IncludeSelf)

            If args Is Nothing Then
                Throw New ArgumentNullException("args")
            End If

            Dim result As SearchResult(Of OrderSearchItem) = SearchService.OrderSearch(args)

            Dim response = SerializeResult(result)

            context.Response.Write(response)
        End Sub

        Private Function SerializeResult(Of T)(result As SearchResult(Of T)) As String
            Dim obj = New With
            {
                .draw = result.Draw,
                .recordsTotal = result.RecordsTotal,
                .recordsFiltered = result.RecordsFiltered,
                .data = result.Data
            }

            Return JsonConvert.SerializeObject(obj)
        End Function

        Private Function IsStoreManager(context As HttpContext) As Boolean
            If context.Session("StoreManager") Is Nothing Then Return False
            Return Convert.ToBoolean(context.Session("StoreManager"))
        End Function
    End Class
End Namespace