﻿Friend Class GroundItemState
    Inherits BasePickerState(Of IWorldModel, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Ground)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case TakeText
                SetState(GameState.Take)
        End Select
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = Model.Avatar.SelectedItemName
        Return New List(Of (String, String)) From
            {
                (TakeText, TakeText)
            }
    End Function
End Class
