﻿Imports IOF.Models

''' <summary>
''' A wrapper class for use by ObjectDataSource tags in aspx markup.
''' Be sure to set DataTextField and DataValueField properties on all DropDownLists.
''' </summary>
Public Class Repository
    Public ReadOnly Property Context As IContext
        Get
            Return [Global].Container.GetInstance(Of IContext)()
        End Get
    End Property

    Public ReadOnly Property Clients As IClientRepository
        Get
            Return [Global].Container.GetInstance(Of IClientRepository)()
        End Get
    End Property

    Public ReadOnly Property Orders As IOrderRepository
        Get
            Return [Global].Container.GetInstance(Of IOrderRepository)()
        End Get
    End Property

    Public ReadOnly Property Accounts As IAccountRepository
        Get
            Return [Global].Container.GetInstance(Of IAccountRepository)()
        End Get
    End Property

    Public ReadOnly Property Vendors As IVendorRepository
        Get
            Return [Global].Container.GetInstance(Of IVendorRepository)()
        End Get
    End Property

    Public ReadOnly Property Items As IItemRepository
        Get
            Return [Global].Container.GetInstance(Of IItemRepository)()
        End Get
    End Property

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
        Dim available As IEnumerable(Of Approver) = Clients.GetAvailableApprovers(Context.CurrentUser.ClientID)

        Return From x In available
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

        Dim result As New List(Of ListItem) From {
            New ListItem("-- View All --", "-1")
        }

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

    Public Function GetAllStaff() As IEnumerable(Of ListItem)
        Dim users As IEnumerable(Of Client) = Clients.GetAllClients(2)


        ' must return ONLY one row, else is critical error
        Dim c As Client = Context.CurrentUser

        Dim result As New List(Of ListItem) From {
            New ListItem("-- View All --", "-1")
        }

        If c IsNot Nothing Then
            ' first add the current user
            result.Add(New ListItem(c.DisplayName, c.ClientID.ToString()))

            ' now add everyone else
            result.AddRange(From x In users
                            Where x.ClientID <> c.ClientID
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
