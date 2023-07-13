Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.CompilerServices
Imports SPLORR.Game
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
    Friend Function Name(character As ICharacter) As String
        Return character.CharacterType.ToCharacterTypeDescriptor.Name
    End Function
    <Extension>
    Private Sub DoOpportunityAttacks(character As ICharacter)
        If Not character.IsAvatar Then
            Return
        End If
        Dim index = 1
        For Each adjacentEnemy In character.AdjacentEnemies
            If character.IsTitsUp Then
                Exit For
            End If
            adjacentEnemy.Attack(character, (LightGray, $"Opportunity Attack {index} of {character.AdjacentEnemies.Count}"))
            index += 1
        Next
    End Sub
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
        character.DoOpportunityAttacks()
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
    <Extension>
    Friend Function IsEnemy(character As ICharacter) As Boolean
        Return character.CharacterType.ToCharacterTypeDescriptor.HasFlag(FlagTypes.Enemy)
    End Function
    Private Function RollDice(diceCount As Integer) As Integer
        Return Enumerable.Range(0, diceCount).Sum(Function(x) RNG.RollDice("1d6/6"))
    End Function
    <Extension>
    Private Function RollAttack(character As ICharacter) As Integer
        Return Math.Clamp(RollDice(character.AttackDice), 0, character.MaximumAttack)
    End Function
    <Extension>
    Private Function RollDefend(character As ICharacter) As Integer
        Return Math.Clamp(RollDice(character.DefendDice), 0, character.MaximumDefend)
    End Function
    <Extension>
    Private Sub DoDamage(character As ICharacter, damage As Integer)
        character.SetHealth(character.Health - damage)
    End Sub
    <Extension>
    Friend Function IsTitsUp(character As ICharacter) As Boolean
        Return character.Health = 0
    End Function
    <Extension>
    Friend Function Health(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.Health)
    End Function
    <Extension>
    Private Function DefendDice(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.DefendDice)
    End Function
    <Extension>
    Private Function AttackDice(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.AttackDice)
    End Function
    <Extension>
    Friend Function MaximumDefend(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumDefend)
    End Function
    <Extension>
    Friend Function MaximumAttack(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumAttack)
    End Function
    <Extension>
    Friend Sub SetHealth(character As ICharacter, health As Integer)
        character.Statistic(StatisticTypes.Health) = Math.Clamp(health, 0, character.MaximumHealth)
    End Sub
    <Extension>
    Friend Function MaximumHealth(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumHealth)
    End Function
    <Extension>
    Friend Function AdjacentEnemies(character As ICharacter) As IEnumerable(Of ICharacter)
        Dim neighbors = character.Cell.Neighbors
        Return neighbors.Select(Function(x) x.Character).Where(Function(x) x IsNot Nothing AndAlso x.IsEnemy)
    End Function
    <Extension>
    Friend Sub Attack(attacker As ICharacter, defender As ICharacter, Optional header As (Integer, String)? = Nothing)
        Dim message = attacker.World.CreateMessage()
        If header IsNot Nothing Then
            message.AddLine(header.Value.Item1, header.Value.Item2)
        End If
        message.AddLine(LightGray, $"{attacker.Name} attacks {defender.Name}!")
        Dim attackRoll = attacker.RollAttack()
        message.AddLine(LightGray, $"{attacker.Name} rolls an attack of {attackRoll}")
        Dim defendRoll = defender.RollDefend()
        message.AddLine(LightGray, $"{defender.Name} rolls a defend of {defendRoll}")
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        If damage > 0 Then
            message.AddLine(LightGray, $"{defender.Name} takes {damage} damage")
            defender.DoDamage(damage)
            If defender.IsTitsUp Then
                defender.Metadata(Metadatas.Epitaph) = $"Killed by {attacker.Name}!"
                message.AddLine(LightGray, $"{attacker.Name} makes {defender.Name} go tits up").SetSfx(If(defender.IsAvatar, Sfx.PlayerDeath, Sfx.EnemyDeath))
                attacker.RemoveStatistic(StatisticTypes.TargetCharacterId)
                defender.Recycle()
            Else
                message.
                    AddLine(LightGray, $"{defender.Name} has {defender.Health} health remaining").SetSfx(If(defender.IsAvatar, Sfx.PlayerHit, Sfx.EnemyHit))
            End If
        Else
            message.AddLine(LightGray, $"{attacker.Name} misses!").SetSfx(Sfx.Miss)
        End If
    End Sub
    <Extension>
    Friend Sub TakeItem(character As ICharacter, item As IItem)
        character.Cell.RemoveItem(item)
        character.AddItem(item)
    End Sub
    <Extension>
    Friend Function IsOverencumbered(character As ICharacter) As Boolean
        Return character.Encumbrance > character.MaximumEncumbrance
    End Function
    <Extension>
    Friend Function Encumbrance(character As ICharacter) As Integer
        Return character.Items.Sum(Function(x) x.Encumbrance)
    End Function
    <Extension>
    Friend Function MaximumEncumbrance(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumEncumbrance) + character.EquippedItems.Sum(Function(x) x.TryGetStatistic(StatisticTypes.MaximumEncumbrance))
    End Function
    <Extension>
    Friend Sub DropItem(character As ICharacter, item As IItem)
        character.Cell.AddItem(item)
        character.RemoveItem(item)
    End Sub
End Module
