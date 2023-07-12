Friend Class GroundState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "On The Ground", context.ControlsText("Select", "Cancel"), Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.Avatar.SelectedItemName = value.Item2
        SetState(GameState.GroundDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Model.Avatar.GroundItems.ToList
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Avatar.HasGroundItems Then
            SetState(Neutral)
        End If
    End Sub
End Class
