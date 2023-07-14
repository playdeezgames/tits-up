Friend Class EquipmentState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "Equipment", context.ControlsText("Select", "Cancel"), GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Model.Avatar.Equipment.SelectedSlot = value.Item2
        SetState(GameState.EquipmentDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Model.Avatar.Equipment.EquippedSlots.ToList
    End Function

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Model.Avatar.Equipment.Exists Then
            SetState(GameState.ActionMenu)
        End If
    End Sub
End Class
