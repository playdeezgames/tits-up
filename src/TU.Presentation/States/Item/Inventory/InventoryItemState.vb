Friend Class InventoryItemState
    Inherits BasePickerState(Of IWorldModel, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Inventory)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case DropText
                SetState(GameState.Drop)
            Case EquipText
                SetState(GameState.Equip)
            Case Else
                Model.Avatar.Inventory.DoItemVerb(value.Item2)
                SetState(Neutral)
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        HeaderText = Model.Avatar.SelectedItemName
        Dim result = New List(Of (String, String)) From
            {
                (DropText, DropText)
            }
        If Model.Avatar.Inventory.CanEquip Then
            result.Add((EquipText, EquipText))
        End If
        result.AddRange(Model.Avatar.Inventory.ItemVerbs)
        Return result
    End Function
End Class
