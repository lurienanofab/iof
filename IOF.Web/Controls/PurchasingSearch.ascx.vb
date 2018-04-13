Imports IOF.Models

Namespace Controls
    <ParseChildren(True)>
    Public Class PurchasingSearch
        Inherits IOFControl

        Public Shared ReadOnly DateFormat As String = "MM/dd/yyyy"

        Public Event Search(ByVal sender As Object, ByVal e As PurchasingSearchEventArgs)

        Public Property OrderStatus As PurchaserOrderStatus

        Public Property Title As String
            Get
                Return litTitle.Text
            End Get
            Set(value As String)
                litTitle.Text = value
            End Set
        End Property

        Public Property StatusIdList As String
            Get
                Return hidStatusIDs.Value
            End Get
            Set(value As String)
                hidStatusIDs.Value = value
            End Set
        End Property

        Public Property DateRangePreset As String
            Get
                Return ddlDatePresets.SelectedValue
            End Get
            Set(ByVal value As String)
                ddlDatePresets.SelectedValue = value
                ApplyPreset()
            End Set
        End Property

        Public Property StartDate As DateTime?
            Get
                If String.IsNullOrEmpty(txtStartDate.Text) Then
                    Return Nothing
                Else
                    Dim result As DateTime
                    If DateTime.TryParse(txtStartDate.Text, result) Then
                        Return result
                    Else
                        Return Nothing
                    End If
                End If
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    txtStartDate.Text = value.Value.ToString(DateFormat)
                Else
                    txtStartDate.Text = String.Empty
                End If
                ddlDatePresets.SelectedValue = String.Empty
            End Set
        End Property

        Public Property EndDate As DateTime?
            Get
                Dim result As DateTime? = Nothing

                If Not String.IsNullOrEmpty(txtEndDate.Text) Then
                    Dim d As DateTime
                    If Not DateTime.TryParse(txtEndDate.Text, d) Then
                        result = Nothing
                    Else
                        result = d
                    End If
                End If

                If result.HasValue AndAlso result.Value < DateTime.MaxValue Then
                    Return result.Value.AddDays(1)
                Else
                    Return result
                End If
            End Get
            Set(value As DateTime?)
                If value.HasValue Then
                    txtEndDate.Text = value.Value.ToString(DateFormat)
                Else
                    txtEndDate.Text = String.Empty
                End If
                ddlDatePresets.SelectedValue = String.Empty
            End Set
        End Property

        Public Property ClaimStatus As PurchaserClaimStatus
            Get
                Return GetClaimStatus()
            End Get
            Set(ByVal value As PurchaserClaimStatus)
                SetClaimStatus(value)
            End Set
        End Property

        Public Property PurchaserClientID As Integer
            Get
                Dim result As Integer = 0
                Integer.TryParse(ddlPurchaser.SelectedValue, result)
                Return result
            End Get
            Set(ByVal value As Integer)
                ddlPurchaser.SelectedValue = value.ToString()
            End Set
        End Property

        Public Property CreatorClientID As Integer
            Get
                Dim result As Integer = 0
                Integer.TryParse(ddlCreator.SelectedValue, result)
                Return result
            End Get
            Set(ByVal value As Integer)
                ddlCreator.SelectedValue = value.ToString()
            End Set
        End Property

        Public Property POID As Integer
            Get
                Dim result As Integer
                Integer.TryParse(txtPOID.Text, result)
                Return result
            End Get
            Set(ByVal value As Integer)
                txtPOID.Text = If(value > 0, value.ToString(), String.Empty)
            End Set
        End Property

        Public Property RealPO As String
            Get
                Return txtRealPO.Text
            End Get
            Set(ByVal value As String)
                txtRealPO.Text = value
            End Set
        End Property

        Public Property PoidVisible As Boolean
            Get
                Return phPOID.Visible
            End Get
            Set(value As Boolean)
                phPOID.Visible = value
            End Set
        End Property

        Public Property PurchaserVisible As Boolean
            Get
                Return phPurchaser.Visible
            End Get
            Set(value As Boolean)
                phPurchaser.Visible = value
            End Set
        End Property

        Public Property CreatorVisible As Boolean
            Get
                Return phCreator.Visible
            End Get
            Set(value As Boolean)
                phCreator.Visible = value
            End Set
        End Property

        Public Property RealPoVisible As Boolean
            Get
                Return phRealPO.Visible
            End Get
            Set(value As Boolean)
                phRealPO.Visible = value
            End Set
        End Property

        Public Property DateRangeVisible As Boolean
            Get
                Return phDateRange.Visible
            End Get
            Set(value As Boolean)
                phDateRange.Visible = value
            End Set
        End Property

        Public Property Criteria As PurchaserSearchArgs
            Get
                Return New PurchaserSearchArgs() With {
                    .StartDate = StartDate,
                    .EndDate = EndDate,
                    .DateRangePreset = DateRangePreset,
                    .ClaimStatus = ClaimStatus,
                    .PurchaserClientID = PurchaserClientID,
                    .CreatorClientID = CreatorClientID,
                    .POID = POID,
                    .RealPO = RealPO,
                    .StatusIdList = StatusIdList,
                    .OrderStatus = OrderStatus}
            End Get
            Set(value As PurchaserSearchArgs)
                StartDate = value.StartDate
                EndDate = value.EndDate
                DateRangePreset = value.DateRangePreset
                ClaimStatus = value.ClaimStatus
                PurchaserClientID = value.PurchaserClientID
                CreatorClientID = value.CreatorClientID
                POID = value.POID
                RealPO = value.RealPO
                StatusIdList = value.StatusIdList
                OrderStatus = value.OrderStatus
            End Set
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
            If Not Page.IsPostBack Then
                ddlPurchaser.DataSource = ClientRepository.GetPurchasers().OrderBy(Function(x) x.DisplayName)
                ddlPurchaser.DataBind()
                ddlPurchaser.Items.Insert(0, New ListItem("-- All --", "0"))

                ddlCreator.DataSource = ClientRepository.GetActiveClients().OrderBy(Function(x) x.DisplayName)
                ddlCreator.DataBind()
                ddlCreator.Items.Insert(0, New ListItem("-- All --", "0"))
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            OnSearch(New PurchasingSearchEventArgs(Criteria))
        End Sub

        Protected Overridable Sub OnSearch(e As PurchasingSearchEventArgs)
            Dim sb As New StringBuilder()

            Dim sd As String = If(e.Criteria.StartDate.HasValue, e.Criteria.StartDate.Value.ToString(DateFormat), "[null]")
            Dim ed As String = If(e.Criteria.EndDate.HasValue, e.Criteria.EndDate.Value.ToString(DateFormat), "[null]")

            sb.AppendLine("<!--")
            sb.AppendLine("DoSearch Critera:")
            sb.AppendLine("------------------------------------------------")
            sb.AppendLine($"e.StartDate: {sd}")
            sb.AppendLine($"e.EndDate: {ed}")
            sb.AppendLine($"e.IncludeValue: {e.Criteria.ClaimStatus}")
            sb.AppendLine($"e.PurchaserClientID: {e.Criteria.PurchaserClientID}")
            sb.AppendLine($"e.CreatorClientID: {e.Criteria.CreatorClientID}")
            sb.AppendLine($"e.POID: {e.Criteria.POID}")
            sb.AppendLine($"e.RealPO: {e.Criteria.RealPO}")
            sb.AppendLine("-->")

            litDebug.Text = sb.ToString()
            RaiseEvent Search(Me, e)
        End Sub

        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path))
        End Sub

        Private Sub ApplyPreset()
            Dim startDate As String = String.Empty
            Dim endDate As String = String.Empty

            If String.IsNullOrEmpty(ddlDatePresets.SelectedValue) Then
                Return
            Else
                Dim preset As String = ddlDatePresets.SelectedValue
                Dim today As Date = DateTime.Now.Date
                Dim fom As Date = New DateTime(today.Year, today.Month, 1)
                Dim fow As Date = today.AddDays(-Convert.ToInt32(today.DayOfWeek))
                Dim foy As Date = New DateTime(today.Year, 1, 1)

                Select Case preset
                    Case "today"
                        startDate = today.ToString(DateFormat)
                        endDate = today.ToString(DateFormat)
                    Case "yesterday"
                        startDate = today.AddDays(-1).ToString(DateFormat)
                        endDate = today.AddDays(-1).ToString(DateFormat)
                    Case "this-week"
                        startDate = fow.ToString(DateFormat)
                        endDate = fow.AddDays(6).ToString(DateFormat)
                    Case "last-week"
                        startDate = fow.AddDays(-7).ToString(DateFormat)
                        endDate = fow.AddDays(1).ToString(DateFormat)
                    Case "this-month"
                        startDate = fom.ToString(DateFormat)
                        endDate = fom.AddMonths(1).AddDays(-1).ToString(DateFormat)
                    Case "last-month"
                        startDate = fom.AddMonths(-1).ToString(DateFormat)
                        endDate = fom.AddDays(-1).ToString(DateFormat)
                    Case "this-year"
                        startDate = foy.ToString(DateFormat)
                        endDate = fom.AddYears(1).AddDays(-1).ToString(DateFormat)
                    Case Else
                        Return
                End Select
            End If

            txtStartDate.Text = startDate
            txtEndDate.Text = endDate
        End Sub

        Private Function GetStatusIDs() As Integer()
            Return hidStatusIDs.Value.Split(","c).Select(Function(x) Integer.Parse(x)).ToArray()
        End Function

        Private Function GetClaimStatus() As PurchaserClaimStatus
            If rdoClaimStatusAll.Checked Then
                Return PurchaserClaimStatus.All
            ElseIf rdoClaimStatusClaimed.Checked Then
                Return PurchaserClaimStatus.Claimed
            ElseIf rdoClaimStatusClaimedBy.Checked Then
                Return PurchaserClaimStatus.ClaimedBy
            ElseIf rdoClaimStatusUnclaimed.Checked Then
                Return PurchaserClaimStatus.Unclaimed
            Else
                Return PurchaserClaimStatus.All
            End If
        End Function

        Private Sub SetClaimStatus(value As PurchaserClaimStatus)
            rdoClaimStatusAll.Checked = value = PurchaserClaimStatus.All
            rdoClaimStatusClaimed.Checked = value = PurchaserClaimStatus.Claimed
            rdoClaimStatusClaimedBy.Checked = value = PurchaserClaimStatus.ClaimedBy
            rdoClaimStatusUnclaimed.Checked = value = PurchaserClaimStatus.Unclaimed
        End Sub
    End Class

    Public Class PurchasingSearchEventArgs
        Inherits EventArgs

        Private _Criteria As PurchaserSearchArgs

        Public ReadOnly Property Criteria As PurchaserSearchArgs
            Get
                Return _Criteria
            End Get
        End Property

        Public Sub New(ByVal criteria As PurchaserSearchArgs)
            _Criteria = criteria
        End Sub
    End Class
End Namespace