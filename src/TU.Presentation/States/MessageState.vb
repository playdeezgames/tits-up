Friend Class MessageState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A Then
            Game.Messages.Dismiss()
            SetState(Neutral)
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim message As IMessage = Game.Messages.Current
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Item2 \ 2 - font.Height * message.LineCount \ 2
        For Each line In message.Lines
            Dim text = line.Text
            font.WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - font.TextWidth(text) \ 2, y), text, line.Hue)
            y += font.Height
        Next
        Context.ShowStatusBar(displayBuffer, font, "Space/(A) - Dismiss", Black, LightGray)
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        PlaySfx(Game.Messages.Current.Sfx)
    End Sub
End Class
