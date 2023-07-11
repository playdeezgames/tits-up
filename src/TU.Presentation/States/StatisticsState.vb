﻿Friend Class StatisticsState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A, Command.B
                SetState(Neutral)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(UIFont)
        Dim y = Context.ViewSize.Item2 \ 2 - font.Height * 3 \ 2
        y = RenderLine(displayBuffer, font, y, $"Health: {Game.Avatar.Health}/{Game.Avatar.MaximumHealth}", Pink)
        y = RenderLine(displayBuffer, font, y, $"Attack: {Game.Avatar.MaximumAttack}", Red)
        y = RenderLine(displayBuffer, font, y, $"Defend: {Game.Avatar.MaximumDefend}", Green)
        Context.ShowHeader(displayBuffer, font, "Statistics", Orange, Black)
        Context.ShowStatusBar(displayBuffer, font, "Space/(A)/Esc/(B) - Go Back", Black, LightGray)
    End Sub

    Private Function RenderLine(displayBuffer As IPixelSink, font As Font, y As Integer, text As String, hue As Integer) As Integer
        font.WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - font.TextWidth(text) \ 2, y), text, hue)
        Return y + font.Height
    End Function
End Class
