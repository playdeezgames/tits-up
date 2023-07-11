Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemExtensions
    <Extension>
    Friend Function Name(item As IItem) As String
        Return item.ItemType.ToItemTypeDescriptor.Name
    End Function
End Module
