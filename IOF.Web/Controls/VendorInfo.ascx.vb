Imports IOF.Models

Namespace Controls
    Public Class VendorInfo
        Inherits IOFControl

        Public Event AddClick As EventHandler
        Public Event UpdateClick As EventHandler
        Public Event CancelClick As EventHandler

        Public Property VendorID As Integer = 0
        Public Property CancelButtonVisible As Boolean = True
        Public Property CancelButtonText As String = "Cancel"

        Protected Overrides Sub OnLoad(e As EventArgs)
            btnVendorCancel.Visible = CancelButtonVisible
            btnVendorCancel.Text = CancelButtonText
            MyBase.OnLoad(e)
        End Sub

        Public Overrides Sub DataBind()
            MyBase.DataBind()

            Alert1.Hide()

            If VendorID = 0 Then
                btnAddVendor.Visible = True
                btnUpdateVendor.Visible = False

                hidVendorID.Value = String.Empty
                txtVendorName.Text = String.Empty
                txtAddress1.Text = String.Empty
                txtAddress2.Text = String.Empty
                txtAddress3.Text = String.Empty
                txtContact.Text = String.Empty
                txtPhone.Text = String.Empty
                txtFax.Text = String.Empty
                txtURL.Text = String.Empty
                txtEmail.Text = String.Empty
            Else
                btnAddVendor.Visible = False
                btnUpdateVendor.Visible = True

                Dim vend As Vendor = VendorRepository.Single(VendorID)

                If vend IsNot Nothing Then
                    hidVendorID.Value = vend.VendorID.ToString()
                    txtVendorName.Text = vend.VendorName
                    txtAddress1.Text = vend.Address1
                    txtAddress2.Text = vend.Address2
                    txtAddress3.Text = vend.Address3
                    txtContact.Text = vend.Contact
                    txtPhone.Text = vend.Phone
                    txtFax.Text = vend.Fax
                    txtURL.Text = vend.URL
                    txtEmail.Text = vend.Email
                End If
            End If
        End Sub

        Protected Sub btnVendorCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent CancelClick(sender, e)
        End Sub

        Protected Sub btnUpdateVendor_Click(ByVal sender As Object, ByVal e As EventArgs)
            Try
                VendorID = Convert.ToInt32(hidVendorID.Value)

                VendorRepository.Update(VendorID, txtVendorName.Text, txtAddress1.Text, txtAddress2.Text, txtAddress3.Text, txtContact.Text, txtPhone.Text, txtFax.Text, txtURL.Text, txtEmail.Text)

                DataBind()

                RaiseEvent UpdateClick(sender, e)
            Catch ex As Exception
                Alert1.Show(ex.Message)
            End Try
        End Sub

        Protected Sub btnAddVendor_Click(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If String.IsNullOrEmpty(txtVendorName.Text) Then
                    Alert1.Show("Vendor Name is required.")
                    Return
                End If

                If String.IsNullOrEmpty(txtAddress1.Text) Then
                    Alert1.Show("Address (line 1) is required.")
                    Return
                End If

                If String.IsNullOrEmpty(txtPhone.Text) Then
                    Alert1.Show("Phone is required.")
                    Return
                End If

                Dim vend = VendorRepository.Add(Page.ActiveClientID, txtVendorName.Text, txtAddress1.Text, txtAddress2.Text, txtAddress3.Text, txtContact.Text, txtPhone.Text, txtFax.Text, txtURL.Text, txtEmail.Text)
                VendorID = vend.VendorID
                DataBind()
                RaiseEvent AddClick(sender, e)
            Catch ex As Exception
                Alert1.Show(ex.Message)
            End Try
        End Sub
    End Class
End Namespace