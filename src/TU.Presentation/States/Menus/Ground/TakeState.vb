Friend Class TakeState
    Inherits BasePickerState(Of IGameContext, Integer)
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context, "<placeholder>", context.ControlsText("Select", "Cancel"), GameState.Ground)
    End Sub

    Protected Overrides Sub OnActivateMenuItem(value As (String, Integer))
        PlaySfx(Sfx.Take)
        Game.TakeItems(value.Item2)
        SetState(GameState.Ground)
    End Sub

    Protected Overrides Function InitializeMenuItems() As List(Of (String, Integer))
        HeaderText = $"Take How Many {Game.ItemName}?"
        Return Enumerable.Range(1, Game.GroundItemCountByName(Game.ItemName)).Select(Function(x) ($"{x}", x)).ToList
    End Function

    Public Overrides Sub OnStart()
        Dim itemCount = Game.GroundItemCountByName(Game.ItemName)
        If itemCount <= 1 Then
            If itemCount > 0 Then
                PlaySfx(Sfx.Take)
                Game.TakeItems(itemCount)
            End If
            SetState(GameState.Ground)
            Return
        End If
        MyBase.OnStart()
    End Sub
End Class
