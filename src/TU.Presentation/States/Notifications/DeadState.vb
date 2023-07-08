Imports AOS.UI
Imports TU.Business

Friend Class DeadState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A Then
            Context.AbandonGame()
            SetState(BoilerplateState.MainMenu)
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Hue.Black)
        Dim font = Context.Font(UIFont)
        Dim centerX = ViewWidth \ 2
        Dim centerTextY = ViewHeight \ 2 - font.Height \ 2
        Dim avatar = Game.Avatar
        With font
            Dim text = "Yer Dead!"
            .WriteText(displayBuffer, (centerX - font.TextWidth(text) \ 2, centerTextY - font.Height), text, Hue.Red)
            text = $"Moves Made: {avatar.MovesMade}"
            .WriteText(displayBuffer, (centerX - font.TextWidth(text) \ 2, centerTextY + font.Height), text, Hue.Cyan)
        End With
        Context.ShowStatusBar(displayBuffer, font, "Space/(A) - Continue", Hue.Black, Hue.LightGray)
    End Sub
End Class
