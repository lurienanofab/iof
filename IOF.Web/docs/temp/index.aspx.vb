Imports System.IO

Namespace Docs
    Public Class Index
        Inherits IOFPage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                LoadFileList()
            End If
        End Sub

        Protected Sub btnDeleteAll_Click(ByVal sender As Object, ByVal e As EventArgs)
            Alert1.Hide()

            Dim paths() As String = Directory.GetFiles(Server.MapPath("."), "*.pdf")

            Dim errmsg As String = String.Empty

            For Each p As String In paths
                Try
                    File.Delete(p)
                Catch ex As Exception
                    errmsg += ex.Message + "<br />"
                End Try
            Next

            If Not String.IsNullOrEmpty(errmsg) Then
                Alert1.Show(errmsg)
            End If

            LoadFileList()
        End Sub

        Private Sub LoadFileList()
            Alert1.Hide()

            Dim paths() As String = Directory.GetFiles(Server.MapPath("."), "*.pdf")

            Dim fileList As New List(Of Object)

            If paths.Length > 0 Then
                For Each p As String In paths
                    Dim fileName As String = Path.GetFileName(p)
                    fileList.Add(New With {.FileName = fileName, .FileUrl = VirtualPathUtility.ToAbsolute($"~/docs/temp/{fileName}")})
                Next
            End If

            btnDeleteAll.Visible = fileList.Count > 0
            phNoData.Visible = fileList.Count = 0

            rptFiles.DataSource = fileList
            rptFiles.DataBind()
        End Sub

    End Class
End Namespace