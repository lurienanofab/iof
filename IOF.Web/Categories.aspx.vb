Imports LNF.Impl.Repository.Ordering
Imports LNF.Repository

Public Class Categories
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            CheckPrivs()
        End If
    End Sub

    Private Sub ShowNode(node As TreeNode, selected As Boolean, expanded As Boolean)
        If node Is Nothing Then Return

        Dim pn As TreeNode = node.Parent
        While pn IsNot Nothing
            pn.Expand()
            pn = pn.Parent
        End While

        If selected Then
            node.Select()
        Else
            node.Selected = False
        End If

        If expanded Then
            node.Expand()
        Else
            node.Collapse()
        End If
    End Sub

    Private Sub AddChildNodes(ByRef parentNode As TreeNode)
        Dim parentId As Integer = Convert.ToInt32(parentNode.Value)
        Dim children As PurchaseOrderCategory() = DataSession.Query(Of PurchaseOrderCategory)().Where(Function(x) x.ParentID = parentId AndAlso x.Active).ToArray()
        For Each c As PurchaseOrderCategory In children
            Dim tn As New TreeNode(String.Format("[{1}] {0}", c.CatName, c.CatNo), c.CatID.ToString())
            parentNode.ChildNodes.Add(tn)
            AddChildNodes(tn)
        Next
    End Sub

    Protected Sub CheckPrivs()
        phEditForms.Visible = IsAdministrator()
    End Sub
End Class