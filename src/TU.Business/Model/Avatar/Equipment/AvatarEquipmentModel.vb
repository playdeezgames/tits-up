Imports TU.Persistence

Friend Class AvatarEquipmentModel
    Implements IAvatarEquipmentModel
    Private ReadOnly avatar As ICharacter
    Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Exists As Boolean Implements IAvatarEquipmentModel.Exists
        Get
            Return avatar.HasEquipment
        End Get
    End Property

    Public ReadOnly Property EquippedSlots As IEnumerable(Of (String, String)) Implements IAvatarEquipmentModel.EquippedSlots
        Get
            Return avatar.Equipment.Select(Function(x) ($"{x.Key.ToEquipSlotTypeDescriptor.Name}: {x.Value.Name}", x.Key))
        End Get
    End Property

    Public Property SelectedSlot As String Implements IAvatarEquipmentModel.SelectedSlot
        Get
            Return avatar.Metadata(Metadatas.SelectedEquipSlot)
        End Get
        Set(value As String)
            avatar.Metadata(Metadatas.SelectedEquipSlot) = value
        End Set
    End Property

    Public ReadOnly Property SelectedEquippedItemName As String Implements IAvatarEquipmentModel.SelectedEquippedItemName
        Get
            Return avatar.Equipment(SelectedSlot).Name
        End Get
    End Property

    Public Sub UnequipSelected() Implements IAvatarEquipmentModel.UnequipSelected
        avatar.Unequip(SelectedSlot)
        SelectedSlot = Nothing
    End Sub
End Class
