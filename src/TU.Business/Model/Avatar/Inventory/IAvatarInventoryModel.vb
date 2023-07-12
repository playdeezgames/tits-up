Public Interface IAvatarInventoryModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property Items As IEnumerable(Of (String, String))
End Interface
