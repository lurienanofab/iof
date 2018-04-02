Imports IOF.Models

Namespace Controls
    Public Class PurchaseOrderSearch
        Inherits IOFControl

        Public Property Title As String = "Search IOF"
        Public Property EnableCopy As Boolean = True
        Public Property EnablePOID As Boolean = True
        Public Property StatusIdList As String = String.Empty
        Public Property DisplayOption As OrderDisplayOption = OrderDisplayOption.Detail
        Public Property Action As String = String.Empty

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            ' this will ensure the __doPostback javascript function is created, which is needed for chkStoreManager event handling
            Page.ClientScript.GetPostBackEventReference(Me, String.Empty)

            If Not Page.IsPostBack Then
                phStoreManager.Visible = Page.IsStoreManager()
                chkStoreManager.Checked = Page.StoreManager
                LoadVendors()
                LoadCriteria()
            End If

            litTitle.Text = Title
        End Sub

        Protected Overrides Sub OnStoreManagerCheckChanged(checked As Boolean)
            LoadVendors()
        End Sub

        Private Sub LoadCriteria()
            If SearchCriteria.StartDate.HasValue Then
                txtStartDate.Value = SearchCriteria.StartDate.Value.ToString("MM/dd/yyyy")
            End If

            If SearchCriteria.EndDate.HasValue Then
                txtEndDate.Value = SearchCriteria.EndDate.Value.AddDays(-1).ToString("MM/dd/yyyy")
            End If

            If SearchCriteria.VendorID > 0 Then
                ddlVendors.SelectedValue = SearchCriteria.VendorID.ToString()
            End If

            If Not String.IsNullOrEmpty(SearchCriteria.VendorName) Then
                txtVendorName.Value = SearchCriteria.VendorName
            End If

            If Not String.IsNullOrEmpty(SearchCriteria.Keywords) Then
                txtKeywords.Value = SearchCriteria.Keywords
            End If

            If Not String.IsNullOrEmpty(SearchCriteria.PartNumber) Then
                txtPartNumber.Value = SearchCriteria.PartNumber
            End If

            If SearchCriteria.POID > 0 Then
                txtPOID.Value = SearchCriteria.POID.ToString()
            End If

            If Not String.IsNullOrEmpty(SearchCriteria.ShortCode) Then
                txtShortCode.Value = SearchCriteria.ShortCode
            End If

            If SearchCriteria.OtherClientID > 0 Then
                ddlClients.SelectedValue = SearchCriteria.OtherClientID.ToString()
            End If

            chkIncludeSelf.Checked = SearchCriteria.IncludeSelf
        End Sub

        Private Sub LoadVendors()
            ddlVendors.DataSource = VendorRepository.GetActiveVendors(Page.ActiveClientID).OrderBy(Function(x) x.VendorName)
            ddlVendors.DataBind()
            ddlVendors.Items.Insert(0, New ListItem("-- Select vendor from the list to filter results --", "-1"))
        End Sub
    End Class
End Namespace