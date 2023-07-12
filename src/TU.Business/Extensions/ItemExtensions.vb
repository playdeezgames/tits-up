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
    <Extension>
    Friend Sub DoVerb(item As IItem, verbType As String, character As ICharacter)
        item.ItemType.ToItemTypeDescriptor.VerbTypes(verbType).Invoke(character, item)
    End Sub
End Module
