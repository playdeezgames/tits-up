Imports System.Runtime.CompilerServices
Imports TU.Persistence

Public Module ItemExtensions
    <Extension>
    Public Function Glyph(item As IItem) As Char
        Return item.ItemType.ToItemTypeDescriptor.Glyph
    End Function
    <Extension>
    Public Function Hue(item As IItem) As Integer
        Return item.ItemType.ToItemTypeDescriptor.Hue
    End Function
    <Extension>
    Public Function Name(item As IItem) As String
        Return item.ItemType.ToItemTypeDescriptor.Name
    End Function
    <Extension>
    Friend Function VerbTypes(item As IItem) As IEnumerable(Of String)
        Return item.ItemType.ToItemTypeDescriptor.AllVerbTypes
    End Function
    <Extension>
    Friend Sub DoVerb(item As IItem, verbType As String, character As ICharacter)
        item.ItemType.ToItemTypeDescriptor.VerbTypes(verbType).Invoke(character, item)
    End Sub
    <Extension>
    Friend Function CanEquip(item As IItem) As Boolean
        Return item.ItemType.ToItemTypeDescriptor.CanEquip
    End Function
    <Extension>
    Friend Function Satiety(item As IItem) As Integer
        Return item.Statistic(StatisticTypes.Satiety)
    End Function
    <Extension>
    Public Function Durability(item As IItem) As Integer
        Return item.Statistic(StatisticTypes.Durability)
    End Function
    <Extension>
    Public Function MaximumDurability(item As IItem) As Integer
        Return item.Statistic(StatisticTypes.MaximumDurability)
    End Function
    <Extension>
    Friend Function MinimumAttack(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.MinimumAttack)
    End Function
    <Extension>
    Friend Function MaximumAttack(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.MaximumAttack)
    End Function
    <Extension>
    Private Function TryGetStatistic(item As IItem, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(item.HasStatistic(statisticType), item.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Friend Function MinimumDefend(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.MinimumDefend)
    End Function
    <Extension>
    Friend Function MaximumDefend(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.MaximumDefend)
    End Function
    <Extension>
    Friend Function IsWeapon(item As IItem) As Boolean
        Return item.TryGetStatistic(StatisticTypes.MaximumAttack) > 0
    End Function
    <Extension>
    Friend Function IsArmor(item As IItem) As Boolean
        Return item.TryGetStatistic(StatisticTypes.MaximumDefend) > 0
    End Function
    <Extension>
    Friend Sub Deplete(item As IItem, depletion As Integer)
        item.SetDurability(item.Durability - depletion)
    End Sub
    <Extension>
    Friend Function IsDepleted(item As IItem) As Boolean
        Return item.Durability = 0
    End Function
    <Extension>
    Private Sub SetDurability(item As IItem, durability As Integer)
        item.Statistic(StatisticTypes.Durability) = Math.Clamp(durability, 0, item.MaximumDurability)
    End Sub
End Module
