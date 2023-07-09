Public Interface IGameContext
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IGameMapContext
    ReadOnly Property Avatar As IGameAvatarContext
    ReadOnly Property TargetCharacter As IGameTargetCharacterContext
End Interface
