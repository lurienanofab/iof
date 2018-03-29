Public Class SearchIOF
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Action = "Search" Then
            PurchaseOrderSearch1.EnablePOID = True
            PurchaseOrderSearch1.EnableCopy = False
            PurchaseOrderSearch1.Action = VirtualPathUtility.ToAbsolute("~/POConfirm.aspx?Action=Search&POID={poid}")
            PurchaseOrderSearch1.Title = "Search IOF"
        ElseIf Action = "UseExisting" Then
            PurchaseOrderSearch1.EnablePOID = False
            PurchaseOrderSearch1.EnableCopy = True
            PurchaseOrderSearch1.Action = VirtualPathUtility.ToAbsolute("~/POConfirm.aspx?Action=Copy&POID={poid}")
            PurchaseOrderSearch1.Title = "Start From Existing IOF"
        End If

        SearchCriteria.UseDefaultStartDate()
        SearchCriteria.UseDefaultEndDate()
        SearchCriteria.UseOtherClientID(IOFContext.CurrentUser.ClientID)
    End Sub
End Class