Imports AOS.UI
Imports TU.Business
Imports TU.Persistence

Friend Class NavigationState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub
    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.A
                SetState(GameState.ActionMenu)
            Case Command.B
                SetState(BoilerplateState.GameMenu)
            Case Command.Up
                MoveAvatar(0, -1)
            Case Command.Down
                MoveAvatar(0, 1)
            Case Command.Left
                MoveAvatar(-1, 0)
            Case Command.Right
                MoveAvatar(1, 0)
        End Select
    End Sub

    Private Sub MoveAvatar(deltaX As Integer, deltaY As Integer)
        Game.Move(deltaX, deltaY)
        SetState(Neutral)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Hue.DarkGray)
        Dim avatar = Game.Avatar
        RenderCurrentMap(displayBuffer, avatar)
        Dim font = Context.Font(UIFont)
        With font
            .WriteText(displayBuffer, (1, 0), $"H: {avatar.Health}/{avatar.MaximumHealth}", Hue.Black)
            .WriteText(displayBuffer, (0, 0), $"H: {avatar.Health}/{avatar.MaximumHealth}", Hue.Pink)
            .WriteText(displayBuffer, (1, font.Height), $"S: {avatar.Satiety}/{avatar.MaximumSatiety}", Hue.Black)
            .WriteText(displayBuffer, (0, font.Height), $"S: {avatar.Satiety}/{avatar.MaximumSatiety}", Hue.Purple)
        End With
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Action Menu", "Game Menu"), Hue.Black, Hue.LightGray)
    End Sub

    Private Sub RenderCurrentMap(displayBuffer As IPixelSink, avatar As ICharacter)
        Dim map = avatar.Map
        Dim font = Context.Font(StarveFont)
        Dim offsetX = Context.ViewSize.Item1 \ 2 - CellWidth \ 2 - avatar.Cell.Column * CellWidth
        Dim offsetY = Context.ViewSize.Item2 \ 2 - CellHeight \ 2 - avatar.Cell.Row * CellHeight
        With font
            For column = 0 To map.Columns - 1
                Dim x = offsetX + column * CellWidth
                For row = 0 To map.Rows - 1
                    Dim y = offsetY + row * CellHeight
                    Dim cell = map.GetCell(column, row)
                    RenderTerrain(displayBuffer, font, (x, y), cell)
                    RenderItem(displayBuffer, font, (x, y), cell.TopItem)
                    RenderCharacter(displayBuffer, font, (x, y), cell.Character)
                Next
            Next
        End With
    End Sub

    Private Sub RenderItem(displayBuffer As IPixelSink, font As Font, position As (x As Integer, y As Integer), item As IItem)
        If item Is Nothing Then
            Return
        End If
        Dim glyph = item.Glyph()
        Dim hue = item.Hue()
        font.WriteText(displayBuffer, position, $"{glyph}", hue)
    End Sub

    Private Sub RenderCharacter(displayBuffer As IPixelSink, font As Font, position As (x As Integer, y As Integer), character As Persistence.ICharacter)
        If character Is Nothing Then
            Return
        End If
        Dim glyph = character.Glyph()
        Dim hue = character.Hue()
        font.WriteText(displayBuffer, position, $"{glyph}", hue)
    End Sub

    Private Sub RenderTerrain(displayBuffer As IPixelSink, font As Font, position As (x As Integer, y As Integer), cell As Persistence.ICell)
        Dim glyph = cell.GetGlyph()
        Dim hue = cell.GetHue()
        Dim background = Black
        If cell.HasCharacter Then
            If cell.Neighbors.Any(Function(x) x IsNot Nothing AndAlso If(x.Character?.IsAvatar, False)) Then
                background = Red
            End If
        End If
        displayBuffer.Fill(position, (CellWidth, CellHeight), background)
        font.WriteText(displayBuffer, position, $"{glyph}", hue)
    End Sub
End Class
