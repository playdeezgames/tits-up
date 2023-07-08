Friend Class InventoryState
    Inherits BasePickerState(Of IGameContext, String)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "Inventory", context.ControlsText("Select", "Cancel"), GameState.ActionMenu)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, String))
        Game.ItemName = value.Item2
        SetState(GameState.InventoryDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, String))
        Return Game.Inventory.ToList
    End Function
    Public Overrides Sub OnStart()
        If Not Game.HasItems Then
            SetState(GameState.ActionMenu)
            Return
        End If
        MyBase.OnStart()
    End Sub

End Class
