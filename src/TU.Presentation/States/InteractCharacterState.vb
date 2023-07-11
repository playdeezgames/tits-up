Friend Class InteractCharacterState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), BoilerplateState.Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.TargetCharacter.DoVerb(value.Item2)
        SetState(Neutral)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = $"{Model.TargetCharacter.Name}({Model.TargetCharacter.Health}/{Model.TargetCharacter.MaximumHealth})"
        Return Model.TargetCharacter.Verbs.ToList
    End Function
End Class
