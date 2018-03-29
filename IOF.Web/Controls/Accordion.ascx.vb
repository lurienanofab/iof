Public Class Accordion
    Inherits IOFControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        lnkHome.HRef = VirtualPathUtility.ToAbsolute("~")
    End Sub

    Public Property DataSource As IList(Of AccordionGroup)

    Public Overrides Sub DataBind()
        rptAccordionGroups.DataSource = From x In DataSource
                                        Where x.Visible
        rptAccordionGroups.DataBind()
    End Sub

    Protected Sub rptAccordion_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim group As AccordionGroup = CType(e.Item.DataItem, AccordionGroup)
        Dim rptNavLinks As Repeater = CType(e.Item.FindControl("rptNavLinks"), Repeater)

        rptNavLinks.DataSource = From x In group.Links
                                 Where x.Visible
        rptNavLinks.DataBind()
    End Sub
End Class

Public Class AccordionGroup
    Public Property ID As String
    Public Property Title As String
    Public Property Expanded As Boolean
    Public Property Links As IList(Of AccordionLink) = New List(Of AccordionLink)
    Public Property Visible As Boolean

    Public Sub New()

    End Sub

    Public Sub New(id As String, title As String, expanded As Boolean, visible As Boolean)
        Me.ID = id
        Me.Title = title
        Me.Expanded = expanded
        Me.Visible = visible
    End Sub

    Public ReadOnly Property AriaExpanded As String
        Get
            Return Expanded.ToString().ToLower()
        End Get
    End Property

    Public ReadOnly Property HeadingID As String
        Get
            Return $"Heading{ID}"
        End Get
    End Property

    Public ReadOnly Property CollapseID As String
        Get
            Return $"Collapse{ID}"
        End Get
    End Property

    Public ReadOnly Property TitleCssClass As String
        Get
            Return If(Expanded, String.Empty, "collapsed")
        End Get
    End Property

    Public ReadOnly Property CollapseCssClass As String
        Get
            Return If(Expanded, "panel-collapse collapse in", "panel-collapse collapse")
        End Get
    End Property
End Class

Public Class AccordionLink
    Public Property Url As String
    Public Property Text As String
    Public Property Target As String
    Public Property Selected As Boolean
    Public Property Visible As Boolean

    Sub New()
    End Sub

    Sub New(url As String, text As String, selected As Boolean, visible As Boolean)
        Me.New(url, text, String.Empty, selected, visible)
    End Sub

    Sub New(url As String, text As String, target As String, selected As Boolean, visible As Boolean)
        Me.Url = url
        Me.Text = text
        Me.Target = target
        Me.Selected = selected
        Me.Visible = visible
    End Sub

    Public ReadOnly Property CssClass As String
        Get
            Return If(Selected, "nav-link selected", "nav-link")
        End Get
    End Property
End Class