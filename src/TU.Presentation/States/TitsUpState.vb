Friend Class TitsUpState
    Inherits BaseGameState(Of IGameContext)
    Private showUntil As DateTimeOffset
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A AndAlso DateTimeOffset.Now > showUntil Then
            Game.Abandon()
            SetState(MainMenu)
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(UIFont)
        Dim text = "Yer tits up!!"
        With font
            .WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - .TextWidth(text) \ 2, Context.ViewSize.Item2 \ 2 - font.Height \ 2), text, Red)
        End With
        With Context.Font(TitsUpFont)
            .WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - CellWidth \ 2, Context.ViewSize.Item2 \ 4 - CellHeight \ 2), ChrW(2), Brown)
        End With
        If DateTimeOffset.Now > showUntil Then
            Context.ShowStatusBar(displayBuffer, font, "Space/(A) - Main Menu", Black, LightGray)
        End If
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        showUntil = DateTimeOffset.Now.AddSeconds(TitsUpDelaySeconds)
    End Sub
End Class
