Public Class Vendors
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            phStoreManager.Visible = IsStoreManager()
            chkStoreManager.Checked = StoreManager
            LoadVendors()
        End If
    End Sub

    Protected Overrides Sub OnStoreManagerCheckChanged(checked As Boolean)
        LoadVendors()
    End Sub

    Private Sub LoadVendors()
        rptVendors.DataSource = VendorRepository.GetActiveVendors(ActiveClientID).OrderBy(Function(x) x.VendorName)
        rptVendors.DataBind()
    End Sub

    Protected Sub VendorInfo1_AddClick(ByVal sender As Object, ByVal e As EventArgs)
        LoadVendors()
        VendorInfo1.Visible = False
        phVendorList.Visible = True
    End Sub

    Protected Sub VendorInfo1_UpdateClick(ByVal sender As Object, ByVal e As EventArgs)
        LoadVendors()
        VendorInfo1.Visible = False
        phVendorList.Visible = True
    End Sub

    Protected Sub VendorInfo1_CancelClick(ByVal sender As Object, ByVal e As EventArgs)
        VendorInfo1.Visible = False
        phVendorList.Visible = True
    End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        If e.CommandName = "add" Then
            VendorInfo1.VendorID = 0
            VendorInfo1.DataBind()
            VendorInfo1.Visible = True
            phVendorList.Visible = False
        ElseIf e.CommandName = "edit" Then
            VendorInfo1.VendorID = Convert.ToInt32(e.CommandArgument)
            VendorInfo1.DataBind()
            VendorInfo1.Visible = True
            phVendorList.Visible = False
        ElseIf e.CommandName = "delete" Then
            VendorRepository.Delete(Convert.ToInt32(e.CommandArgument))
            LoadVendors()
        End If
    End Sub
End Class