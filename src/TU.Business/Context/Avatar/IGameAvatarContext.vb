Public Interface IGameAvatarContext
    Sub Move(delta As (Integer, Integer))
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
    ReadOnly Property HasGroundItems As Boolean
    ReadOnly Property GroundItems As List(Of (String, String))
End Interface
