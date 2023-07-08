Imports System.ComponentModel
Imports System.Data
Imports SPLORR.Game

Friend Class ForageState
    Inherits BaseGameState(Of IGameContext)
    Private Const GridColumns = 12
    Private Const GridRows = 12
    Private Grid(GridColumns, GridRows) As (Boolean, String)
    Private Column As Integer
    Private Row As Integer
    Private Tallies As New Dictionary(Of String, Integer)

    Public Sub New(parent As IGameController, setState As Action(Of String, Boolean), context As IUIContext(Of IGameContext))
        MyBase.New(parent, setState, context)
    End Sub
    Public Overrides Sub HandleCommand(cmd As String)
        Select Case cmd
            Case Command.B
                SetState(Neutral)
            Case Command.Up
                Row = (Row + GridRows - 1) Mod GridRows
            Case Command.Down
                Row = (Row + 1) Mod GridRows
            Case Command.Left
                Column = (Column + GridColumns - 1) Mod GridColumns
            Case Command.Right
                Column = (Column + 1) Mod GridColumns
            Case Command.A
                DoForage()
        End Select
    End Sub

    Private Sub DoForage()
        If Grid(Column, Row).Item1 Then
            Return
        End If

        Dim gridCell = Grid(Column, Row)

        If Game.DoForaging(gridCell.Item2) Then
            PlaySfx(Sfx.Yoink)
        End If

        If Not String.IsNullOrEmpty(gridCell.Item2) Then
            If Not Tallies.ContainsKey(gridCell.Item2) Then
                Tallies(gridCell.Item2) = 1
            Else
                Tallies(gridCell.Item2) += 1
            End If
        End If

        Grid(Column, Row) = (True, gridCell.Item2)

        If Game.IsDead Then
            SetState(GameState.Dead)
        End If
    End Sub

    Const OffsetX = ViewWidth \ 2 - GridColumns * CellWidth \ 2
    Const OffsetY = ViewHeight \ 2 - GridRows * CellHeight \ 2
    Private Function Plot(column As Integer, row As Integer) As (Integer, Integer)
        Return (OffsetX + column * CellWidth, OffsetY + row * CellHeight)
    End Function
    Public Overrides Sub Render(displayBuffer As IPixelSink)
        displayBuffer.Fill((0, 0), Context.ViewSize, Black)
        Dim font = Context.Font(StarveFont)
        For Each gridX In Enumerable.Range(0, GridColumns)
            For Each gridY In Enumerable.Range(0, GridRows)
                Dim gridCell = Grid(gridX, gridY)
                If Not gridCell.Item1 Then
                    font.WriteText(displayBuffer, Plot(gridX, gridY), ChrW(&HAE), DarkGray)
                Else
                    Dim itemTypeGlyphAndHue = Game.GetItemTypeGlyphAndHue(gridCell.Item2)
                    font.WriteText(displayBuffer, Plot(gridX, gridY), itemTypeGlyphAndHue.Item1, itemTypeGlyphAndHue.Item2)
                End If
            Next
        Next
        font.WriteText(displayBuffer, Plot(Column, Row), ChrW(&HAF), If(Grid(Column, Row).Item1, Red, LightGreen))

        font = Context.Font(UIFont)
        Dim avatar = Game.Avatar
        With font
            .WriteText(displayBuffer, (1, 0), $"H: {avatar.Health}/{avatar.MaximumHealth}", Hue.Black)
            .WriteText(displayBuffer, (0, 0), $"H: {avatar.Health}/{avatar.MaximumHealth}", Hue.Pink)
            .WriteText(displayBuffer, (1, font.Height), $"S: {avatar.Satiety}/{avatar.MaximumSatiety}", Hue.Black)
            .WriteText(displayBuffer, (0, font.Height), $"S: {avatar.Satiety}/{avatar.MaximumSatiety}", Hue.Purple)
        End With
        Dim y = 0
        For Each tally In Tallies
            Dim text = $"+{tally.Value} {Game.ItemTypeName(tally.Key)}"
            font.WriteText(displayBuffer, (ViewWidth - font.TextWidth(text), y), text, LightGray)
            y += font.Height
        Next
        Context.ShowStatusBar(displayBuffer, font, Context.ControlsText("Forage!", "Stop"), Hue.Black, Hue.LightGray)
    End Sub
    Public Overrides Sub OnStart()
        MyBase.OnStart()
        For Each gridX In Enumerable.Range(0, GridColumns)
            For Each gridY In Enumerable.Range(0, GridRows)
                Grid(gridX, gridY) = (False, String.Empty)
            Next
        Next
        Dim foragables = Game.Foragables
        Dim count As Integer
        For Each foragable In foragables
            count = foragable.Value
            While count > 0
                Dim gridX = RNG.FromRange(0, GridColumns - 1)
                Dim gridY = RNG.FromRange(0, GridRows - 1)
                If String.IsNullOrEmpty(Grid(gridX, gridY).Item2) Then
                    count -= 1
                    Grid(gridX, gridY) = (Grid(gridX, gridY).Item1, foragable.Key)
                End If
            End While
        Next
        count = Game.ForageAttempts
        While count > 0
            Dim gridX = RNG.FromRange(0, GridColumns - 1)
            Dim gridY = RNG.FromRange(0, GridRows - 1)
            Dim gridCell = Grid(gridX, gridY)
            If Not gridCell.Item1 AndAlso String.IsNullOrEmpty(gridCell.Item2) Then
                Grid(gridX, gridY) = (True, String.Empty)
                count -= 1
            End If
        End While
        Column = 0
        Row = 0
        Tallies.Clear()
    End Sub
End Class
