﻿Imports TU.Persistence

Friend Module ItemInitializer
    Friend Function CreateItem(world As IWorld, itemType As String) As IItem
        Dim item = world.CreateItem(itemType)
        For Each entry In itemType.ToItemTypeDescriptor.Statistics
            item.Statistic(entry.Key) = entry.Value
        Next
        Return item
    End Function
End Module
