Friend Class NeutralState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Game.Messages.HasAny Then
            SetState(GameState.Message)
            Return
        End If
        If Game.IsTitsUp Then
            SetState(GameState.TitsUp)
            Return
        End If
        If Game.TargetCharacter.Exists Then
            SetState(GameState.InteractCharacter)
            Return
        End If
        SetState(Navigation)
    End Sub
End Class
