Imports IOF.Models

Namespace Controls
    Public Class PurchaseOrderHeaderView
        Inherits IOFControl

        Public Event Command As CommandEventHandler

        Private _Order As Order
        Private _Vendor As Vendor

        Public Property EnableLink As Boolean = False
        Public Property POID As Integer
        Public Property CopyPOID As Integer = 0

        Public ReadOnly Property AccountName As String
            Get
                Return litAccount.Text
            End Get
        End Property

        Public ReadOnly Property VendorName As String
            Get
                Return litVendorName.Text
            End Get
        End Property

        Public ReadOnly Property ApprovedBy As String
            Get
                Return litApprovedBy.Text
            End Get
        End Property

        Public ReadOnly Property NeededDate As String
            Get
                Return litNeededDate.Text
            End Get
        End Property

        Public ReadOnly Property Notes As String
            Get
                Return litNotes.Text
            End Get
        End Property

        Public ReadOnly Property Order As Order
            Get
                Return _Order
            End Get
        End Property

        Public ReadOnly Property Vendor As Vendor
            Get
                Return _Vendor
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                btnPurchaseOrderInfo.Visible = EnableLink
                lblPurchaseOrderInfo.Visible = Not EnableLink

                If _Order IsNot Nothing Then
                    If Not EnableLink AndAlso _Order.StatusID = Status.Draft Then
                        'this means someone else's draft IOF is being displayed, so show a message
                        litPOID.Text += " <span style=""color: #ff0000;"">[***DRAFT***]</span>"
                    End If
                End If
            End If
        End Sub

        Protected Sub btnPurchaseOrderInfo_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
            RaiseEvent Command(sender, e)
        End Sub

        Public Overrides Sub DataBind()
            _Order = OrderRepository.Single(POID)
            _Vendor = VendorRepository.Single(_Order.VendorID)

            btnPurchaseOrderInfo.CommandArgument = POID.ToString()

            litPOID.Text = $"<span>{_Order.POID}</span>"

            If CopyPOID <> 0 Then
                lblCopyIOF.Text = $"(Copied from POID {CopyPOID})"
                lblCopyIOF.Visible = True
            End If

            If _Order IsNot Nothing Then
                phRealPO.Visible = Not String.IsNullOrEmpty(_Order.RealPO)
                litRealPO.Text = _Order.RealPO
                litVendorName.Text = _Vendor.VendorName
                litAccount.Text = GetShortCode()
                litApprovedBy.Text = _Order.ApproverName
                litNeededDate.Text = _Order.NeededDate.ToShortDateString()
                litOversized.Text = If(_Order.Oversized, "Yes", "No")
                litShippingMethod.Text = _Order.ShippingMethodName
                litIsInventoryControlled.Text = If(OrderRepository.IsInventoryControlled(_Order.POID), "Yes", "No")
                litOrderedBy.Text = GetDisplayName()
                litNotes.Text = _Order.Notes.Replace(Convert.ToChar(10), "<br />")
                lblStatusID.Text = _Order.StatusID.ToString()
            End If

            MyBase.DataBind()
        End Sub

        Public Function GetShortCode() As String
            Return AccountRepository.GetShortCode(_Order.AccountID)
        End Function

        Public Function GetDisplayName() As String
            Dim result As String = _Order.DisplayName

            If _Vendor.ClientID <> _Order.ClientID Then
                If _Vendor.ClientID = 0 Then
                    result += " (Store Manager)"
                Else
                    Dim c As Client = ClientRepository.Single(Vendor.ClientID)
                    result += String.Format(" ({0})", c.DisplayName)
                End If
            End If

            Return result
        End Function
    End Class
End Namespace