Imports System.Xml

Namespace Controls
    Public Class Tracking
        Inherits UserControl

        Public Enum CheckPoint
            Ordered = 7
            Cancelled = 11
        End Enum

        Public Property POID As Integer

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

        Public Sub LoadTrackingData()
            Dim dataSource As DataTable = LNF.Ordering.Tracking.SelectTrackingData(POID)
            lblPOID.Text = POID.ToString()
            rptTracking.DataSource = dataSource
            rptTracking.DataBind()
        End Sub
    End Class
End Namespace