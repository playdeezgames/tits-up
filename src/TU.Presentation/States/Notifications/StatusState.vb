Friend Class StatusState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B, Command.A
                SetState(GameState.ActionMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Hue.Black)
        Dim font = Context.Font(UIFont)
        Context.ShowHeader(displayBuffer, font, "Status", Hue.Black, Hue.Orange)
        Context.ShowStatusBar(displayBuffer, font, "Space/(A) - Continue", Hue.Black, Hue.LightGray)
        Dim avatar = Game.Avatar
        With font
            .WriteText(displayBuffer, (0, font.Height), $"Health: {avatar.Health}/{avatar.MaximumHealth}", Hue.Pink)
            .WriteText(displayBuffer, (0, font.Height * 2), $"Satiety: {avatar.Satiety}/{avatar.MaximumSatiety}", Hue.Purple)
            .WriteText(displayBuffer, (0, font.Height * 3), $"Attack: {avatar.MinimumAttack}-{avatar.MaximumAttack}", Hue.Red)
            .WriteText(displayBuffer, (0, font.Height * 4), $"Defend: {avatar.MinimumDefend}-{avatar.MaximumDefend}", Hue.Green)
        End With
    End Sub
End Class
