﻿Public Class CharacterData
    Public Property Recycled As Boolean
    Public Property CharacterType As String
    Public Property MapId As Integer
    Public Property CellIndex As Integer
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property ItemIds As New HashSet(Of Integer)
    Public Property EquipSlots As New Dictionary(Of String, Integer)
End Class
