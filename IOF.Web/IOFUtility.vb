Imports IOF.Models

Public Class IOFUtility
    Public Shared ReadOnly Property Settings As Settings = New Settings()

    Public Shared Function HasDuplicate(items As IEnumerable(Of Item), partNum As String, description As String) As Integer
        Return HasDuplicate(items, partNum, description, -1)
    End Function

    Public Shared Function HasDuplicate(items As IEnumerable(Of Item), partNum As String, description As String, itemId As Integer) As Integer
        Dim cleanPartNum As String = CleanString(partNum)
        Dim cleanDescription As String = CleanString(description)

        ' Check if PartNum is empty
        If Not String.IsNullOrEmpty(cleanPartNum) Then
            ' Are there duplicates in database by PartNum?
            Dim item As Item = items.FirstOrDefault(Function(x) x.Active AndAlso CleanString(x.PartNum) = cleanPartNum AndAlso x.ItemID <> itemId)
            If item IsNot Nothing Then
                Return item.ItemID
            End If
        Else
            ' Are there duplicates in database by Description?
            Dim item As Item = items.FirstOrDefault(Function(x) x.Active AndAlso CleanString(x.Description) = cleanDescription AndAlso x.ItemID <> itemId)
            If item IsNot Nothing Then
                Return item.ItemID
            End If
        End If

        Return -1
    End Function

    Public Shared Function HasDuplicate(items As IEnumerable(Of Detail), partNum As String, description As String, catId As Integer, podid As Integer) As Integer
        Dim cleanPartNum As String = CleanString(partNum)
        Dim cleanDescription As String = CleanString(description)

        ' Check if PartNum is empty
        If Not String.IsNullOrEmpty(cleanPartNum) Then
            ' Are there duplicates in database by PartNum and Category?
            Dim detail As Detail = items.FirstOrDefault(Function(x) CleanString(x.PartNum) = cleanPartNum AndAlso x.CategoryID <> catId AndAlso x.PODID <> podid)
            If detail IsNot Nothing Then
                Return detail.PODID
            End If
        Else
            ' Are there duplicates in database by Description and Category?
            Dim detail As Detail = items.FirstOrDefault(Function(x) CleanString(x.Description) = cleanDescription AndAlso x.CategoryID <> catId AndAlso x.PODID <> podid)
            If detail IsNot Nothing Then
                Return detail.PODID
            End If
        End If

        Return -1
    End Function

    ''' <summary>
    ''' Convert to lower case and strips spaces.
    ''' </summary>
    Public Shared Function CleanString(ByVal s As String) As String
        Return s.ToLower().Replace(" ", String.Empty).Trim()
    End Function
End Class

Public Class Settings
    Public ReadOnly Property IsProduction As Boolean
        Get
            Return Boolean.Parse(ConfigurationManager.AppSettings("IsProduction"))
        End Get
    End Property

    Public ReadOnly Property UsePurchaseList As Boolean
        Get
            Return Boolean.Parse(ConfigurationManager.AppSettings("UsePurchaseList"))
        End Get
    End Property

    Public ReadOnly Property NoAccountClients As Integer()
        Get
            Dim splitter As String() = ConfigurationManager.AppSettings("NoAccountClients").Split(","c)
            Return splitter.Select(Function(x) Convert.ToInt32(x)).ToArray()
        End Get
    End Property
End Class