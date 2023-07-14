Friend Class WonState
    Inherits BaseGameState(Of IWorldModel)
    Private showUntil As DateTimeOffset
    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        If cmd = Command.A AndAlso DateTimeOffset.Now > showUntil Then
            Model.Abandon()
            SetState(MainMenu)
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(UIFont)
        Dim text = "You ""won""!"
        With font
            .WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - .TextWidth(text) \ 2, Context.ViewSize.Item2 \ 2 - font.Height \ 2), text, Green)
            text = $"Yer dignity: {Model.Avatar.Dignity}"
            .WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - .TextWidth(text) \ 2, Context.ViewSize.Item2 \ 2 + font.Height * 3 \ 2), text, Blue)
        End With
        With Context.Font(TitsUpFont)
            .WriteText(displayBuffer, (Context.ViewSize.Item1 \ 2 - CellWidth \ 2, Context.ViewSize.Item2 \ 4 - CellHeight \ 2), ChrW(1), Brown)
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
