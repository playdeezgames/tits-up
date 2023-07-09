﻿Friend Class NavigationState
    Inherits BaseGameState(Of IGameContext)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub

    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(GameMenu)
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
        Context.ShowStatusBar(displayBuffer, Context.Font(UIFont), Context.ControlsText("Action Menu", "Game Menu"), Black, LightGray)
    End Sub

    Private Sub DrawMap(displayBuffer As IPixelSink)
        Dim offsetX As Integer = Game.GetOffsetX(ViewWidth, CellWidth)
        Dim offsetY As Integer = Game.GetOffsetY(ViewHeight, CellHeight)
        Dim plotX = offsetX
        For Each column In Enumerable.Range(0, Game.MapColumnCount)
            If plotX >= ViewWidth Then
                Exit For
            ElseIf plotX <= -CellWidth Then
                Continue For
            End If
            Dim plotY = offsetY
            For Each row In Enumerable.Range(0, Game.MapRowCount)
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
        displayBuffer.Fill(plot, (CellWidth, CellHeight), Black)
        Dim font = Context.Font(TitsUpFont)
        Dim glyphAndColor = Game.GetTerrainGlyphAndColor(location)
        font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        If Game.HasCharacter(location) Then
            glyphAndColor = Game.GetCharacterGlyphAndColor(location)
            font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        End If
    End Sub
End Class
