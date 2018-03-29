Imports StructureMap.Attributes

Public Class IOFControl
    Inherits UserControl

    <SetterProperty>
    Public Property IOFContext As IContext

    <SetterProperty>
    Public Property ClientRepository As IClientRepository

    <SetterProperty>
    Public Property AccountRepository As IAccountRepository

    <SetterProperty>
    Public Property OrderRepository As IOrderRepository

    <SetterProperty>
    Public Property DetailRepository As IDetailRepository

    <SetterProperty>
    Public Property VendorRepository As IVendorRepository

    Public Overloads Property Page As IOFPage
        Get
            Return CType(MyBase.Page, IOFPage)
        End Get
        Set(value As IOFPage)
            MyBase.Page = Page
        End Set
    End Property

    Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        If Page.IsPostBack Then
            Dim target As String = Request.Form("__EVENTTARGET")
            If target = "chkStoreManager" Then
                Dim arg As Boolean
                If (Boolean.TryParse(Request.Form("__EVENTARGUMENT"), arg)) Then
                    Page.StoreManager = arg
                    OnStoreManagerCheckChanged(arg)
                End If
            End If
        End If

        MyBase.OnLoad(e)
    End Sub

    Protected Overridable Sub OnStoreManagerCheckChanged(checked As Boolean)
        ' do nothing unless overridden
    End Sub
End Class
