Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CharacterExtensions
    <Extension>
    Friend Sub SetTargetCharacter(character As ICharacter, target As ICharacter)
        If target Is Nothing Then
            character.RemoveStatistic(StatisticTypes.TargetCharacterId)
            Return
        End If
        character.Statistic(StatisticTypes.TargetCharacterId) = target.Id
    End Sub
    <Extension>
    Friend Function TargetCharacter(character As ICharacter) As ICharacter
        If character.HasStatistic(StatisticTypes.TargetCharacterId) Then
            Return character.World.Character(character.Statistic(StatisticTypes.TargetCharacterId))
        End If
        Return Nothing
    End Function
    <Extension>
    Friend Sub Move(character As ICharacter, delta As (Integer, Integer))
        Dim nextColumn = character.Cell.Column + delta.Item1
        Dim nextRow = character.Cell.Row + delta.Item2
        Dim nextCell = character.Cell.Map.GetCell(nextColumn, nextRow)
        If nextCell Is Nothing Then
            Return
        End If
        If Not nextCell.IsTenable Then
            Return
        End If
        character.SetTargetCharacter(nextCell.Character)
        If nextCell.HasCharacter Then
            Return
        End If
        'TODO: opportunity attacks
        nextCell.Character = character
        character.Cell.Character = Nothing
        character.Cell = nextCell
    End Sub
    <Extension>
    Friend Function Verbs(character As ICharacter) As IEnumerable(Of String)
        Return character.CharacterType.ToCharacterTypeDescriptor.AvailableVerbs
    End Function
    <Extension>
    Friend Sub DoVerb(target As ICharacter, verbType As String, source As ICharacter)
        target.CharacterType.ToCharacterTypeDescriptor.Verbs(verbType).Invoke(source, target)
    End Sub
End Module
