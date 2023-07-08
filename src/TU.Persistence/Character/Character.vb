﻿Friend Class Character
    Inherits CharacterDataClient
    Implements ICharacter

    Public Sub New(worldData As Data.WorldData, characterId As Integer)
        MyBase.New(worldData, characterId)
    End Sub

    Public ReadOnly Property Id As Integer Implements ICharacter.Id
        Get
            Return CharacterId
        End Get
    End Property

    Public ReadOnly Property CharacterType As String Implements ICharacter.CharacterType
        Get
            Return CharacterData.CharacterType
        End Get
    End Property

    Public Property Cell As ICell Implements ICharacter.Cell
        Get
            Return New Cell(WorldData, CharacterData.MapId, CharacterData.CellIndex)
        End Get
        Set(value As ICell)
            CharacterData.MapId = value.Map.Id
            CharacterData.CellIndex = value.Id
        End Set
    End Property

    Public ReadOnly Property Map As IMap Implements ICharacter.Map
        Get
            Return Cell.Map
        End Get
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements ICharacter.Statistic
        Get
            Return CharacterData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            CharacterData.Statistics(statisticType) = value
        End Set
    End Property
    Public ReadOnly Property World As IWorld Implements ICharacter.World
        Get
            Return New World(WorldData)
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItem) Implements ICharacter.Items
        Get
            Return CharacterData.ItemIds.Select(Function(x) New Item(WorldData, x))
        End Get
    End Property

    Public ReadOnly Property HasItems As Boolean Implements ICharacter.HasItems
        Get
            Return CharacterData.ItemIds.Any
        End Get
    End Property

    ReadOnly Property IsAvatar As Boolean Implements ICharacter.IsAvatar
        Get
            Return If(WorldData.AvatarCharacterId, -1) = Id
        End Get
    End Property

    Public ReadOnly Property HasEquipment As Boolean Implements ICharacter.HasEquipment
        Get
            Return CharacterData.EquipSlots.Any
        End Get
    End Property

    Public ReadOnly Property Equipment As IReadOnlyDictionary(Of String, IItem) Implements ICharacter.Equipment
        Get
            Return CharacterData.EquipSlots.ToDictionary(Of String, IItem)(Function(x) x.Key, Function(x) New Item(WorldData, x.Value))
        End Get
    End Property

    Public ReadOnly Property EquippedItems As IReadOnlyList(Of IItem) Implements ICharacter.EquippedItems
        Get
            Return CharacterData.EquipSlots.Values.Distinct.Select(Of IItem)(Function(x) New Item(WorldData, x)).ToList
        End Get
    End Property

    Public Sub Recycle() Implements ICharacter.Recycle
        If Not IsAvatar Then
            Cell.Character = Nothing
            CharacterData.Recycled = True
        End If
    End Sub

    Public Sub RemoveItem(item As IItem) Implements ICharacter.RemoveItem
        CharacterData.ItemIds.Remove(item.Id)
    End Sub

    Public Sub AddItem(item As IItem) Implements ICharacter.AddItem
        CharacterData.ItemIds.Add(item.Id)
    End Sub

    Public Sub Equip(equipSlotType As String, item As IItem) Implements ICharacter.Equip
        Unequip(equipSlotType)
        RemoveItem(item)
        CharacterData.EquipSlots(equipSlotType) = item.Id
    End Sub
    Public Sub Unequip(equipSlotType As String) Implements ICharacter.Unequip
        If CharacterData.EquipSlots.ContainsKey(equipSlotType) Then
            AddItem(New Item(WorldData, CharacterData.EquipSlots(equipSlotType)))
            CharacterData.EquipSlots.Remove(equipSlotType)
        End If
    End Sub

    Public Sub UnequipItem(item As IItem) Implements ICharacter.UnequipItem
        For Each equipSlot In CharacterData.EquipSlots.Where(Function(x) x.Value = item.Id).Select(Function(x) x.Key)
            Unequip(equipSlot)
        Next
    End Sub
End Class
