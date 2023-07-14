Friend Class GroundDetailState
    Inherits BasePickerState(Of IWorldModel, Integer)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Ground)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Model.Avatar.SelectedItemId = value.Item2
        SetState(GameState.GroundItem)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        HeaderText = Model.Avatar.SelectedItemName
        Return Model.Avatar.Ground.GroundItemsByName.ToList
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Select Case Model.Avatar.Ground.GroundItemsByName.Count
            Case 0
                SetState(Ground)
            Case 1
                Model.Avatar.SelectedItemId = Model.Avatar.Ground.GroundItemsByName.Single.Item2
                SetState(GameState.GroundItem)
        End Select
    End Sub
End Class
