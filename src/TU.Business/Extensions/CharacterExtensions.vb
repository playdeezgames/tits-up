Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CharacterExtensions
    <Extension>
    Friend Sub Move(character As ICharacter, delta As (Integer, Integer))
        Dim nextColumn = character.Cell.Column + delta.Item1
        Dim nextRow = character.Cell.Row + delta.Item2
        Dim nextCell = character.Cell.Map.GetCell(nextColumn, nextRow)
        If nextCell Is Nothing Then
            Return
        End If
        If nextCell.IsTenable Then
            nextCell.Character = character
            character.Cell.Character = Nothing
            character.Cell = nextCell
        End If
    End Sub
End Module
