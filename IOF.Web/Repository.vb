Imports IOF.Models
Imports StructureMap.Attributes

''' <summary>
''' A wrapper class for use by ObjectDataSource tags in aspx markup.
''' Be sure to set DataTextField and DataValueField properties on all DropDownLists.
''' </summary>
Public Class Repository

    <SetterProperty>
    Public Property Context As IContext

    <SetterProperty>
    Public Property Clients As IClientRepository

    <SetterProperty>
    Public Property Orders As IOrderRepository

    <SetterProperty>
    Public Property Accounts As IAccountRepository

    <SetterProperty>
    Public Property Vendors As IVendorRepository

    <SetterProperty>
    Public Property Items As IItemRepository

    Public Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    ''' <summary>
    ''' Get a list of all shipping methods. Used in ObjectDataSources (for example in POInfo.aspx).
    ''' </summary>
    Public Function GetAllShippingMethods() As IEnumerable(Of ListItem)
        Return From x In Orders.GetAllShippingMethods()
               Select New ListItem(x.ShippingMethodName, x.ShippingMethodID.ToString())
    End Function

    ''' <summary>
    ''' Get a list of accounts that can be added by the current user. Used in ObjectDataSources (for example in POInfo.aspx).
    ''' </summary>
    Public Function GetAvailableAccounts() As IEnumerable(Of ListItem)
        Return From x In Accounts.GetAvailableAccounts(Context.CurrentUser.ClientID)
               Order By x.AccountDisplayName
               Select New ListItem(x.AccountDisplayName, x.AccountID.ToString())
    End Function

    ''' <summary>
    ''' Get a list of approvers that can be added by the current user. Used in ObjectDataSources (for example in POInfo.aspx).
    ''' </summary>
    Public Function GetAvailableApprovers() As IEnumerable(Of ListItem)
        Return From x In Clients.GetAvailableApprovers(Context.CurrentUser.ClientID)
               Order By x.DisplayName
               Select New ListItem(x.DisplayName, x.ClientID.ToString())
    End Function

    ''' <summary>
    ''' Get a list of active clients for populating DropDownLists. Used in ObjectDataSources (for example in PurchaseOrderSearch.ascx).
    ''' </summary>
    Public Function GetAllClients() As IEnumerable(Of ListItem)
        Dim users As IEnumerable(Of Client) = Clients.GetActiveClients()

        ' must return ONLY one row, else is critical error
        Dim c As Client = (From x In users
                           Where x.ClientID = Context.CurrentUser.ClientID).FirstOrDefault()

        Dim result As New List(Of ListItem)

        result.Add(New ListItem("-- View All --", "-1"))

        If c IsNot Nothing Then
            ' first add the current user
            result.Add(New ListItem(c.DisplayName, c.ClientID.ToString()))

            ' now add everyone else
            result.AddRange(From x In users
                            Where x.ClientID <> Context.CurrentUser.ClientID
                            Order By x.DisplayName
                            Select New ListItem(x.DisplayName, x.ClientID.ToString()))
        Else
            ' we should never get here because there should always be a current user
            result.AddRange(From x In users
                            Order By x.DisplayName
                            Select New ListItem(x.DisplayName, x.ClientID.ToString()))
        End If

        Return result
    End Function
End Class
