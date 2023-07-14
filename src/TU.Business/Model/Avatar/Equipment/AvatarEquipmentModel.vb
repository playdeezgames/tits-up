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
End Class
