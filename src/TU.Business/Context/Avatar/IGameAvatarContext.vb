Public Interface IGameAvatarContext
    Sub Move(delta As (Integer, Integer))
    Sub TakeSelectedItem()
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
    ReadOnly Property HasGroundItems As Boolean
    ReadOnly Property GroundItems As IEnumerable(Of (String, String))
    Property SelectedItemName As String
    ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer))
    Property SelectedItemId As Integer?
    ReadOnly Property Encumbrance As Integer
    ReadOnly Property MaximumEncumbrance As Integer
    ReadOnly Property IsOverencumbered As Boolean
End Interface
