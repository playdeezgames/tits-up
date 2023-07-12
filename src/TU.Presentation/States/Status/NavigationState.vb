Imports System.Xml.Schema

Friend Class NavigationState
    Inherits BaseGameState(Of IWorldModel)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IWorldModel))
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
        If Model.Avatar.Encumbrance.IsOver Then
            Context.ShowHeader(displayBuffer, font, "**ENCUMBERED**", Black, Red)
        End If
        Context.ShowStatusBar(displayBuffer, Font, Context.ControlsText("Action Menu", "Game Menu"), Black, LightGray)
    End Sub

    Private Sub DrawStats(displayBuffer As IPixelSink)
        Dim health = Model.Avatar.Health
        Dim maximumHealth = Model.Avatar.MaximumHealth
        With Context.Font(UIFont)
            .WriteText(displayBuffer, (0, 0), $"H: {health}/{maximumHealth}", Pink)
        End With
    End Sub

    Private Sub DrawMap(displayBuffer As IPixelSink)
        Dim offsetX As Integer = Model.Map.GetOffsetX(ViewWidth, CellWidth)
        Dim offsetY As Integer = Model.Map.GetOffsetY(ViewHeight, CellHeight)
        Dim plotX = offsetX
        For Each column In Enumerable.Range(0, Model.Map.Columns)
            If plotX >= ViewWidth Then
                Exit For
            ElseIf plotX <= -CellWidth Then
                Continue For
            End If
            Dim plotY = offsetY
            For Each row In Enumerable.Range(0, Model.Map.Rows)
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
        displayBuffer.Fill(plot, (CellWidth, CellHeight), If(Model.Map.IsAdjacent(location) AndAlso Model.Map.HasEnemy(location), Red, Black))
        Dim font = Context.Font(TitsUpFont)
        Dim glyphAndColor = Model.Map.TerrainGlyphAndColor(location)
        font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        If Model.Map.HasItem(location) Then
            glyphAndColor = Model.Map.ItemGlyphAndColor(location)
            font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        End If
        If Model.Map.HasCharacter(location) Then
            glyphAndColor = Model.Map.CharacterGlyphAndColor(location)
            font.WriteText(displayBuffer, plot, glyphAndColor.Item1, glyphAndColor.Item2)
        End If
    End Sub
End Class
