Friend Class MessageState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A, Command.B
                Game.DismissMessage()
                OnStart()
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Hue.Black)
        Dim message = Game.CurrentMessage
        Dim font = Context.Font(UIFont)
        Dim offsetY = Context.ViewSize.Item2 \ 2 - font.Height * message.LineCount \ 2
        Dim centerX = Context.ViewSize.Item1 \ 2
        For Each line In message.Lines
            Dim offsetX = centerX - font.TextWidth(line.Text) \ 2
            font.WriteText(displayBuffer, (offsetX, offsetY), line.Text, line.Hue)
            offsetY += font.Height
        Next
    End Sub

    Public Overrides Sub OnStart()
        MyBase.OnStart()
        If Not Game.HasMessages Then
            PopState()
            Return
        End If
        Dim sfx = Game.CurrentMessage.Sfx
        If Not String.IsNullOrEmpty(sfx) Then
            PlaySfx(sfx)
        End If
    End Sub
End Class
