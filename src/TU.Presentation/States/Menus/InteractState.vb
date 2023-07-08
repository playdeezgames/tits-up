Friend Class InteractState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), BoilerplateState.Neutral)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.B Then
            Game.ClearTargetCell()
        End If
        MyBase.HandleCommand(cmd)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        If Game.DoTargetCellVerb(value.Item2) Then
            SetStates(GameState.Message, BoilerplateState.Neutral)
        Else
            SetState(BoilerplateState.Neutral)
        End If
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = $"Interact with {Game.TargetTerrainName}"
        Return Game.TargetCellVerbs.ToList
    End Function
End Class
