Imports StructureMap.Attributes

Public MustInherit Class IOFHandler
    Implements IHttpHandler

    <SetterProperty>
    Public Property EmailService As IEmailService

    <SetterProperty>
    Public Property AttachmentService As IAttachmentService

    <SetterProperty>
    Public Property ClientRepository As IClientRepository

    <SetterProperty>
    Public Property PurchaseOrderRepository As IOrderRepository

    <SetterProperty>
    Public Property PurchaseOrderDetailRepository As IDetailRepository

    <SetterProperty>
    Public Property DetailRepository As IDetailRepository

    <SetterProperty>
    Public Property ItemRepository As IItemRepository

    Public Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    Public MustOverride Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

    Public Overridable ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
