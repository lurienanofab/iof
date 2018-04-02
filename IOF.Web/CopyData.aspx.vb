Imports IOF.Models

Public Class CopyData
    Inherits IOFPage

    Public Function GetCopyToClientID() As Integer
        Dim result As Integer
        Dim temp As String = String.Empty

        If ddlCopyToClient.Visible AndAlso ddlCopyToClient.Items.Count > 0 Then
            temp = ddlCopyToClient.SelectedValue
        Else
            temp = hidCopyToClientID.Value
        End If

        If Not Integer.TryParse(temp, result) Then
            result = -1
        End If

        Return result
    End Function

    Public Function GetCopyFromClientID() As Integer
        Dim result As Integer = -1
        If ddlCopyFromClient.Items.Count > 0 AndAlso ddlCopyFromClient.SelectedValue <> "=x=" Then
            If Not Integer.TryParse(ddlCopyFromClient.SelectedValue, result) Then
                result = -1
            End If
        End If
        Return result
    End Function

    Public Function GetCopyFromVendorID() As Integer
        Dim result As Integer = 0
        If ddlCopyFromVendor.Items.Count > 0 Then
            If Not Integer.TryParse(ddlCopyFromVendor.SelectedValue, result) Then
                result = 0
            End If
        End If
        Return result
    End Function

    Public ReadOnly Property IncludeItems As Boolean
        Get
            Return chkCopyFromIncludeItems0.Checked
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            LoadData()
        End If
    End Sub

    Private Sub LoadData()
        Dim clients As IEnumerable(Of Client)

        Dim currentUser As Client = IOFContext.CurrentUser

        If IsAdministrator() OrElse IsApprover() Then
            clients = ClientRepository.GetActiveClients()
            ddlCopyToClient.DataSource = CreateClientListItems(clients)
            ddlCopyToClient.DataBind()
            If IsStoreManager() Then
                ddlCopyToClient.Items.Insert(0, New ListItem("Store Manager", "0"))
            End If
            ddlCopyToClient.SelectedValue = ActiveClientID.ToString()
            ddlCopyToClient.Visible = True
            phCopyToClientName.Visible = False
        ElseIf IsStoreManager() Then
            ddlCopyToClient.DataSource = New List(Of ListItem) From {New ListItem("Store Manager", "0"), New ListItem(currentUser.DisplayName, currentUser.ClientID.ToString())}
            ddlCopyToClient.DataBind()
            ddlCopyToClient.SelectedValue = ActiveClientID.ToString()
        Else
            litCopyToClientName.Text = currentUser.DisplayName
            hidCopyToClientID.Value = currentUser.ClientID.ToString()
            ddlCopyToClient.Visible = False
            phCopyToClientName.Visible = True
        End If

        ddlCopyFromClient.DataSource = CreateClientListItems(ClientRepository.GetClientsWithVendor())
        ddlCopyFromClient.DataBind()

        If IsStoreManager() Then
            ddlCopyFromClient.Items.Insert(0, New ListItem("Store Manager", "0"))
        End If

        ddlCopyFromClient.Items.Insert(0, New ListItem("-- Select --", "=x="))
    End Sub

    Protected Sub ddlCopyFromClient_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim clientId As Integer = GetCopyFromClientID()

        If clientId = -1 Then
            ddlCopyFromVendor.Items.Clear()
            Return
        End If

        ddlCopyFromVendor.DataSource = CreateVendorListItems(VendorRepository.GetActiveVendors(clientId))
        ddlCopyFromVendor.DataBind()
    End Sub

    Protected Sub btnCopyData_Click(sender As Object, e As EventArgs)
        Alert1.Hide()

        Dim toClientId As Integer = GetCopyToClientID()
        Dim fromClientId As Integer = GetCopyFromClientID()
        Dim fromVendorId As Integer = GetCopyFromVendorID()

        If toClientId = -1 Then
            Alert1.Show("A client to copy to must be specified.")
            Return
        End If

        If fromClientId = -1 Then
            Alert1.Show("A client to copy from must be specified.")
            Return
        End If

        If fromVendorId = 0 Then
            Alert1.Show("A vendor to copy from must be specified.")
            Return
        End If

        If (toClientId = 0 OrElse fromClientId = 0) AndAlso (Not IsStoreManager()) Then
            Alert1.Show("You do not have permission to use the Store Manager data.")
            Return
        End If

        Dim result As CopyDataResult = CopyData(toClientId, fromClientId, fromVendorId, IncludeItems)

        If result.Success Then
            If IncludeItems Then
                Dim i As Integer = result.Items.Count()
                Dim s As String = If(i <> 1, "s", String.Empty)
                Alert1.Show($"Data copy completed successfully! ({i} item{s} added)", BootstrapAlertType.Success)
            Else
                Alert1.Show("Data copy completed successfully!", BootstrapAlertType.Success)
            End If
        Else
            Alert1.Show("A problem occurred while trying to copy data. Please try again or contact the system administrator at <a href=""mailto:lnf-it@umich.edu"">lnf-it@umich.edu</a>")
        End If
    End Sub

    Public Function CopyData(toClientId As Integer, fromClientId As Integer, fromVendorId As Integer, includeItems As Boolean) As CopyDataResult
        Dim result As New CopyDataResult()

        Dim toVendor As Vendor = VendorRepository.Copy(toClientId, fromVendorId)

        If toVendor IsNot Nothing Then
            If includeItems Then
                result.Items = ItemRepository.Copy(toVendor.VendorID, fromVendorId)
            Else
                result.Items = New Item() {}
            End If

            result.Vendor = toVendor
            result.Success = True
        Else
            result.Success = False
        End If

        Return result
    End Function

    Private Function CreateClientListItems(clients As IEnumerable(Of Client)) As ListItem()
        Return clients.OrderBy(Function(x) x.DisplayName).Select(Function(x) New ListItem(x.DisplayName, x.ClientID.ToString())).ToArray()
    End Function

    Private Function CreateVendorListItems(vendors As IEnumerable(Of Vendor)) As ListItem()
        Return vendors.OrderBy(Function(x) x.VendorName).Select(Function(x) New ListItem(x.VendorName, x.VendorID.ToString())).ToArray()
    End Function
End Class

Public Class CopyDataResult
    Public Property Success As Boolean
    Public Property Vendor As Vendor
    Public Property Items As IEnumerable(Of Item)
End Class