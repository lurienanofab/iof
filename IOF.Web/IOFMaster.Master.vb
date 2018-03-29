Imports StructureMap.Attributes

Public Class IOFMaster
    Inherits MasterPage

    <SetterProperty>
    Public Property PdfService As IPdfService

    Public Property CurrentRoute As Route

    Public Sub New()
        IOC.Container.BuildUp(Me)
    End Sub

    Public Overloads Property Page As IOFPage
        Get
            Return CType(MyBase.Page, IOFPage)
        End Get
        Set(value As IOFPage)
            MyBase.Page = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        'It is possible that there is no authenticated user when a Approve link is clicked from the approver email.
        'A user login is not necessary in this case so that supervisors can approve from their phones, etc without logging in.
        If Session("UserName") IsNot Nothing AndAlso Page.User IsNot Nothing AndAlso Page.User.Identity IsNot Nothing Then
            If Session("UserName").ToString() <> Page.User.Identity.Name Then
                Session.Abandon()
                Response.Redirect(Request.Url.ToString())
            End If
        End If

        If Not Page.IsPostBack Then
            CurrentRoute = Route.GetCurrentRoute(Request)
            Accordion1.DataSource = GetAccordionGroups()
            Accordion1.DataBind()
        End If
    End Sub

    Private Function GetAccordionGroups() As IList(Of AccordionGroup)
        Dim currentRoute As Route = Route.GetCurrentRoute(Request)

        Dim result As New List(Of AccordionGroup)

        Dim group As AccordionGroup

        ' 0, 0  => ~/SearchIOF.aspx?Action=UseExisting
        ' 0, 1  => ~/POInfo.aspx?Action=New
        ' 0, 2  => ~/SearchIOF.aspx?Action=Search
        ' 0, 3  => ~/UnfinishedIOF.aspx?Action=Unfinished
        ' 0, 4  => ~/TrackIOF.aspx
        ' 1, 0  => ~/Vendors.aspx
        ' 1, 1  => ~/Accounts.aspx
        ' 1, 2  => ~/Approvers.aspx
        ' 1, 3  => ~/Items.aspx
        ' 1, 4  => ~/CopyData.aspx
        ' 2, 0  => ~/ApprovalList.aspx
        ' 3, 0  => ~/PurchaseList.aspx
        ' 3, 1  => ~/OrderedIOF.aspx
        ' 4, 0  => ~/reports/?type=ipbm
        ' 4, 1  => ~/reports/?type=smr
        ' 5, 0  => ~/Categories.aspx
        ' 5, 1  => ~/docs/temp/
        ' 5, 2  => ~/Purchasers.aspx

        group = New AccordionGroup("Forms", "IOF Forms", IsExpanded(0), True)
        group.Links.Add(New AccordionLink("~/SearchIOF.aspx?Action=UseExisting", "Start From Existing IOF", IsSelected(0, 0), True))
        group.Links.Add(New AccordionLink("~/POInfo.aspx?Action=New", "Start New", IsSelected(0, 1), True))
        group.Links.Add(New AccordionLink("~/SearchIOF.aspx?Action=Search", "Search IOF", IsSelected(0, 2), True))
        group.Links.Add(New AccordionLink("~/UnfinishedIOF.aspx", "Draft IOF", IsSelected(0, 3), True))
        group.Links.Add(New AccordionLink("~/TrackIOF.aspx", "IOF Tracking", IsSelected(0, 4), True))
        group.Links.Add(New AccordionLink("/legacy-iof", "Legacy IOF (New Window)", "_blank", False, True))
        result.Add(group)

        group = New AccordionGroup("Settings", "Settings", IsExpanded(1), True)
        group.Links.Add(New AccordionLink("~/Vendors.aspx", "Vendors", IsSelected(1, 0), True))
        group.Links.Add(New AccordionLink("~/Accounts.aspx", "Accounts", IsSelected(1, 1), True))
        group.Links.Add(New AccordionLink("~/Approvers.aspx", "Approvers", IsSelected(1, 2), True))
        group.Links.Add(New AccordionLink("~/Items.aspx", "Items", IsSelected(1, 3), True))
        group.Links.Add(New AccordionLink("~/CopyData.aspx", "Copy Data", IsSelected(1, 4), True))
        result.Add(group)

        group = New AccordionGroup("Approver", "Approver", IsExpanded(2), True)
        group.Links.Add(New AccordionLink("~/ApprovalList.aspx", "Approval List", IsSelected(2, 0), True))
        result.Add(group)

        group = New AccordionGroup("Purchaser", "Purchaser", IsExpanded(3), True)
        group.Links.Add(New AccordionLink("~/PurchaseList.aspx", "Purchase List", IsSelected(3, 0), IOFUtility.Settings.UsePurchaseList))
        group.Links.Add(New AccordionLink("~/OrderedIOF.aspx", "Orders", IsSelected(3, 1), True))
        result.Add(group)

        group = New AccordionGroup("Reports", "Reports", IsExpanded(4), True)
        group.Links.Add(New AccordionLink("~/reports/?type=ipbm", "Item Purchases By Month", IsSelected(4, 0), True))
        group.Links.Add(New AccordionLink("~/reports/?type=smr", "Store Manager Report", IsSelected(4, 1), True))
        result.Add(group)

        group = New AccordionGroup("Administrator", "Administrator", IsExpanded(5), Page.IsAdministrator())
        group.Links.Add(New AccordionLink("~/Categories.aspx", "Categories", IsSelected(5, 0), True))
        group.Links.Add(New AccordionLink("~/Purchasers.aspx", "Purchasers", IsSelected(5, 2), True))
        group.Links.Add(New AccordionLink("~/docs/temp/", "File List", IsSelected(5, 1), True))
        result.Add(group)

        Return result
    End Function

    Private Function IsExpanded(group As Integer) As Boolean
        Return CurrentRoute.Group = group
    End Function

    Private Function IsSelected(group As Integer, link As Integer) As Boolean
        Return CurrentRoute.Group = group AndAlso CurrentRoute.Link = link
    End Function
End Class