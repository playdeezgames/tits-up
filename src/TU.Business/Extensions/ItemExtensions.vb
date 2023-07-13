﻿Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemExtensions
    <Extension>
    Friend Function Descriptor(item As IItem) As ItemTypeDescriptor
        Return item.ItemType.ToItemTypeDescriptor
    End Function
    <Extension>
    Friend Function Name(item As IItem) As String
        Return item.Descriptor.Name
    End Function
    <Extension>
    Friend Function TryGetStatistic(item As IItem, statisticType As String, Optional defaultValue As Integer = 0) As Integer
        Return If(item.HasStatistic(statisticType), item.Statistic(statisticType), defaultValue)
    End Function
    <Extension>
    Friend Function Encumbrance(item As IItem) As Integer
        Return item.TryGetStatistic(StatisticTypes.Encumbrance, 0)
    End Function
    <Extension>
    Friend Sub DoVerb(item As IItem, verbType As String, character As ICharacter)
        item.Descriptor.VerbTypes(verbType).Invoke(character, item)
    End Sub
End Module
