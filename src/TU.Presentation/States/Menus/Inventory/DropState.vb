Friend Class DropState
    Inherits BasePickerState(Of IGameContext, Integer)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.InventoryDetail)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        Game.DropItems(value.Item2)
        SetState(GameState.InventoryDetail)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        HeaderText = $"Drop How Many {Game.ItemName}?"
        Return Enumerable.Range(1, Game.ItemCountByName(Game.ItemName)).Select(Function(x) ($"{x}", x)).ToList
    End Function

    Public Overrides Sub OnStart()
        Dim itemCount = Game.ItemCountByName(Game.ItemName)
        If itemCount <= 1 Then
            Game.DropItems(itemCount)
            SetState(GameState.Inventory)
            Return
        End If
        MyBase.OnStart()
    End Sub
End Class
