Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports TU.Persistence
Public Module CharacterNavigationExtensions
    <Extension>
    Public Function Move(character As ICharacter, deltaX As Integer, deltaY As Integer) As ICell
        Dim currentCell = character.Cell
        Dim nextCell = currentCell.Map.GetCell(currentCell.Column + deltaX, currentCell.Row + deltaY)
        If nextCell Is Nothing OrElse Not nextCell.IsTenable Then
            Return nextCell
        End If
        If nextCell.HasCharacter Then
            Return nextCell
        End If
        character.DoCounterAttacks("Opportunity Attack")
        character.ApplyHunger()
        character.SetMovesMade(character.MovesMade + 1)
        character.Cell = nextCell
        currentCell.Character = Nothing
        nextCell.Character = character
        Return Nothing
    End Function
    <Extension>
    Public Function MovesMade(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MovesMade)
    End Function
    <Extension>
    Private Sub SetMovesMade(character As ICharacter, movesMade As Integer)
        character.Statistic(StatisticTypes.MovesMade) = movesMade
    End Sub
End Module
