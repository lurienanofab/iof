Imports IOF.Models

Public Class ApprovalProcess
    Inherits IOFPage

    Public Property Parameters As ApprovalProcessParameters

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                Alert1.Hide()
                GetParameters()

                Dim po As Order = OrderRepository.Single(Parameters.POID)
                Dim approver As Approver = ClientRepository.GetApprover(po)

                If Not Request.IsAuthenticated Then
                    ' sign in as the approver
                    IOFContext.SignIn(approver)
                End If

                lblApproverName.Text = approver.DisplayName

                'Make sure this IOF has not been processed before
                'Dim realApprover As Client = ClientRepository.SingleClient(Parameters.ApproverID)

                'Dim errmsg As String = String.Empty
                If po.StatusID = Status.AwaitingApproval Then
                    If Parameters.Action = "Approve" Then
                        ' Approve PO - Update Status to Approved
                        OrderRepository.Approve(Parameters.POID, Parameters.ApproverID)
                        Dim filePath As String = PdfService.CreatePDF(Parameters.POID)
                        EmailService.SendPurchaserEmail(Parameters.POID, filePath)
                        Alert1.Show($"Thank you. IOF #{Parameters.POID} has been approved!", BootstrapAlertType.Success)
                    ElseIf Parameters.Action = "Reject" Then
                        ' Reject PO - Update Status to Draft
                        OrderRepository.Reject(Parameters.POID, Parameters.ApproverID)
                        Alert1.Show($"IOF #{Parameters.POID} has been rejected!", BootstrapAlertType.Success)
                        phReject.Visible = True
                    Else
                        Throw New Exception("Invalid Action parameter.")
                    End If
                Else
                    Alert1.Show($"The current status of IOF #{po.POID} is <strong>{po.StatusName}</strong>, and cannot be approved or rejected at this time.")
                End If
            Catch ex As Exception
                Alert1.Show(ex.Message)
            End Try
        End If
    End Sub

    Protected Sub btnSendEmail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSendEmail.Click
        Try
            GetParameters()
            EmailService.SendRejectEmail(Parameters.POID, txtRejectReason.Text)
            Alert1.Show("Thank you. Your comments have been sent to IOF owner.", BootstrapAlertType.Success)
            phReject.Visible = False
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