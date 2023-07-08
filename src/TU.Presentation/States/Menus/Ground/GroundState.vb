Friend Class GroundState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "Pick Up...", context.ControlsText("Select", "Cancel"), GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Game.ItemName = value.Item2
        SetState(GameState.Take)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Game.GroundItems.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key)).ToList
    End Function

    Public Overrides Sub OnStart()
        If Not Game.CanPickUp Then
            SetState(BoilerplateState.Neutral)
            Return
        End If
        MyBase.OnStart()
    End Sub
End Class
