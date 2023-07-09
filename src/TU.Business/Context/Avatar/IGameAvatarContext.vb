Public Interface IGameAvatarContext
    Sub Move(delta As (Integer, Integer))
    ReadOnly Property Statistic(statisticType As String) As Integer
End Interface
