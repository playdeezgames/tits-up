Friend Class GroundItemState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Ground)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case TakeText
                SetState(GameState.Take)
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = Game.Avatar.SelectedItemName
        Return New List(Of (String, String)) From
            {
                (TakeText, TakeText)
            }
    End Function
End Class
