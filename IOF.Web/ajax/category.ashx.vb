Imports Newtonsoft.Json

Namespace Ajax
    Public Class Category
        Inherits IOFHandler

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            context.Response.ContentType = "application/json"

            Try
                Dim result As Object = Nothing
                Dim command = context.Request.Form("command")

                If command = "add" Then
                    ' add the category
                    Dim parentId As Integer = Integer.Parse(context.Request.Form("parentId"))
                    Dim categoryName As String = context.Request.Form("categoryName")
                    Dim categoryNumber As String = context.Request.Form("categoryNumber")
                    AddCategory(parentId, categoryName, categoryNumber)
                    result = GetCategories()
                ElseIf command = "modify" Then
                    ' modify the category
                    Dim categoryId As Integer = Integer.Parse(context.Request.Form("categoryId"))
                    Dim categoryName As String = context.Request.Form("categoryName")
                    Dim categoryNumber As String = context.Request.Form("categoryNumber")
                    ModifyCategory(categoryId, categoryName, categoryNumber)
                    result = GetCategories()
                ElseIf command = "delete" Then
                    Dim categoryId As Integer = Integer.Parse(context.Request.Form("categoryId"))
                    DetailRepository.DeleteCategory(categoryId)
                    result = GetCategories()
                ElseIf command = "get-children" Then
                    Dim categoryId As Integer = Integer.Parse(context.Request.Form("categoryId"))
                    result = GetChildCategories(categoryId)
                Else
                    ' Default command (used in Categories.aspx)
                    result = GetCategories()
                End If

                Dim settings As New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore}
                context.Response.Write(JsonConvert.SerializeObject(result, Formatting.None, settings))
            Catch ex As Exception
                Dim errmsg = ex.Message
                context.Response.StatusCode = 500
                context.Response.Write(JsonConvert.SerializeObject(New With {.message = errmsg}))
            End Try

        End Sub

        Private Function GetCategories() As IEnumerable(Of TreeNode)
            Dim list As New List(Of TreeNode)

            Dim parents As IEnumerable(Of Models.Category) = DetailRepository.GetParentCategories().OrderBy(Function(x) x.CategoryNumberToDouble())

            For Each p As Models.Category In parents
                Dim pnode As TreeNode = CreateTreeNode(p)

                Dim children As IEnumerable(Of Models.Category) = DetailRepository.GetChildCategories(p.CategoryID).OrderBy(Function(x) x.CategoryNumberToDouble())
                If children.Count() > 0 Then
                    pnode.Nodes = children.Select(Function(x) CreateTreeNode(x))
                Else
                    pnode.Nodes = Nothing
                End If

                list.Add(pnode)
            Next

            Return list
        End Function

        Private Function GetChildCategories(categoryId As Integer) As IEnumerable(Of Models.Category)
            Dim result As List(Of Models.Category) = DetailRepository.GetChildCategories(categoryId).OrderBy(Function(x) x.CategoryNumberToDouble()).ToList()
            Return result
        End Function

        Private Function CreateTreeNode(c As Models.Category) As TreeNode
            Dim node As New TreeNode($"[{c.CategoryNumber}] {c.CategoryName}") With {
                .CategoryID = c.CategoryID,
                .CategoryName = c.CategoryName,
                .CategoryNumber = c.CategoryNumber,
                .IsParent = c.ParentID = 0
            }
            Return node
        End Function

        Private Sub AddCategory(parentId As Integer, categoryName As String, categoryNumber As String)
            If String.IsNullOrEmpty(categoryNumber) Then
                Throw New Exception("Category number is required.")
            End If

            If String.IsNullOrEmpty(categoryName) Then
                Throw New Exception("Category name is required.")
            End If

            DetailRepository.AddCategory(parentId, categoryName, categoryNumber)
        End Sub

        Private Sub ModifyCategory(categoryId As Integer, categoryName As String, categoryNumber As String)
            If String.IsNullOrEmpty(categoryNumber) Then
                Throw New Exception("Category number is required.")
            End If

            If String.IsNullOrEmpty(categoryName) Then
                Throw New Exception("Category name is required.")
            End If

            DetailRepository.ModifyCategory(categoryId, categoryName, categoryNumber)
        End Sub
    End Class

    Public Class TreeNode
        Public Sub New(text As String)
            Me.Text = text
        End Sub

        <JsonProperty("text")>
        Public Property Text As String

        <JsonProperty("nodes")>
        Public Property Nodes As IEnumerable(Of TreeNode)

        <JsonProperty("categoryId")>
        Public Property CategoryID As Integer

        <JsonProperty("categoryName")>
        Public Property CategoryName As String

        <JsonProperty("categoryNumber")>
        Public Property CategoryNumber As String

        <JsonProperty("isParent")>
        Public Property IsParent As Boolean
    End Class
End Namespace