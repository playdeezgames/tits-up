Public Interface IAvatarEquipmentModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property EquippedSlots As IEnumerable(Of (String, String))
End Interface
