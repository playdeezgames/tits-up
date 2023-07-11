Imports System.Xml.Schema

Friend Class NavigationState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(GameMenu)
            Case Command.A
                SetState(ActionMenu)
            Case Command.Up
                SetState(GameState.MoveNorth)
            Case Command.Down
                SetState(GameState.MoveSouth)
            Case Command.Left
                SetState(GameState.MoveWest)
            Case Command.Right
                SetState(GameState.MoveEast)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, DarkGray)
        DrawMap(displayBuffer)
        DrawStats(displayBuffer)
        Dim font = Context.Font(UIFont)
        If Game.Avatar.Encumbrance.IsOver Then
            Context.ShowHeader(displayBuffer, font, "**ENCUMBERED**", Black, Red)
        End If
        Context.ShowStatusBar(displayBuffer, Font, Context.ControlsText("Action Menu", "Game Menu"), Black, LightGray)
    End Sub

    Private Sub DrawStats(displayBuffer As IPixelSink)
        Dim health = Game.Avatar.Health
        Dim maximumHealth = Game.Avatar.MaximumHealth
        With Context.Font(UIFont)
            .WriteText(displayBuffer, (0, 0), $"H: {health}/{maximumHealth}", Pink)
        End With
    End Sub

    Private Sub DrawMap(displayBuffer As IPixelSink)
        Dim offsetX As Integer = Game.Map.GetOffsetX(ViewWidth, CellWidth)
        Dim offsetY As Integer = Game.Map.GetOffsetY(ViewHeight, CellHeight)
        Dim plotX = offsetX
        For Each column In Enumerable.Range(0, Game.Map.Columns)
            If plotX >= ViewWidth Then
                Exit For
            ElseIf plotX <= -CellWidth Then
                Continue For
            End If
            Dim plotY = offsetY
            For Each row In Enumerable.Range(0, Game.Map.Rows)
                If plotY >= ViewHeight Then
                    Exit For
                ElseIf plotY <= -CellHeight Then
                    Continue For
                End If
                DrawCell(displayBuffer, (plotX, plotY), (column, row))
                plotY += CellHeight
            Next
            plotX += CellWidth
        Next
    End Sub

    Private Sub DrawCell(displayBuffer As IPixelSink, plot As (Integer, Integer), location As (Integer, Integer))
        displayBuffer.Fill(plot, (CellWidth, CellHeight), If(Game.Map.IsAdjacent(location) AndAlso Game.Map.HasEnemy(location), Red, Black))
        Dim font = Context.Font(TitsUpFont)
        Dim glyphAndColor = Game.Map.TerrainGlyphAndColor(location)
        font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        If Game.Map.HasItem(location) Then
            glyphAndColor = Game.Map.ItemGlyphAndColor(location)
            font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        End If
        If Game.Map.HasCharacter(location) Then
            glyphAndColor = Game.Map.CharacterGlyphAndColor(location)
            font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        End If
    End Sub
End Class
