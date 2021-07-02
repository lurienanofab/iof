Imports IOF.Models

Public Class ApprovalProcess
    Inherits IOFPage

    Public Property Parameters As ApprovalProcessParameters

    Public Property ShowTrackingInfo As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                Setup()

                'Make sure this IOF has not been processed before

                If ValidateStatus() Then
                    If Parameters.Action = "Approve" Then
                        ' Approve PO - Update Status to Approved
                        Approve()
                    ElseIf Parameters.Action = "Reject" Then
                        ' Reject PO - Update Status to Draft
                        Reject()
                    ElseIf Parameters.Action = "ApproveOrReject" Then
                        ' Approve or Reject PO - Show buttons
                        phApproveOrReject.Visible = True
                    Else
                        Throw New Exception("Invalid parameter: Action")
                    End If
                End If
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Private Sub Setup()
        Alert1.Hide()
        GetParameters()
        ValidateApprover()
    End Sub

    Private Sub ValidateApprover()
        Dim signedIn As Boolean = True

        Dim approver As Client = ClientRepository.Single(Parameters.ApproverID)

        If Not Context.User.Identity.IsAuthenticated Then
            signedIn = False
        Else
            If Context.User.Identity.Name <> approver.UserName Then
                signedIn = False
            End If
        End If

        If Not signedIn Then
            IOFContext.SignIn(approver)
        End If

        lblApproverName.Text = approver.DisplayName
    End Sub

    Private Function ValidateStatus() As Boolean
        Dim po As Order = OrderRepository.Single(Parameters.POID)
        If po.StatusID = Status.AwaitingApproval Then
            Return True
        Else
            ShowInvalidStatusError(po)
            ShowTracking(po.POID)
            Return False
        End If
    End Function

    Private Sub ShowTracking(poid As Integer)
        If ShowTrackingInfo Then
            phTracking.Visible = True
            Tracking1.POID = poid
            Tracking1.LoadTrackingData()
        Else
            phTracking.Visible = False
        End If
    End Sub

    Private Sub ShowInvalidStatusError(po As Order)
        Alert1.Show($"The current status of IOF #{po.POID} is <strong>{po.StatusName}</strong>, and cannot be approved or rejected at this time.")
    End Sub

    Private Sub Approve()
        ' Approve PO - Update Status to Approved
        OrderRepository.Approve(Parameters.POID, Parameters.ApproverID)
        Dim filePath As String = PdfService.CreatePDF(Parameters.POID)
        EmailService.SendPurchaserEmail(Parameters.POID, filePath)
        ShowTracking(Parameters.POID)
        Alert1.Show($"Thank you. IOF #{Parameters.POID} has been approved!", BootstrapAlertType.Success)
    End Sub

    Private Sub Reject()
        OrderRepository.Reject(Parameters.POID, Parameters.ApproverID)
        ShowTracking(Parameters.POID)
        Alert1.Show($"IOF #{Parameters.POID} has been rejected!", BootstrapAlertType.Success)
        phReject.Visible = True
    End Sub

    Protected Sub BtnSendEmail_Click(sender As Object, e As EventArgs)
        Try
            GetParameters()
            EmailService.SendRejectEmail(Parameters.POID, txtRejectReason.Text)
            ShowTracking(Parameters.POID)
            Alert1.Show("Thank you. Your comments have been sent to IOF owner.", BootstrapAlertType.Success)
            phReject.Visible = False
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Protected Sub ApproveOrReject_Command(sender As Object, e As CommandEventArgs)
        Try
            GetParameters()

            phApproveOrReject.Visible = False

            If ValidateStatus() Then
                If e.CommandName = "approve" Then
                    Approve()
                ElseIf e.CommandName = "reject" Then
                    Reject()
                Else
                    Throw New Exception("Invalid parameter: CommandName")
                End If
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Private Sub GetParameters()
        If Request.QueryString("qs") IsNot Nothing Then
            Dim encrypted As String = Request.QueryString("qs")
            Parameters = EmailService.GetApprovalProcessParameters(encrypted)
        End If
    End Sub
End Class