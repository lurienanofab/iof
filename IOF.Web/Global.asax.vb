Imports System.Reflection
Imports System.Security.Principal
Imports System.Web.Compilation
Imports IOF.Impl
Imports LNF
Imports LNF.DataAccess
Imports LNF.Impl
Imports LNF.Web
Imports SimpleInjector

Public Class [Global]
    Inherits HttpApplication

    Private Shared webapp As WebApp

    Public Shared ReadOnly Property Container As Container
        Get
            Return webapp.Container
        End Get
    End Property

    Private _uow As IUnitOfWork

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        Bootstrap()
        If IOFUtility.Settings.IsProduction Then
            Application("AppServer") = $"http://{Environment.MachineName}.eecs.umich.edu/"
        Else
            Application("AppServer") = "http://localhost/"
        End If
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        If User.IsInRole("FinancialAdmin") Then
            Dim clientRepo As IClientRepository = webapp.GetInstance(Of IClientRepository)()
            Dim iofContext As IContext = webapp.GetInstance(Of IContext)()
            If clientRepo.IsPurchaser(iofContext.CurrentUser.ClientID) Then
                Response.Redirect("~/PurchaseList.aspx")
            End If
        End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub

    Protected Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As System.EventArgs)
        If Request.IsAuthenticated Then
            Dim ident As FormsIdentity = CType(User.Identity, FormsIdentity)
            Dim roles As String() = ident.Ticket.UserData.Split("|"c)
            Context.User = New GenericPrincipal(ident, roles)
        End If
    End Sub

    Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        _uow = webapp.GetInstance(Of IProvider)().DataAccess.StartUnitOfWork()
    End Sub

    Protected Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        If _uow IsNot Nothing Then
            _uow.Dispose()
        End If
    End Sub

    Private Sub Bootstrap()
        Dim assemblies As Assembly() = BuildManager.GetReferencedAssemblies().Cast(Of Assembly)().ToArray()

        webapp = New WebApp()

        ' setup up dependency injection container
        Dim wcc As New WebContainerConfiguration(webapp.Container)
        wcc.EnablePropertyInjection()
        wcc.RegisterAllTypes()

        webapp.Container.Register(Of IContext, Context)()
        webapp.Container.Register(Of IOrderRepository, OrderRepository)()
        webapp.Container.Register(Of IDetailRepository, DetailRepository)()
        webapp.Container.Register(Of IItemRepository, ItemRepository)()
        webapp.Container.Register(Of IClientRepository, ClientRepository)()
        webapp.Container.Register(Of IAccountRepository, AccountRepository)()
        webapp.Container.Register(Of IVendorRepository, VendorRepository)()
        webapp.Container.Register(Of IReportService, ReportService)()
        webapp.Container.Register(Of IExcelService, ExcelService)()
        webapp.Container.Register(Of ISearchService, SearchService)()
        webapp.Container.Register(Of IPdfService, PdfService)()
        webapp.Container.Register(Of IAttachmentService, AttachmentService)()
        webapp.Container.Register(Of IEmailService, EmailService)()

        ' setup web dependency injection
        webapp.Bootstrap(assemblies)
    End Sub
End Class