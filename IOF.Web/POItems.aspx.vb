Imports IOF.Models

Public Class POItems
    Inherits IOFPage

    Protected ShowModalDialog As Boolean

    Public Enum ItemState
        AddNew = 1
        AddExisting = 2
        ModifyItem = 3
    End Enum

    Public Class FormArgs
        Public State As ItemState
        Public ItemID As Integer = 0
        Public PartNum As String = String.Empty
        Public Description As String = String.Empty
        Public CategoryID As Integer = 0
        Public CategoryParentID As Integer = 0
        Public Quantity As Double = 1
        Public Unit As String = String.Empty
        Public UnitPrice As Double = 0
        Public InventoryItemID As Integer = 0
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetErrItem(String.Empty)
        lblErrItems.Text = String.Empty

        LoadPurchaseOrderDetails()

        If Not Page.IsPostBack Then
            LoadItems()
            LoadCategories()
            LoadInventoryItems()
        End If
    End Sub

    Private Sub PopulateItemForm(args As FormArgs)
        SetErrItem(String.Empty)
        ddlItems.SelectedValue = If(args.ItemID = 0, "-1", args.ItemID.ToString())
        If args.CategoryID <> 0 AndAlso args.CategoryParentID = 0 Then
            If ddlCat1.Items.FindByValue(args.CategoryID.ToString()) IsNot Nothing Then
                ddlCat1.SelectedValue = args.CategoryID.ToString()
            Else
                ddlCat1.SelectedIndex = 0
            End If
            phChildCategory.Visible = False
        ElseIf args.CategoryID <> 0 AndAlso args.CategoryParentID <> 0 Then
            ddlCat1.SelectedValue = args.CategoryParentID.ToString()
            LoadSubCategories()
            If ddlCat2.Items.FindByValue(args.CategoryID.ToString()) IsNot Nothing Then
                ddlCat2.SelectedValue = args.CategoryID.ToString()
            Else
                ddlCat2.SelectedIndex = -1
            End If
        ElseIf args.CategoryID = 0 AndAlso args.CategoryParentID = 0 Then
            ddlCat1.SelectedIndex = 0
            LoadSubCategories()
        End If

        hidPartNum.Value = args.PartNum
        txtPartNum.Text = args.PartNum

        hidDescription.Value = args.Description
        txtDescription.Text = args.Description

        txtQuantity.Text = args.Quantity.ToString()
        txtUnit.Text = args.Unit

        If args.UnitPrice = 0 Then
            txtUnitPrice.Text = String.Empty
        Else
            txtUnitPrice.Text = args.UnitPrice.ToString("0.00")
        End If

        ddlInventoryItem.SelectedValue = args.InventoryItemID.ToString()

        Select Case args.State
            Case ItemState.AddNew
                lblItemDetailsTitle.Text = "Add Item"
                phItemsList.Visible = True
                btnAddItem.Visible = True
                btnUpdateItem.Visible = False
                btnCancelUpdate.Visible = False
                txtPartNum.Visible = True
                txtDescription.Visible = True
            Case ItemState.AddExisting
                lblItemDetailsTitle.Text = "Add Item"
                phItemsList.Visible = True
                btnAddItem.Visible = True
                btnUpdateItem.Visible = False
                btnCancelUpdate.Visible = False
                txtPartNum.Visible = True
                txtDescription.Visible = True
            Case ItemState.ModifyItem
                lblItemDetailsTitle.Text = "Modify Item"
                phItemsList.Visible = False
                btnAddItem.Visible = False
                btnUpdateItem.Visible = True
                btnCancelUpdate.Visible = True
                txtPartNum.Visible = True
                txtDescription.Visible = True
        End Select
    End Sub

    Private Sub LoadItems()
        Dim order As Order = OrderRepository.Single(POID)
        Dim vendorId As Integer = order.VendorID
        ddlItems.DataSource = ItemRepository.GetVendorItems(order.VendorID).OrderBy(Function(x) x.Description)
        ddlItems.DataBind()
        ddlItems.Items.Insert(0, New ListItem("-- To copy a previously purchased item, select it from list --", "-1"))
    End Sub

    Protected Sub ddlItems_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlItems.SelectedIndexChanged
        Dim args As New FormArgs()
        If ddlItems.SelectedValue = "-1" Then
            args.State = ItemState.AddNew
            PopulateItemForm(args)
        Else
            Dim itemId As Integer = Convert.ToInt32(ddlItems.SelectedValue)
            Dim item As Item = ItemRepository.Single(itemId)
            Dim mostRecentDetail As Detail = DetailRepository.GetItemDetails(itemId).OrderByDescending(Function(x) x.CreatedDate).FirstOrDefault()

            args.State = ItemState.AddExisting
            args.ItemID = item.ItemID
            args.PartNum = item.PartNum
            args.Description = item.Description
            args.UnitPrice = item.UnitPrice
            args.InventoryItemID = item.InventoryItemID.GetValueOrDefault()
            If mostRecentDetail IsNot Nothing Then
                args.CategoryID = mostRecentDetail.CategoryID
                args.CategoryParentID = mostRecentDetail.ParentID
                args.Unit = mostRecentDetail.Unit
            End If
            PopulateItemForm(args)

            btnAddItem.Visible = True
            btnUpdateItem.Visible = False
        End If

        LoadPurchaseOrderDetails()
    End Sub

    Private Sub LoadCategories()
        ddlCat1.DataSource = DetailRepository.GetParentCategories().OrderBy(Function(x) x.CategoryNumberToDouble())
        ddlCat1.DataBind()
        ddlCat1.SelectedIndex = 0
        LoadSubCategories()
    End Sub

    Private Sub LoadSubCategories()
        Dim parentId As Integer = Convert.ToInt32(ddlCat1.SelectedValue)
        ddlCat2.DataSource = DetailRepository.GetChildCategories(parentId).OrderBy(Function(x) x.CategoryNumberToDouble())
        ddlCat2.DataBind()
        phChildCategory.Visible = ddlCat2.Items.Count > 0
    End Sub

    Private Sub LoadInventoryItems()
        If StoreManager Then
            phInventoryItem.Visible = True
            ddlInventoryItem.DataSource = ItemRepository.GetInventoryItems().OrderBy(Function(x) x.Description)
            ddlInventoryItem.DataBind()
            ddlInventoryItem.Items.Insert(0, New ListItem("-- Select --", "0"))
        End If
    End Sub

    Protected Sub ddlCat1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlCat1.SelectedIndexChanged
        LoadSubCategories()
        LoadPurchaseOrderDetails()
    End Sub

    Protected Sub btnAddItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddItem.Click
        Try
            Dim catId As Integer = GetSelectedCatID()
            Dim podid As Integer = GetSelectedPODID()

            LoadPurchaseOrderDetails()

            If PurchaseOrderDetailView1.Items Is Nothing Then
                Return
            End If

            If String.IsNullOrEmpty(txtDescription.Text) Then
                SetErrItem("Description is required.")
                Return
            End If

            Dim details As IEnumerable(Of Detail) = DetailRepository.GetOrderDetails(POID)

            ' Check if Part Num & Description Exists in POD
            If IOFUtility.HasDuplicate(details, txtPartNum.Text, txtDescription.Text, catId, podid) <> -1 Then
                SetErrItem("This item already exists in the current purchase order.")
            Else
                ' Check if Part Num & Description Exists in DB Items table
                ShowModalDialog = False

                Dim po As Order = OrderRepository.Single(POID)
                Dim vendorItems As IEnumerable(Of Item) = ItemRepository.GetVendorItems(po.VendorID)
                Dim itemId As Integer = IOFUtility.HasDuplicate(vendorItems, txtPartNum.Text, txtDescription.Text)

                If itemId = -1 Then
                    ' If item doesn't exist already, add it to DB Items table
                    Dim unitPrice As Double = 0
                    Dim qty As Double = 0
                    If Double.TryParse(txtUnitPrice.Text, unitPrice) Then
                        If Double.TryParse(txtQuantity.Text, qty) Then
                            Dim inventoryItemId As Integer?
                            If GetInventoryItemID(ddlInventoryItem, inventoryItemId) Then
                                Dim item As Item = ItemRepository.Add(txtPartNum.Text, txtDescription.Text, unitPrice, po.VendorID, inventoryItemId)

                                itemId = item.ItemID

                                LoadItems()
                                ddlItems.SelectedValue = itemId.ToString()

                                ' Add into POD
                                DetailRepository.Add(po.POID, item.ItemID, catId, qty, txtUnit.Text, unitPrice)

                                PopulateItemForm(New FormArgs With {.State = ItemState.AddNew})

                                SetErrItem("Item added to purchase order.")
                            Else
                                SetErrItem("This item must be linked to a store item. You may have to create the store item first if it does not yet exist.")
                            End If
                        Else
                            SetErrItem("Invalid Quantity. Please enter a numeric value.")
                        End If
                    Else
                        SetErrItem("Invalid Unit Price. Please enter a numeric value.")
                    End If
                Else
                    ' If item already exists, ask if user wants to update the existing item
                    If chkAutoOverwrite.Checked Then
                        Overwrite(itemId, po.VendorID)
                    Else
                        ShowModalDialog = True
                        Exit Sub
                    End If
                End If
            End If

            LoadPurchaseOrderDetails()
        Catch ex As Exception
            SetErrItem(ex.Message)
        End Try
    End Sub

    Protected Sub btnOverwriteDB_Click(ByVal sender As Object, ByVal e As EventArgs)
        ShowModalDialog = False
        Dim po As Order = OrderRepository.Single(POID)
        Dim items As IEnumerable(Of Item) = ItemRepository.GetOrderItems(POID)
        Dim itemId As Integer = IOFUtility.HasDuplicate(items, txtPartNum.Text, txtDescription.Text)
        Overwrite(itemId, po.VendorID)
    End Sub

    Private Sub Overwrite(itemId As Integer, vendorId As Integer)
        Try
            Dim inventoryItemId As Integer?
            If GetInventoryItemID(ddlInventoryItem, inventoryItemId) Then
                ' Item already exists, update CatID, UnitPrice in DB Items table
                ItemRepository.Update(itemId, txtPartNum.Text, txtDescription.Text, Double.Parse(txtUnitPrice.Text), inventoryItemId)

                LoadItems()

                ddlItems.SelectedValue = itemId.ToString()

                ' Add into POD
                Dim catId As Integer = GetSelectedCatID()
                DetailRepository.Add(POID, itemId, catId, Double.Parse(txtQuantity.Text), txtUnit.Text, Double.Parse(txtUnitPrice.Text))

                LoadPurchaseOrderDetails()

                PopulateItemForm(New FormArgs With {.State = ItemState.AddNew})

                SetErrItem("Item is added to purchase order.")
            Else
                SetErrItem("This item must be linked to a store item. You may have to create the store item first if it does not yet exist.")
            End If
        Catch ex As Exception
            SetErrItem(ex.Message)
        End Try
    End Sub

    Protected Function GetSelectedCatID() As Integer
        Return If(phChildCategory.Visible, Convert.ToInt32(ddlCat2.SelectedValue), Convert.ToInt32(ddlCat1.SelectedValue))
    End Function

    Protected Function GetSelectedPODID() As Integer
        If String.IsNullOrEmpty(hidSelectedPODID.Value) Then
            Return 0
        Else
            Return Convert.ToInt32(hidSelectedPODID.Value)
        End If
    End Function

    Protected Sub btnUpdateItem_Click(sender As Object, e As EventArgs)
        Try
            LoadPurchaseOrderDetails()

            Dim catId As Integer = GetSelectedCatID()
            Dim podid As Integer = GetSelectedPODID()
            Dim unitPrice As Double

            If Double.TryParse(txtUnitPrice.Text, unitPrice) Then
                ' Check if Part No & Description has changed
                If txtPartNum.Text <> hidPartNum.Value OrElse (String.IsNullOrEmpty(txtPartNum.Text) AndAlso String.IsNullOrEmpty(hidPartNum.Value) AndAlso txtDescription.Text <> hidDescription.Value) Then
                    ' Part No or Description have changed, check if item already exists in POD
                    Dim details As IEnumerable(Of Detail) = DetailRepository.GetOrderDetails(POID)

                    If IOFUtility.HasDuplicate(details, txtPartNum.Text, txtDescription.Text, catId, podid) <> -1 Then
                        ' Item already exists in POD, stop and give warning
                        SetErrItem("This item already exists in the current purchase order.")
                    Else
                        ' Item does not exist in POD, delete current one in POD
                        DetailRepository.Delete(podid)

                        ' Add new item like new
                        btnAddItem_Click(btnAddItem, Nothing)
                    End If
                Else
                    ' PartNum or Description have not changed, change all applicable fields
                    Dim po As Order = OrderRepository.Single(POID)
                    Dim inventoryItemId As Integer?
                    If GetInventoryItemID(ddlInventoryItem, inventoryItemId) Then
                        Dim itemId As Integer = Convert.ToInt32(ddlItems.SelectedValue)

                        Dim qty = Double.Parse(txtQuantity.Text)

                        ItemRepository.Update(itemId, txtPartNum.Text, txtDescription.Text, unitPrice, inventoryItemId)

                        DetailRepository.Update(podid, catId, qty, txtUnit.Text, unitPrice)

                        PopulateItemForm(New FormArgs With {.State = ItemState.AddNew})

                        SetErrItem("Item is updated.")
                    Else
                        SetErrItem("This item must be linked to a store item. You may have to create the store item first if it does not yet exist.")
                    End If
                End If

                LoadPurchaseOrderDetails()
            Else
                SetErrItem("Invalid unit price.")
            End If
        Catch ex As Exception
            SetErrItem(ex.Message)
        End Try
    End Sub

    Protected Sub btnCancelUpdate_Click(sender As Object, e As EventArgs) Handles btnCancelUpdate.Click
        PopulateItemForm(New FormArgs With {.State = ItemState.AddNew})
        LoadPurchaseOrderDetails()
    End Sub

    Private Function CleanString(ByVal inString As String) As String
        ' Convert to lower case and strips spaces
        Return inString.ToLower.Replace(" ", "")
    End Function

    Private Sub LoadPurchaseOrderDetails()
        If POID > 0 Then
            PurchaseOrderDetailView1.POID = POID
            PurchaseOrderDetailView1.DataBind()
        Else
            Response.Redirect("~")
        End If
    End Sub

    Protected Sub gvItems_RowCommand(sender As Object, e As CommandEventArgs)
        Dim pod As Detail
        If e.CommandName = "EditItem" Then
            LoadPurchaseOrderDetails()
            hidSelectedPODID.Value = e.CommandArgument.ToString()
            pod = DetailRepository.Single(GetSelectedPODID())
            Dim args As New FormArgs()
            args.State = ItemState.ModifyItem
            args.ItemID = pod.ItemID
            args.PartNum = pod.PartNum
            args.Description = pod.Description
            args.CategoryID = pod.CategoryID
            args.CategoryParentID = pod.ParentID
            args.Quantity = pod.Quantity
            args.Unit = pod.Unit
            args.UnitPrice = pod.UnitPrice
            args.InventoryItemID = pod.InventoryItemID.GetValueOrDefault()
            PopulateItemForm(args)
        ElseIf e.CommandName = "DeleteItem" Then
            'LoadPurchaseOrderDetails()
            Dim podid As Integer = Convert.ToInt32(e.CommandArgument)
            DetailRepository.Delete(podid)
        End If

        LoadPurchaseOrderDetails()
    End Sub

    Protected Sub btnSavePOItems_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSavePOItems.Click
        If PurchaseOrderDetailView1.RowCount = 0 Then
            lblErrItems.Text = "You must add one or more items to your purchase order."
        Else
            If FromPOID > 0 Then
                Response.Redirect($"~/POConfirm.aspx?Action={Action}&POID={POID}&FromPOID={FromPOID}")
            Else
                Response.Redirect($"~/POConfirm.aspx?Action={Action}&POID={POID}")
            End If
        End If
    End Sub

    Protected Sub btnCancelPOItems_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelPOItems.Click
        If Action = "UseExisting" Then
            Response.Redirect($"~/POConfirm.aspx?Action={Action}&POID={POID}&FromPOID={FromPOID}")
        Else
            Response.Redirect("~")
        End If
    End Sub

    Private Sub SetErrItem(msg As String)
        If String.IsNullOrEmpty(msg) Then
            phErrItem.Visible = False
            litErrItem.Text = String.Empty
        Else
            phErrItem.Visible = True
            litErrItem.Text = msg
        End If
    End Sub
End Class