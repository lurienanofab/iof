Imports IOF.Models

Public Class Items
    Inherits IOFPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Alert1.Hide()
            phStoreManager.Visible = IsStoreManager() 'this is true if the user has the necessary privilege
            chkStoreManager.Checked = StoreManager 'this is true if the user has checked the Store Manager checkbox
            phInventoryItem.Visible = StoreManager
            LoadVendors()
            LoadItems()
            LoadInventoryItems()
        End If
    End Sub

    Private Sub LoadVendors()
        ddlVendors.DataSource = VendorRepository.GetActiveVendors(ActiveClientID).OrderBy(Function(x) x.VendorName)
        ddlVendors.DataBind()
        ddlVendors.SelectedIndex = 0

        If ddlVendors.Items.Count = 0 Then
            Alert1.Show("You must have at least one vendor before adding new items.")
            btnAddItem.Enabled = False
        End If
    End Sub

    Protected Overrides Sub OnStoreManagerCheckChanged(checked As Boolean)
        LoadVendors()
        LoadItems()
        LoadInventoryItems()
    End Sub

    Protected Sub ddlVendors_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlVendors.SelectedIndexChanged
        LoadItems()
    End Sub

    Private Function GetItems() As IEnumerable(Of Item)
        Dim vendorId As Integer = Integer.Parse(ddlVendors.SelectedValue)
        Dim all As Boolean? = Nothing
        Dim items As IEnumerable(Of Item) = ItemRepository.GetVendorItems(vendorId, all).OrderBy(Function(x) x.Description)
        Return items
    End Function

    Private Sub LoadItems()
        If ddlVendors.Items.Count > 0 Then
            litVendorName.Text = ddlVendors.SelectedItem.Text
            rptItems.DataSource = GetItems()
            rptItems.DataBind()
        End If
    End Sub

    Private Sub LoadInventoryItems()
        If StoreManager Then
            phInventoryItem.Visible = True
            ddlInventoryItem.DataSource = ItemRepository.GetInventoryItems().OrderBy(Function(x) x.Description)
            ddlInventoryItem.DataBind()
            ddlInventoryItem.Items.Insert(0, New ListItem("-- Select --", "0"))
        Else
            phInventoryItem.Visible = False
        End If
    End Sub

    Protected Sub btnAddItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddItem.Click
        Try
            ' Check if Part Num & Description Exists in DB Items table
            Dim vendorId As Integer = Integer.Parse(ddlVendors.SelectedValue)
            Dim vendorItems As IEnumerable(Of Item) = ItemRepository.GetVendorItems(vendorId)
            Dim itemId As Integer = IOFUtility.HasDuplicate(vendorItems, txtPartNum.Text, txtDescription.Text)

            If itemId = -1 Then
                ' If item doesn't exist already, add it to database
                ItemRepository.Add(txtPartNum.Text, txtDescription.Text, Double.Parse(txtUnitPrice.Text), vendorId, Nothing)
                LoadItems()
                ClearItemForm()
            Else
                ' If item already exists, ask if user wants to update the existing item
                ClearItemForm()
                Alert1.Show("Item already exists in the database.")
            End If
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Protected Sub btnUpdateItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateItem.Click
        Try
            ' Check if Part Num & Description Exists in DB Items table
            Dim items As IEnumerable(Of Item) = GetItems()
            Dim itemId As Integer = IOFUtility.HasDuplicate(items, txtPartNum.Text, txtDescription.Text, Integer.Parse(lblItemID.Text))

            If itemId = -1 Then
                Dim unitPrice As Double
                If Double.TryParse(txtUnitPrice.Text, unitPrice) Then
                    ' If item doesn't exist already, modify it

                    Dim inventoryItemId As Integer?
                    GetInventoryItemID(ddlInventoryItem, inventoryItemId)

                    ItemRepository.Update(itemId, txtPartNum.Text, txtDescription.Text, unitPrice, inventoryItemId)

                    LoadItems()
                Else
                    Alert1.Show("Invalid unit price.")
                End If
            Else
                Alert1.Show("Another item with the same Part Number or Description already exists in the database.")
            End If

            ClearItemForm()

            btnClear.Text = "Clear"
            btnAddItem.Visible = True
            btnUpdateItem.Visible = False
        Catch ex As Exception
            Alert1.Show(ex.Message)
        End Try
    End Sub

    Private Sub PopulateItemForm(item As Item)
        Alert1.Hide()
        lblItemID.Text = item.ItemID.ToString()
        txtPartNum.Text = item.PartNum
        txtDescription.Text = item.Description
        txtUnitPrice.Text = item.UnitPrice.ToString("###0.00")
        If StoreManager Then
            ddlInventoryItem.SelectedValue = item.InventoryItemID.ToString()
        End If
    End Sub

    Private Sub ClearItemForm()
        Alert1.Hide()
        lblItemID.Text = String.Empty
        txtPartNum.Text = String.Empty
        txtDescription.Text = String.Empty
        txtUnitPrice.Text = String.Empty
        If StoreManager Then
            ddlInventoryItem.SelectedValue = "0"
        End If
    End Sub

    Protected Function IsDeleted(i As Item) As Boolean
        Return Not i.Active
    End Function

    Protected Function ItemDeleteCommandName(i As Item) As String
        If IsDeleted(i) Then
            Return "Restore"
        Else
            Return "Delete"
        End If
    End Function

    Protected Sub btnClear_Click(sender As Object, e As EventArgs)
        ClearItemForm()
        btnClear.Text = "Clear"
        btnAddItem.Visible = True
        btnUpdateItem.Visible = False
    End Sub

    Protected Sub Row_Command(sender As Object, e As CommandEventArgs)
        Dim itemId As Integer = Convert.ToInt32(e.CommandArgument)
        Dim item As Item = ItemRepository.Single(itemId)

        If item IsNot Nothing Then
            Select Case e.CommandName
                Case "modify"
                    PopulateItemForm(item)
                    btnClear.Text = "Cancel"
                    btnAddItem.Visible = False
                    btnUpdateItem.Visible = True
                Case "delete"
                    ItemRepository.Delete(itemId)
                    LoadItems()
                Case "restore"
                    ItemRepository.Restore(itemId)
                    LoadItems()
            End Select
        End If
    End Sub
End Class