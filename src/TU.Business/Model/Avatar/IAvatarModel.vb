Public Interface IAvatarModel
    Sub Move(delta As (Integer, Integer))
    ReadOnly Property IsTitsUp As Boolean

    'TODO: refactor into ground items
    ReadOnly Property GroundItems As IEnumerable(Of (String, String))
    ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer))
    ReadOnly Property HasGroundItems As Boolean

    'TODO: refactor into selected item
    Sub TakeSelectedItem()
    Property SelectedItemId As Integer?
    Property SelectedItemName As String

    'TODO: refactor into combat
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property AverageAttack As Double
    ReadOnly Property AverageDefend As Double
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer

    Property Epitaph As String

    ReadOnly Property Inventory As IAvatarInventoryModel
    ReadOnly Property Encumbrance As IAvatarEncumbranceModel
End Interface
