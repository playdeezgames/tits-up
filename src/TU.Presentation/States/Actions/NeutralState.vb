Imports AOS.UI
Imports TU.Business

Friend Class NeutralState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Throw New NotImplementedException
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        Throw New NotImplementedException
    End Sub
    Public Overrides Sub OnStart()
        If Game.HasMessages Then
            SetStates(Message, BoilerplateState.Neutral)
            Return
        End If
        If Not Game.IsDead Then
            If Game.IsInCombat Then
                SetState(GameState.Combat)
                Return
            ElseIf Game.IsInteracting Then
                SetState(GameState.Interact)
                Return
            End If
        End If
        If Game.IsDead Then
            Game.ClearTargetCell()
            SetState(GameState.Dead)
            Return
        End If
        SetState(GameState.Navigation)
        MyBase.OnStart()
    End Sub
End Class
