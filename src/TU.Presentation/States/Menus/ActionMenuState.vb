Friend Class ActionMenuState
    Inherits BasePickerState(Of IGameContext, String)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(
            parent,
            setState,
            context,
            "Actions Menu",
            context.ControlsText("Select", "Cancel"),
            BoilerplateState.Neutral)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Select Case value.Item2
            Case InventoryText
                SetState(GameState.Inventory)
            Case StatusText
                SetState(GameState.Status)
            Case PickUpText
                SetState(GameState.Ground)
            Case EquipmentText
                SetState(GameState.Equipment)
            Case CraftText
                SetState(GameState.Craft)
            Case ForageText
                SetState(GameState.Forage)
            Case Else
                Game.DoVerb(value.Item2)
                SetState(Neutral)
        End Select
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Dim result As New List(Of (String, String))
        If Game.CanPickUp Then
            result.Add((PickUpText, PickUpText))
        End If
        result.AddRange(Game.AvailableVerbs)
        If Game.HasItems Then
            result.Add((InventoryText, InventoryText))
        End If
        If Game.HasEquipment Then
            result.Add((EquipmentText, EquipmentText))
        End If
        If Game.CanCraft Then
            result.Add((CraftText, CraftText))
        End If
        If Game.CanForage Then
            result.Add((ForageText, ForageText))
        End If
        result.Add((StatusText, StatusText))
        Return result
    End Function
End Class
