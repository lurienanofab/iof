Imports System.Xml
Imports IOF.Models
Imports LNF.Ordering

Public Class TrackIOF
    Inherits IOFPage

    Public Enum CheckPoint
        Ordered = 7
        Cancelled = 11
    End Enum


    Public Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Action = "Detail" Then
            ShowTrackingDetail()
        Else
            ShowTrackingList()
            SearchCriteria.UseDefaultStartDate()
            SearchCriteria.UseDefaultEndDate()
            SearchCriteria.UseOtherClientID(IOFContext.CurrentUser.ClientID)
            PurchaseOrderSearch1.Action = VirtualPathUtility.ToAbsolute("~/TrackIOF.aspx?Action=Detail&POID={poid}")
        End If
    End Sub

    Public Sub ShowTrackingDetail()
        phSearch.Visible = False
        phDetail.Visible = True
        lblPOID.Text = POID.ToString()
        rptTracking.DataSource = TrackingUtility.SelectTrackingData(POID)
        rptTracking.DataBind()
    End Sub

    Public Sub ShowTrackingList()
        phSearch.Visible = True
        phDetail.Visible = False
        lblPOID.Text = String.Empty
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("~/TrackIOF.aspx")
    End Sub

    Protected Sub btnView_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim po As Order = OrderRepository.Single(POID)

        Dim url As String = String.Empty

        Select Case po.StatusID
            Case Status.Draft
                url = $"~/POConfirm.aspx?Action=Unfinished&POID={POID}&ReturnTo=Track"
            Case Else
                url = $"~/POConfirm.aspx?Action=Search&POID={POID}&ReturnTo=Track"
        End Select

        Response.Redirect(url)
    End Sub

    Protected Function GetNotes(dataItem As Object) As String
        Dim result As String = String.Empty

        If dataItem Is Nothing Then
            Return result
        End If

        Dim row As DataRowView = CType(dataItem, DataRowView)

        If row Is Nothing Then
            Return result
        End If

        If row("TrackingData") Is DBNull.Value Then
            Return result
        End If

        Dim xml As String = row("TrackingData").ToString()
        Dim checkpointId As Integer = Convert.ToInt32(row("CheckpointID"))

        Dim xmlstr As String = xml.ToString()
        Dim cid As Integer

        If Not String.IsNullOrEmpty(xmlstr) AndAlso Integer.TryParse(checkpointId.ToString(), cid) Then
            Try
                Dim xdoc As New XmlDocument()
                xdoc.LoadXml(xmlstr)

                Dim xnode As XmlNode
                Dim attr As XmlAttribute

                xnode = xdoc.SelectSingleNode("/data/add[@key='notes']")
                If xnode IsNot Nothing Then
                    attr = xnode.Attributes("value")
                    If attr IsNot Nothing Then
                        result = xnode.Attributes("value").Value
                    End If
                End If

                xnode = Nothing
                attr = Nothing

                Dim cp As CheckPoint = CType(cid, CheckPoint)
                Select Case cp
                    Case CheckPoint.Ordered
                        xnode = xdoc.SelectSingleNode("/data/add[@key='RealPOID']")
                        If xnode IsNot Nothing Then
                            attr = xnode.Attributes("value")
                            If attr IsNot Nothing Then
                                result += " Real PO: " + xnode.Attributes("value").Value
                            End If
                        End If
                End Select
            Catch
                'invalid xml was passed, do nothing
            End Try
        End If

        Return result.Trim()
    End Function
End Class