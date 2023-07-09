Friend Class MoveState
    Inherits BaseGameState(Of IGameContext)
    Private ReadOnly Property Delta As (Integer, Integer)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext), delta As (Integer, Integer))
        MyBase.New(parent, setState, context)
        Me.Delta = delta
    End Sub
    Public Overrides Sub HandleCommand(cmd As String)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Render(displayBuffer As IPixelSink)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Game.Move(Delta)
        SetState(Neutral)
    End Sub
End Class
