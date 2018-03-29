Imports IOF.Models
Imports StructureMap.Attributes

Public Class IOFPage
    Inherits Page

    <SetterProperty>
    Public Property IOFContext As IContext

    <SetterProperty>
    Public Property EmailService As IEmailService

    <SetterProperty>
    Public Property PdfService As IPdfService

    <SetterProperty>
    Public Property OrderRepository As IOrderRepository

    <SetterProperty>
    Public Property DetailRepository As IDetailRepository

    <SetterProperty>
    Public Property VendorRepository As IVendorRepository

    <SetterProperty>
    Public Property ClientRepository As IClientRepository

    <SetterProperty>
    Public Property AccountRepository As IAccountRepository

    <SetterProperty>
    Public Property ItemRepository As IItemRepository

    <SetterProperty>
    Public Property SearchService As ISearchService

    Private _CurrentUserAccounts As IList(Of Account)

    Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        If Page.IsPostBack Then
            Dim target As String = Request.Form("__EVENTTARGET")
            If target = "chkStoreManager" Then
                Dim arg As Boolean
                If (Boolean.TryParse(Request.Form("__EVENTARGUMENT"), arg)) Then
                    StoreManager = arg
                    OnStoreManagerCheckChanged(arg)
                End If
            End If
        End If

        MyBase.OnLoad(e)
    End Sub

    Public ReadOnly Property ActiveClientID As Integer
        Get
            If StoreManager Then
                Return 0
            Else
                Return IOFContext.CurrentUser.ClientID
            End If
        End Get
    End Property

    Public Property StoreManager As Boolean
        Get
            If (Session("StoreManager") Is Nothing) Then Return False
            If Not IsStoreManager() Then Return False
            Return Convert.ToBoolean(Session("StoreManager"))
        End Get
        Set(value As Boolean)
            Session("StoreManager") = value
        End Set
    End Property

    ''' <summary>
    ''' Get accounts for the current user.
    ''' </summary>
    Public ReadOnly Property CurrentUserAccounts As IList(Of Account)
        Get
            If _CurrentUserAccounts Is Nothing Then
                _CurrentUserAccounts = AccountRepository.GetActiveAccounts(IOFContext.CurrentUser.ClientID).OrderBy(Function(x) x.AccountName).ToList()
            End If
            Return _CurrentUserAccounts
        End Get
    End Property

    Public ReadOnly Property Action As String
        Get
            Return Request.QueryString("Action")
        End Get
    End Property

    Public ReadOnly Property POID As Integer
        Get
            Return If(String.IsNullOrEmpty(Request.QueryString("POID")), 0, Convert.ToInt32(Request.QueryString("POID")))
        End Get
    End Property

    Public ReadOnly Property FromPOID As Integer
        Get
            Return If(String.IsNullOrEmpty(Request.QueryString("FromPOID")), 0, Convert.ToInt32(Request.QueryString("FromPOID")))
        End Get
    End Property

    Public ReadOnly Property ReturnTo As String
        Get
            Return Request.QueryString("ReturnTo")
        End Get
    End Property

    Protected Overridable Sub OnStoreManagerCheckChanged(checked As Boolean)
        ' do nothing unless overridden
    End Sub

    Protected Function GetPrintUrl() As String
        Return $"~/PrintIOF.ashx?POID={POID}"
    End Function

    Protected Function GetInventoryItemID(ddl As DropDownList, ByRef result As Integer?) As Boolean
        result = Nothing
        If StoreManager Then
            If ddl.SelectedValue = "0" Then
                Return False
            End If
            result = Integer.Parse(ddl.SelectedValue)
        End If
        Return True
    End Function

    ''' <summary>
    ''' Check if the current user is a store manager.
    ''' </summary>
    Public Function IsStoreManager() As Boolean
        Return ClientRepository.IsStoreManager(IOFContext.CurrentUser.ClientID)
    End Function

    ''' <summary>
    ''' Check if the current user is an administrator.
    ''' </summary>
    Public Function IsAdministrator() As Boolean
        Return ClientRepository.IsAdministrator(IOFContext.CurrentUser.ClientID)
    End Function

    ''' <summary>
    ''' Check if the current user is a staff member.
    ''' </summary>
    Public Function IsStaff() As Boolean
        Return ClientRepository.IsStaff(IOFContext.CurrentUser.ClientID)
    End Function

    ''' <summary>
    ''' Check if the current user is a purchaser.
    ''' </summary>
    Public Function IsPurchaser() As Boolean
        Return ClientRepository.IsPurchaser(IOFContext.CurrentUser.ClientID)
    End Function

    ''' <summary>
    ''' Check if the current user is an approver.
    ''' </summary>
    Public Function IsApprover() As Boolean
        Return ClientRepository.IsApprover(IOFContext.CurrentUser.ClientID)
    End Function
End Class
