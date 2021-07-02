Imports IOF.Models

Namespace Controls
    Public Class PurchaseOrderDetailView
        Inherits IOFControl

        Public Event TitleCommand As CommandEventHandler
        Public Event ItemDescriptionCommand As CommandEventHandler
        Public Event RowDataBound As GridViewRowEventHandler
        Private _Order As Order
        Private _Items As IEnumerable(Of Detail)

        Public Property POID As Integer
            Get
                Dim result As Integer = 0
                Integer.TryParse(hidPOID.Value, result)
                Return result
            End Get
            Set(value As Integer)
                hidPOID.Value = value.ToString()
            End Set
        End Property

        Public Property HideTitle As Boolean
        Public Property EnableTitleLink As Boolean
        Public Property EnableItemDescriptionLink As Boolean
        Public Property TitleCommandName As String
        Public Property ItemDescriptionCommandName As String
        Public Property PurchaserMode As Boolean
        Public Property CanDeleteItems As Boolean
        Public Property UseAjax As Boolean = True
        Public ReadOnly Property Total As Double = 0

        Public ReadOnly Property Order As Order
            Get
                Return _Order
            End Get
        End Property

        Public ReadOnly Property Items As IEnumerable(Of Detail)
            Get
                Return _Items
            End Get
        End Property

        Public ReadOnly Property RowCount As Integer
            Get
                Return _Items.Count
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                If _HideTitle Then
                    btnPurchaseOrderItems.Visible = False
                    lblPurchaseOrderItems.Visible = False
                Else
                    btnPurchaseOrderItems.Visible = _EnableTitleLink
                    lblPurchaseOrderItems.Visible = Not _EnableTitleLink
                End If
            End If
        End Sub

        Public Overrides Sub DataBind()
            btnPurchaseOrderItems.CommandArgument = POID.ToString()

            _Order = OrderRepository.Single(POID)
            _Items = DetailRepository.GetOrderDetails(POID)

            rptItems.DataSource = _Items
            rptItems.DataBind()

            phNoData.Visible = _Items.Count() <= 0
            rptItems.Visible = _Items.Count() > 0

            phUpdateOption.Visible = PurchaserCanModify()
        End Sub

        Protected Sub btnPurchaseOrderItems_Command(sender As Object, e As CommandEventArgs)
            RaiseEvent TitleCommand(sender, e)
        End Sub

        Protected Sub btnItemDescriptionLink_Command(sender As Object, e As CommandEventArgs)
            RaiseEvent ItemDescriptionCommand(sender, e)
        End Sub

        Protected Function PurchaserCanModify() As Boolean
            If PurchaserMode Then
                Return (Order.StatusID = Status.Ordered OrElse Order.StatusID = Status.Approved OrElse Order.StatusID = Status.ProcessedManually) AndAlso Order.PurchaserID.GetValueOrDefault() = IOFContext.CurrentUser.ClientID
            Else
                Return False
            End If
        End Function

        Protected Function GetCategoryClass(item As Detail) As String
            Dim action As String = Request.QueryString("Action")

            If action = "New" OrElse action = "Unfinished" OrElse action = "UseExisting" Then
                If Not item.IsCategoryActive() Then
                    Return "category inactive"
                End If
            End If

            Return "category"
        End Function

        Protected Sub rptItems_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
            If e.Item.ItemType = ListItemType.Footer Then
                Dim litTotalExtPrice As Literal = CType(e.Item.FindControl("litTotalExtPrice"), Literal)
                litTotalExtPrice.Text = Items.Sum(Function(x) x.ExtPrice).ToString("C")
            End If
        End Sub

        Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
            Dim splitter = e.CommandArgument.ToString().Split(":"c)
            Dim args = New With {.index = Integer.Parse(splitter(0)), .podid = Integer.Parse(splitter(1))}

            If e.CommandName = "delete" Then
                DetailRepository.Delete(args.podid)
            End If

            DataBind()
        End Sub
    End Class
End Namespace