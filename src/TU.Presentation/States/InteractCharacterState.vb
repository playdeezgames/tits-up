Friend Class InteractCharacterState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), BoilerplateState.Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Game.DoTargetCharacterVerb(value.Item2)
        SetState(Neutral)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Game.TargetCharacterVerbs.ToList
    End Function
End Class
