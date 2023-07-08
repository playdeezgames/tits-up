Friend Class EquipmentState
    Inherits BasePickerState(Of IGameContext, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "Equipment", context.ControlsText("Select", "Cancel"), GameState.ActionMenu)
    End Sub
    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Game.EquipSlotType = value.Item2
        SetState(GameState.EquipmentDetail)
    End Sub
    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Game.EquippedSlots.Select(Function(x) ($"{Game.EquipSlotName(x)}: {Game.EquippedItem(x).Name}({Game.EquippedItem(x).Durability}/{Game.EquippedItem(x).MaximumDurability})", x)).ToList
    End Function
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Game.HasEquipment Then
            SetState(GameState.ActionMenu)
        End If
    End Sub
End Class
