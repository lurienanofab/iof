Public Class Route
    Public Const FORMS_GROUP As Integer = 0
    Public Const SETTINGS_GROUP As Integer = 1
    Public Const APPROVER_GROUP As Integer = 2
    Public Const PURCHASER_GROUP As Integer = 3
    Public Const REPORTS_GROUP As Integer = 4
    Public Const ADMIN_GROUP As Integer = 5

    Public ReadOnly Property Path As String
    Public ReadOnly Property Action As String
    Public ReadOnly Property Type As String
    Public ReadOnly Property Group As Integer
    Public ReadOnly Property Link As Integer

    Private Sub New(path As String, action As String, type As String, group As Integer, link As Integer)
        Me.Path = path
        Me.Action = action
        Me.Type = type
        Me.Group = group
        Me.Link = link
    End Sub

    Public Shared Function GetCurrentRoute(request As HttpRequest) As Route
        Dim path As String = request.Url.AbsolutePath
        Dim action As String = If(request.QueryString("Action"), String.Empty)
        Dim type As String = If(request.QueryString("Type"), String.Empty)

        Dim result As Route = _RouteTable.FirstOrDefault(Function(x) x.Path = path AndAlso x.Action = action AndAlso x.Type = type)

        If result Is Nothing Then
            Throw New Exception($"Unable to find route for Path = [{path}], Action = [{action}], Type = [{type}]")
        End If

        Return result
    End Function

    Private Shared _RouteTable As New List(Of Route) From
    {
        New Route(AbsolutePath("~/index.aspx"), String.Empty, String.Empty, -1, -1),
        New Route(AbsolutePath("~/SearchIOF.aspx"), "UseExisting", String.Empty, FORMS_GROUP, 0),
        New Route(AbsolutePath("~/POInfo.aspx"), "UseExisting", String.Empty, FORMS_GROUP, 0),
        New Route(AbsolutePath("~/POItems.aspx"), "UseExisting", String.Empty, FORMS_GROUP, 0),
        New Route(AbsolutePath("~/POConfirm.aspx"), "UseExisting", String.Empty, FORMS_GROUP, 0),
        New Route(AbsolutePath("~/POConfirm.aspx"), "Copy", String.Empty, FORMS_GROUP, 0),
        New Route(AbsolutePath("~/POInfo.aspx"), "New", String.Empty, FORMS_GROUP, 1),
        New Route(AbsolutePath("~/POItems.aspx"), "New", String.Empty, FORMS_GROUP, 1),
        New Route(AbsolutePath("~/POConfirm.aspx"), "New", String.Empty, FORMS_GROUP, 1),
        New Route(AbsolutePath("~/SearchIOF.aspx"), "Search", String.Empty, FORMS_GROUP, 2),
        New Route(AbsolutePath("~/POConfirm.aspx"), "Search", String.Empty, FORMS_GROUP, 2),
        New Route(AbsolutePath("~/POConfirm.aspx"), "Unfinished", String.Empty, FORMS_GROUP, 3),
        New Route(AbsolutePath("~/POItems.aspx"), "Unfinished", String.Empty, FORMS_GROUP, 3),
        New Route(AbsolutePath("~/POInfo.aspx"), "Unfinished", String.Empty, FORMS_GROUP, 3),
        New Route(AbsolutePath("~/UnfinishedIOF.aspx"), String.Empty, String.Empty, FORMS_GROUP, 3),
        New Route(AbsolutePath("~/TrackIOF.aspx"), "Detail", String.Empty, FORMS_GROUP, 4),
        New Route(AbsolutePath("~/TrackIOF.aspx"), String.Empty, String.Empty, FORMS_GROUP, 4),
        New Route(AbsolutePath("~/Vendors.aspx"), String.Empty, String.Empty, SETTINGS_GROUP, 0),
        New Route(AbsolutePath("~/Accounts.aspx"), String.Empty, String.Empty, SETTINGS_GROUP, 1),
        New Route(AbsolutePath("~/Approvers.aspx"), String.Empty, String.Empty, SETTINGS_GROUP, 2),
        New Route(AbsolutePath("~/Items.aspx"), String.Empty, String.Empty, SETTINGS_GROUP, 3),
        New Route(AbsolutePath("~/CopyData.aspx"), String.Empty, String.Empty, SETTINGS_GROUP, 4),
        New Route(AbsolutePath("~/ApprovalList.aspx"), String.Empty, String.Empty, APPROVER_GROUP, 0),
        New Route(AbsolutePath("~/ApprovalProcess.aspx"), String.Empty, String.Empty, APPROVER_GROUP, 0),
        New Route(AbsolutePath("~/PurchaseList.aspx"), String.Empty, String.Empty, PURCHASER_GROUP, 0),
        New Route(AbsolutePath("~/OrderedIOF.aspx"), String.Empty, String.Empty, PURCHASER_GROUP, 1),
        New Route(AbsolutePath("~/reports/index.aspx"), String.Empty, "ipbm", REPORTS_GROUP, 0),
        New Route(AbsolutePath("~/reports/index.aspx"), String.Empty, "smr", REPORTS_GROUP, 1),
        New Route(AbsolutePath("~/Categories.aspx"), String.Empty, String.Empty, ADMIN_GROUP, 0),
        New Route(AbsolutePath("~/docs/temp/index.aspx"), String.Empty, String.Empty, ADMIN_GROUP, 1),
        New Route(AbsolutePath("~/Purchasers.aspx"), String.Empty, String.Empty, ADMIN_GROUP, 2)
    }

    Public Shared ReadOnly Property RouteTable As IEnumerable(Of Route)
        Get
            Return _RouteTable
        End Get
    End Property

    Public Shared Function AbsolutePath(virtualPath As String) As String
        Return VirtualPathUtility.ToAbsolute(virtualPath)
    End Function
End Class
