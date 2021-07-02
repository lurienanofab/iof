Imports LNF

Public MustInherit Class IOFHandler
    Implements IHttpHandler

    <Inject> Public Property EmailService As IEmailService

    <Inject> Property AttachmentService As IAttachmentService

    <Inject> Public Property ClientRepository As IClientRepository

    <Inject> Public Property PurchaseOrderRepository As IOrderRepository

    <Inject> Public Property PurchaseOrderDetailRepository As IDetailRepository

    <Inject> Public Property DetailRepository As IDetailRepository

    <Inject> Public Property ItemRepository As IItemRepository

    <Inject> Public Property VendorRepository As IVendorRepository

    Public Sub New()
        'IOC.Container.BuildUp(Me)
    End Sub

    Public MustOverride Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

    Public Overridable ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
