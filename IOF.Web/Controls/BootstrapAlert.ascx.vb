Public Class BootstrapAlert
    Inherits IOFControl

    Public Property AlertType As BootstrapAlertType = BootstrapAlertType.Danger

    Public Property Message As String
        Get
            Return litAlertMessage.Text
        End Get
        Set(value As String)
            litAlertMessage.Text = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Update()
        End If
    End Sub

    Public Sub Show(msg As String, Optional type As BootstrapAlertType = BootstrapAlertType.Danger)
        Message = msg
        AlertType = type
        Update()
    End Sub

    Public Sub Hide()
        Visible = False
    End Sub

    Private Sub Update()
        Visible = Not String.IsNullOrEmpty(Message)
        Dim atype = [Enum].GetName(GetType(BootstrapAlertType), AlertType).ToLower()
        divAlert.Attributes("class") = $"bootstrap-alert alert alert-{atype}"
    End Sub
End Class

Public Enum BootstrapAlertType
    Success
    Info
    Warning
    Danger
End Enum