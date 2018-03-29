Public Class SearchCriteria
    Public Shared Sub Save(startDate As DateTime?, endDate As DateTime?, vendorId As Integer, vendorName As String, keywords As String, partNumber As String, poid As Integer, shortCode As String, otherClientId As Integer, includeSelf As Boolean)
        HttpContext.Current.Session("SearchCriteria.StartDate") = If(startDate.HasValue, startDate.Value.ToString("yyyy-MM-dd"), String.Empty)
        HttpContext.Current.Session("SearchCriteria.EndDate") = If(endDate.HasValue, endDate.Value.ToString("yyyy-MM-dd"), String.Empty)
        HttpContext.Current.Session("SearchCriteria.VendorID") = vendorId
        HttpContext.Current.Session("SearchCriteria.VendorName") = vendorName
        HttpContext.Current.Session("SearchCriteria.Keywords") = keywords
        HttpContext.Current.Session("SearchCriteria.PartNumber") = partNumber
        HttpContext.Current.Session("SearchCriteria.POID") = poid
        HttpContext.Current.Session("SearchCriteria.ShortCode") = shortCode
        HttpContext.Current.Session("SearchCriteria.OtherClientID") = otherClientId
        HttpContext.Current.Session("SearchCriteria.IncludeSelf") = includeSelf
    End Sub

    Public Shared Sub Clear()
        HttpContext.Current.Session.Remove("SearchCriteria.StartDate")
        HttpContext.Current.Session.Remove("SearchCriteria.EndDate")
        HttpContext.Current.Session.Remove("SearchCriteria.VendorID")
        HttpContext.Current.Session.Remove("SearchCriteria.VendorName")
        HttpContext.Current.Session.Remove("SearchCriteria.Keywords")
        HttpContext.Current.Session.Remove("SearchCriteria.PartNumber")
        HttpContext.Current.Session.Remove("SearchCriteria.POID")
        HttpContext.Current.Session.Remove("SearchCriteria.ShortCode")
        HttpContext.Current.Session.Remove("SearchCriteria.OtherClientID")
        HttpContext.Current.Session.Remove("SearchCriteria.IncludeSelf")
    End Sub

    Public Shared ReadOnly Property StartDate As DateTime?
        Get
            If HttpContext.Current.Session("SearchCriteria.StartDate") IsNot Nothing Then
                Dim value As String = HttpContext.Current.Session("SearchCriteria.StartDate").ToString()
                If Not String.IsNullOrEmpty(value) Then
                    Return DateTime.ParseExact(value, "yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture)
                End If
            End If

            ' session variable is either not set or an empty string
            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property EndDate As DateTime?
        Get
            If HttpContext.Current.Session("SearchCriteria.EndDate") IsNot Nothing Then
                Dim value As String = HttpContext.Current.Session("SearchCriteria.EndDate").ToString()
                If Not String.IsNullOrEmpty(value) Then
                    Return DateTime.ParseExact(value, "yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture)
                End If
            End If

            ' session variable is either not set or an empty string
            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property VendorID As Integer
        Get
            If HttpContext.Current.Session("SearchCriteria.VendorID") Is Nothing Then
                Return 0
            Else
                Return Convert.ToInt32(HttpContext.Current.Session("SearchCriteria.VendorID"))
            End If
        End Get
    End Property

    Public Shared ReadOnly Property VendorName As String
        Get
            If HttpContext.Current.Session("SearchCriteria.VendorName") Is Nothing Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session("SearchCriteria.VendorName").ToString()
            End If
        End Get
    End Property

    Public Shared ReadOnly Property Keywords As String
        Get
            If HttpContext.Current.Session("SearchCriteria.Keywords") Is Nothing Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session("SearchCriteria.Keywords").ToString()
            End If
        End Get
    End Property

    Public Shared ReadOnly Property PartNumber As String
        Get
            If HttpContext.Current.Session("SearchCriteria.PartNumber") Is Nothing Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session("SearchCriteria.PartNumber").ToString()
            End If
        End Get
    End Property

    Public Shared ReadOnly Property POID As Integer
        Get
            If HttpContext.Current.Session("SearchCriteria.POID") Is Nothing Then
                Return 0
            Else
                Return Convert.ToInt32(HttpContext.Current.Session("SearchCriteria.POID"))
            End If
        End Get
    End Property

    Public Shared ReadOnly Property ShortCode As String
        Get
            If HttpContext.Current.Session("SearchCriteria.ShortCode") Is Nothing Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session("SearchCriteria.ShortCode").ToString()
            End If
        End Get
    End Property

    Public Shared ReadOnly Property OtherClientID As Integer
        Get
            If HttpContext.Current.Session("SearchCriteria.OtherClientID") Is Nothing Then
                Return 0
            Else
                Return Convert.ToInt32(HttpContext.Current.Session("SearchCriteria.OtherClientID"))
            End If
        End Get
    End Property

    Public Shared ReadOnly Property IncludeSelf As Boolean
        Get
            If HttpContext.Current.Session("SearchCriteria.IncludeSelf") Is Nothing Then
                Return True
            Else
                Return Convert.ToBoolean(HttpContext.Current.Session("SearchCriteria.IncludeSelf"))
            End If
        End Get
    End Property

    Public Shared Sub UseDefaultStartDate()
        ' only set if session variable is null (it might be an empty string for any)
        If HttpContext.Current.Session("SearchCriteria.StartDate") Is Nothing Then
            HttpContext.Current.Session("SearchCriteria.StartDate") = DateTime.Now.Date.AddMonths(-3).ToString("yyyy-MM-dd")
        End If
    End Sub

    Public Shared Sub UseDefaultEndDate()
        ' only set if session variable is null (it might be an empty string for any)
        If HttpContext.Current.Session("SearchCriteria.EndDate") Is Nothing Then
            HttpContext.Current.Session("SearchCriteria.EndDate") = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd")
        End If
    End Sub

    Public Shared Sub UseOtherClientID(value As Integer)
        ' only set if session variable is null (it might be -1 for any)
        If HttpContext.Current.Session("SearchCriteria.OtherClientID") Is Nothing Then
            HttpContext.Current.Session("SearchCriteria.OtherClientID") = value
        End If
    End Sub
End Class
