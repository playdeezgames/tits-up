Imports SPLORR.Game
Imports TU.Persistence
Imports System.Runtime.CompilerServices

Public Module CharacterCombatExtensions
    <Extension>
    Public Sub Run(character As ICharacter)
        'TODO: counterattack!
    End Sub
    <Extension>
    Public Function MinimumAttack(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MinimumAttack) + character.Equipment.Values.Sum(Function(x) x.MinimumAttack)
    End Function
    <Extension>
    Public Function MaximumAttack(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumAttack) + character.Equipment.Values.Sum(Function(x) x.MaximumAttack)
    End Function
    <Extension>
    Public Function MinimumDefend(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MinimumDefend) + character.Equipment.Values.Sum(Function(x) x.MinimumDefend)
    End Function
    <Extension>
    Public Function MaximumDefend(character As ICharacter) As Integer
        Return character.Statistic(StatisticTypes.MaximumDefend) + character.Equipment.Values.Sum(Function(x) x.MaximumDefend)
    End Function
    <Extension>
    Private Function RollAttack(character As ICharacter) As Integer
        Return RNG.FromRange(character.MinimumAttack, character.MaximumAttack)
    End Function
    <Extension>
    Private Function RollDefend(character As ICharacter) As Integer
        Return RNG.FromRange(character.MinimumDefend, character.MaximumDefend)
    End Function
    <Extension>
    Private Function Weapons(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsWeapon)
    End Function
    <Extension>
    Private Function Armors(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsArmor)
    End Function
    <Extension>
    Private Function DepleteWeapons(character As ICharacter, durability As Integer) As IEnumerable(Of IItem)
        Dim weapons = character.Weapons
        While weapons.Any AndAlso durability > 0
            Dim weapon = RNG.FromEnumerable(weapons)
            weapon.Deplete(1)
            durability -= 1
        End While
        Return weapons.Where(Function(x) x.IsDepleted)
    End Function
    <Extension>
    Private Function DepleteArmors(character As ICharacter, durability As Integer) As IEnumerable(Of IItem)
        Dim armors = character.Armors
        While armors.Any AndAlso durability > 0
            Dim armor = RNG.FromEnumerable(armors)
            armor.Deplete(1)
            durability -= 1
        End While
        Return armors.Where(Function(x) x.IsDepleted)
    End Function
    <Extension>
    Public Sub Attack(attacker As ICharacter, defender As ICharacter, Optional doCounterAttacks As Boolean = False, Optional counterIndex As Integer = 0, Optional counterMaximum As Integer = 0, Optional title As String = "")
        Dim message = attacker.World.CreateMessage()
        If counterIndex > 0 Then
            message.AddLine(LightGray, $"{title} {counterIndex}/{counterMaximum}")
        End If
        Dim attackRoll = attacker.RollAttack()
        message.AddLine(Business.Hue.LightGray, $"{attacker.Name} rolls an attack of {attackRoll}")
        For Each brokenWeapon In attacker.DepleteWeapons(attackRoll)
            message.AddLine(Business.Hue.Red, $"{attacker.Name} breaks their {brokenWeapon.Name}!")
            attacker.UnequipItem(brokenWeapon)
            attacker.RemoveItem(brokenWeapon)
            brokenWeapon.Recycle()
        Next
        Dim defendRoll = defender.RollDefend()
        message.AddLine(Business.Hue.LightGray, $"{defender.Name} rolls a defend of {defendRoll}")
        For Each brokenArmor In defender.DepleteArmors(Math.Min(attackRoll, defendRoll))
            message.AddLine(Business.Hue.Red, $"{attacker.Name} breaks their {brokenArmor.Name}!")
            attacker.UnequipItem(brokenArmor)
            attacker.RemoveItem(brokenArmor)
            brokenArmor.Recycle()
        Next
        Dim damage = Math.Max(0, attackRoll - defendRoll)
        If damage > 0 Then
            defender.SetHealth(defender.Health - damage)
            message.AddLine(Business.Hue.LightGray, $"{defender.Name} takes {damage} damage.")
            If defender.IsDead Then
                message.Sfx = If(doCounterAttacks, Sfx.EnemyDeath, Sfx.PlayerDeath)
                message.AddLine(Business.Hue.LightGray, $"{attacker.Name} kills {defender.Name}")
                defender.DropItems()
                defender.Recycle()
            Else
                message.Sfx = If(doCounterAttacks, Sfx.EnemyHit, Sfx.PlayerHit)
                message.AddLine(Business.Hue.LightGray, $"{defender.Name} has {defender.Health} health remaining")
            End If
        Else
            message.Sfx = Sfx.Miss
            message.AddLine(Business.Hue.LightGray, $"{attacker.Name} misses.")
        End If
        If doCounterAttacks Then
            attacker.DoCounterAttacks("Counter Attack")
        End If
    End Sub
    <Extension>
    Friend Sub DoCounterAttacks(character As ICharacter, title As String)
        Dim neighbors = character.Cell.Neighbors.Where(Function(x) If(x?.HasCharacter, False))
        Dim total = neighbors.Count
        Dim index = 1
        For Each neighbor In neighbors
            neighbor.Character.Attack(character, False, index, total, title)
            index += 1
            If character.IsDead Then
                Exit For
            End If
        Next
    End Sub

End Module
