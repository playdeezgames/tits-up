Public Interface IGameAvatarContext
    Sub Move(delta As (Integer, Integer))
    ReadOnly Property Statistic(statisticType As String) As Integer
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
End Interface
