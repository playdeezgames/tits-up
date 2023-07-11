Friend Class GroundDetailState
    Inherits BasePickerState(Of IWorldModel, Integer)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Ground)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        HeaderText = Game.Avatar.SelectedItemName
        Return Game.Avatar.GroundItemsByName.ToList
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        Select Case Game.Avatar.GroundItemsByName.Count
            Case 0
                SetState(Ground)
            Case 1
                Game.Avatar.SelectedItemId = Game.Avatar.GroundItemsByName.Single.Item2
                SetState(GameState.GroundItem)
        End Select
    End Sub
End Class
