Imports System.Reflection
Imports StructureMap

Public Class IOC
    Public Shared ReadOnly Property Container As Container

    Shared Sub New()
        Container = New Container(New ScanningRegistry())
    End Sub
End Class


Public Class ScanningRegistry
    Inherits Registry

    Public Sub New()
        Dim setting As String = ConfigurationManager.AppSettings("ScanningAssemblies")

        Dim assemblies As Assembly() = New Assembly() {}

        If Not String.IsNullOrEmpty(setting) Then
            assemblies = setting.Split(","c).Select(Function(x) Assembly.Load(x)).ToArray()
        End If

        Scan(Sub(x)
                 If assemblies.Length = 0 Then
                     x.AssembliesFromApplicationBaseDirectory()
                 Else
                     For Each a In assemblies
                         x.Assembly(a)
                     Next
                 End If

                 x.WithDefaultConventions()
             End Sub)
    End Sub
End Class
