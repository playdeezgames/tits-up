Public Interface IAvatarInventoryModel
    ReadOnly Property Exists As Boolean
    ReadOnly Property Items As IEnumerable(Of (String, String))
    ReadOnly Property ItemsByName As IEnumerable(Of (String, Integer))
    ReadOnly Property ItemVerbs As IEnumerable(Of (String, String))
    Sub DropSelected()
End Interface
