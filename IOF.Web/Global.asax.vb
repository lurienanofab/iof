Imports System.Security.Principal
Imports LNF.Web
Imports StructureMap.Attributes

Public Class Global_asax
    Inherits HttpApplication

    <SetterProperty>
    Public Property IOFContext As IContext

    <SetterProperty>
    Public Property ClientRepository As IClientRepository

    Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
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
            If ClientRepository.IsPurchaser(IOFContext.CurrentUser.ClientID) Then
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
End Class