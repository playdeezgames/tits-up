Public Interface IGameAvatarContext
    Sub Move(delta As (Integer, Integer))

    ReadOnly Property GroundItems As IEnumerable(Of (String, String))
    ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer))
    Sub TakeSelectedItem()
    Property SelectedItemId As Integer?
    Property SelectedItemName As String
    ReadOnly Property HasGroundItems As Boolean

    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer

    ReadOnly Property Encumbrance As IGameAvatarEncumbranceContext
End Interface
