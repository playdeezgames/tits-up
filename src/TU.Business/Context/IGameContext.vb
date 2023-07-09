Public Interface IGameContext
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IGameMapContext
    ReadOnly Property Avatar As IGameAvatarContext
    ReadOnly Property TargetCharacter As IGameTargetCharacterContext

    Sub Move(delta As (Integer, Integer))
    Function GetAvatarStatistic(statisticType As String) As Integer

    Function TargetCharacterVerbs() As IEnumerable(Of (String, String))
    Sub DoTargetCharacterVerb(verbType As String)
    ReadOnly Property HasTargetCharacter As Boolean
End Interface
