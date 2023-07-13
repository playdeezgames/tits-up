Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemExtensions
    <Extension>
    Friend Function Name(item As IItem) As String
        Return item.ItemType.ToItemTypeDescriptor.Name
    End Function
    <Extension>
    Private Function TryGetStatistic(item As IItem, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(item.HasStatistic(statisticType), item.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Friend Function Encumbrance(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.Encumbrance, 0)
    End Function
    <Extension>
    Friend Sub DoVerb(item As IItem, verbType As String, character As ICharacter)
        item.ItemType.ToItemTypeDescriptor.VerbTypes(verbType).Invoke(character, item)
    End Sub
End Module
