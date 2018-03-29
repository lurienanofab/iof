Imports IOF.Models

Public Class Index
    Inherits IOFPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim currentUser As Client = IOFContext.CurrentUser

            Dim summary As OrderSummary = OrderRepository.GetOrderSummary(currentUser.ClientID)

            'StatusID    Status
            '-----------------------------------
            '1           Draft
            '2           Awaiting Approval
            '3           Approved
            '4           Ordered
            '5           Completed
            '6           Cancelled
            '7           Processed Manually

            lblUnfinishedIOF.Text = summary.Draft.ToString()
            lblApprovalPendingIOF.Text = summary.AwaitingApproval.ToString()
            lblApprovedIOF.Text = summary.Approved.ToString()
            lblOrderedIOF.Text = summary.Ordered.ToString()

            hypApprovalList.NavigateUrl = $"~/ApprovalList.aspx?filter={currentUser.ClientID}"
            hypPurchaseList.NavigateUrl = $"~/PurchaseList.aspx?filter={currentUser.ClientID}"
            hypOrderedIOF.NavigateUrl = $"~/OrderedIOF.aspx?filter={currentUser.ClientID}"

            lblUserName.Text = currentUser.DisplayName

            SearchCriteria.Clear()
        End If
    End Sub

End Class