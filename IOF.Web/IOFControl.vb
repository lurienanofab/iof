Public Class IOFControl
    Inherits UserControl

    Public ReadOnly Property IOFContext As IContext
        Get
            Return Page.IOFContext
        End Get
    End Property

    Public ReadOnly Property ClientRepository As IClientRepository
        Get
            Return Page.ClientRepository
        End Get
    End Property

    Public ReadOnly Property AccountRepository As IAccountRepository
        Get
            Return Page.AccountRepository
        End Get
    End Property

    Public ReadOnly Property OrderRepository As IOrderRepository
        Get
            Return Page.OrderRepository
        End Get
    End Property

    Public ReadOnly Property DetailRepository As IDetailRepository
        Get
            Return Page.DetailRepository
        End Get
    End Property

    Public ReadOnly Property VendorRepository As IVendorRepository
        Get
            Return Page.VendorRepository
        End Get
    End Property

    Public Overloads Property Page As IOFPage
        Get
            Return CType(MyBase.Page, IOFPage)
        End Get
        Set(value As IOFPage)
            MyBase.Page = Page
        End Set
    End Property

    Sub New()
        'IOC.Container.BuildUp(Me)
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
