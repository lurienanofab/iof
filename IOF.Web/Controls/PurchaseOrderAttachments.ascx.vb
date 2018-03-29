Imports System.IO
Imports IOF.Models
Imports StructureMap.Attributes

Public Class PurchaseOrderAttachments
    Inherits IOFControl

    Public Event Upload As EventHandler(Of AttachmentEventArgs)
    Public Event Delete As EventHandler(Of AttachmentEventArgs)

    <SetterProperty>
    Public Property AttachmentService As IAttachmentService

    Public Property POID As Integer
        Get
            Return If(String.IsNullOrEmpty(hidPOID.Value), 0, Integer.Parse(hidPOID.Value))
        End Get
        Set(value As Integer)
            hidPOID.Value = value.ToString()
        End Set
    End Property

    Public Property [ReadOnly] As Boolean
        Get
            Return If(String.IsNullOrEmpty(hidReadOnly.Value), True, Boolean.Parse(hidReadOnly.Value))
        End Get
        Set(value As Boolean)
            hidReadOnly.Value = value.ToString()
        End Set
    End Property

    Public Sub LoadAttachments()
        If POID = 0 Then
            Throw New InvalidProgramException("POID must be set before calling LoadAttachments")
        End If

        phUpload.Visible = Not [ReadOnly]

        Dim attachments As IEnumerable(Of Attachment) = AttachmentService.GetAttachments(POID)

        If attachments.Count() > 0 Then
            rptAttachments.Visible = True
            phNoAttachments.Visible = False
            rptAttachments.DataSource = attachments
            rptAttachments.DataBind()
        Else
            rptAttachments.Visible = False
            phNoAttachments.Visible = True
        End If
    End Sub

    Protected Sub btnAddAttachment_Click(sender As Object, e As EventArgs)
        If fuAttachments.HasFiles Then
            Dim attachments As IEnumerable(Of Attachment)
            attachments = (From x In fuAttachments.PostedFiles
                           Select SaveAttachment(POID, x)).ToArray()
            LoadAttachments()
            RaiseEvent Upload(Me, New AttachmentEventArgs(POID, attachments))
        End If
    End Sub

    Protected Sub lbtnDeleteAttachment_Command(sender As Object, e As CommandEventArgs)
        If e.CommandName = "delete" Then
            Dim fileName As String = e.CommandArgument.ToString()

            Dim attachment = AttachmentService.DeleteAttachment(POID, fileName)

            LoadAttachments()

            RaiseEvent Delete(Me, New AttachmentEventArgs(POID, {attachment}))
        End If
    End Sub

    Private Function SaveAttachment(poid As Integer, postedFile As HttpPostedFile) As Attachment
        postedFile.InputStream.Position = 0

        Dim br As New BinaryReader(postedFile.InputStream)
        Dim data As Byte() = br.ReadBytes(postedFile.ContentLength)

        Dim result As Attachment = AttachmentService.SaveAttachment(poid, postedFile.FileName, data)

        Return result
    End Function
End Class