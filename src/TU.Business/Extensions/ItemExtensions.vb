Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemExtensions
    <Extension>
    Friend Function Name(item As IItem) As String
        Return item.ItemType.ToItemTypeDescriptor.Name
    End Function
    <Extension>
    Friend Function Encumbrance(item As IItem) As Integer
        Return item.Statistic(StatisticTypes.Encumbrance)
    End Function
End Module
