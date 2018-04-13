Imports IOF.Models
Imports StructureMap.Attributes

Namespace Reports
    Public Class Index
        Inherits IOFPage

        <SetterProperty>
        Public Property ExcelService As IExcelService

        <SetterProperty>
        Public Property ReportService As IReportService

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            Select Case Request.QueryString("type")
                Case "ipbm"
                    ShowItemReport()
                Case "smr"
                    ShowStoreManagerReport()
            End Select
        End Sub

        Private Sub ShowItemReport()
            litReportTitle.Text = "Item Purchases By Month"
            phItemReport.Visible = True

            If Not Page.IsPostBack Then
                Dim clients As IEnumerable(Of Client) = ClientRepository.GetActiveClients()
                ddlClients.DataSource = clients.OrderBy(Function(x) x.DisplayName)
                ddlClients.DataBind()

                Dim selectedClientId As String

                If IsStoreManager() Then
                    Dim listItem As New ListItem()
                    listItem.Text = "Store Manager"
                    listItem.Value = "0"
                    ddlClients.Items.Insert(0, listItem)
                    selectedClientId = "0"
                Else
                    selectedClientId = IOFContext.CurrentUser.ClientID.ToString()
                End If


                ddlClients.SelectedValue = selectedClientId

                UpdateItems()
            End If
        End Sub

        Private Sub ShowStoreManagerReport()
            litReportTitle.Text = "Store Manager Report"
            phStoreManagerReport.Visible = True
        End Sub

        Protected Sub ddlClients_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            hidSelectedItemID.Value = String.Empty
            UpdateItems()
            LoadItemReport()
        End Sub

        Private Sub UpdateItems()
            Dim items As IEnumerable(Of Item) = ItemRepository.GetClientItems(Integer.Parse(ddlClients.SelectedValue)).OrderBy(Function(x) x.Description)
            rptItems.DataSource = items
            rptItems.DataBind()
        End Sub

        Protected Sub Item_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
            If e.CommandName = "item-report" Then
                Dim itemId As Integer = Convert.ToInt32(e.CommandArgument)
                hidSelectedItemID.Value = itemId.ToString()
                LoadItemReport()
            Else
                Throw New NotImplementedException()
            End If
        End Sub

        Private Sub LoadItemReport()
            If Not String.IsNullOrEmpty(hidSelectedItemID.Value) Then
                Dim itemId As Integer = Integer.Parse(hidSelectedItemID.Value)
                Dim item As Item = ItemRepository.Single(itemId)
                Dim reportItems As IEnumerable(Of ItemReportItem) = ReportService.GetItemReport(itemId)
                ShowItemInfo(item)
                rptItemReport.DataSource = reportItems
                rptItemReport.DataBind()
                trNoData.Visible = reportItems.Count = 0
                litTotalUnit.Text = reportItems.Sum(Function(x) x.TotalUnit).ToString()
                litTotalCost.Text = reportItems.Sum(Function(x) x.TotalCost).ToString("C")
                panItemReportExcelExport.Visible = reportItems.Count > 0
            Else
                ShowItemInfo(Nothing)
                rptItemReport.DataSource = Nothing
                rptItemReport.DataBind()
                panItemReportExcelExport.Visible = False
            End If
        End Sub

        Protected Sub Export_Command(ByVal sender As Object, ByVal e As CommandEventArgs)

            Dim command As String = e.CommandName
            Dim resp As String = "Invalid format"
            Dim buffer As Byte() = Nothing
            Dim ct As String = "text/html"
            Dim fn As String = String.Empty

            Dim reportType As String = Request.QueryString("type")

            If reportType = "ipbm" Then
                Dim itemId As Integer = Convert.ToInt32(hidSelectedItemID.Value)
                Dim data As IEnumerable(Of ItemReportItem) = ReportService.GetItemReport(itemId)

                If command = "export-excel" Then
                    Dim exp As IExcelExport = ExcelService.CreateExport()
                    Dim ws As IWorksheet(Of ItemReportItem) = exp.AddWorkSheet(data, "Item Report")
                    ws.Sum(Function(x) x.TotalUnit)
                    ws.Sum(Function(x) x.TotalCost)
                    ws.FormatCurrency(Function(x) x.TotalCost)

                    buffer = exp.GetBytes()
                    ct = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    fn = "ItemPurchasesByMonth.xlsx"
                Else
                    Dim csv As New StringBuilder()

                    csv.AppendLine("Year,Month,MonthName,TotalUnit,TotalCost")

                    For Each item As ItemReportItem In data
                        csv.AppendLine($"{item.Year},{item.Month},{item.MonthName},{item.TotalUnit},{item.TotalCost}")
                    Next

                    buffer = Encoding.UTF8.GetBytes(csv.ToString())
                    ct = "application/csv"
                    fn = "ItemPurchasesByMonth.csv"
                End If
            ElseIf reportType = "smr" Then
                Dim data As IEnumerable(Of StoreManagerReportItem) = ReportService.GetStoreManagerReport()

                If command = "export-excel" Then
                    Dim exp As IExcelExport = ExcelService.CreateExport()
                    Dim ws As IWorksheet(Of StoreManagerReportItem) = exp.AddWorkSheet(data, "Store Manager Report")
                    ws.FormatCurrency(Function(x) x.UnitPrice)
                    ws.FormatCurrency(Function(x) x.StorePackagePrice)
                    ws.FormatCurrency(Function(x) x.StoreUnitPrice)
                    ws.Format(Function(x) x.LastOrdered, "MM/dd/yyyy")
                    ws.Format(Function(x) x.LastPurchased, "MM/dd/yyyy")

                    buffer = exp.GetBytes()
                    ct = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    fn = "StoreManagerReport.xlsx"
                Else
                    Dim csv As New StringBuilder()

                    csv.AppendLine("ItemID,Description,VendorID,VendorName,LastOrdered,Unit,UnitPrice,StoreItemID,StoreDescription,StorePackageQuantity,StorePackagePrice,StoreUnitPrice,LastPurchased")

                    For Each item As StoreManagerReportItem In data
                        Dim iofDesc = item.Description.Replace("""", """""")
                        Dim storeDesc = If(Not String.IsNullOrEmpty(item.StoreDescription), item.StoreDescription.Replace("""", """"""), String.Empty)
                        csv.AppendLine($"{item.ItemID},""{iofDesc}"",{item.VendorID},""{item.VendorName}"",{item.LastOrdered},{item.Unit},{item.UnitPrice},{item.StoreItemID},""{storeDesc}"",{item.StorePackageQuantity},{item.StorePackagePrice},{item.StoreUnitPrice},{item.LastPurchased}")
                    Next

                    buffer = Encoding.UTF8.GetBytes(csv.ToString())
                    ct = "application/csv"
                    fn = "StoreManagerReport.csv"
                End If
            Else
                Throw New Exception($"Unknown report type: {reportType}")
            End If

            Response.Clear()
            Response.Charset = String.Empty
            If fn <> String.Empty Then
                Response.AddHeader("content-disposition", "attachment;filename=" + fn)
            End If
            Response.ContentType = ct
            Response.BinaryWrite(buffer)
            Response.End()
        End Sub

        Private Sub ShowItemInfo(item As Item)
            If item Is Nothing Then
                phItemInfo.Visible = False
                litItemDescription.Text = String.Empty
                litItemVendorName.Text = String.Empty
                litItemPartNum.Text = String.Empty
            Else
                phItemInfo.Visible = True
                litItemDescription.Text = item.Description
                litItemVendorName.Text = item.VendorName
                litItemPartNum.Text = item.PartNum
            End If
        End Sub
    End Class
End Namespace