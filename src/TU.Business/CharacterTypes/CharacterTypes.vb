Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CharacterTypes
    Friend Const Tizzy = "Tizzy"
    Friend Const Goblin = "Goblin"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Tizzy,
                New CharacterTypeDescriptor(
                    "Tizzy",
                    ChrW(1),
                    Hue.Brown,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 3},
                        {StatisticTypes.MaximumHealth, 3},
                        {StatisticTypes.AttackDice, 1},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 4},
                        {StatisticTypes.MaximumDefend, 2}
                    },
                    flags:=New List(Of String) From {FlagTypes.Avatar})
            },
            {
                Goblin,
                New CharacterTypeDescriptor(
                    "Goblin",
                    ChrW(5),
                    Hue.Green,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1}
                    },
                    verbs:=New Dictionary(Of String, Action(Of ICharacter, ICharacter)) From
                    {
                        {VerbTypes.Fight, AddressOf DefaultAttack},
                        {VerbTypes.Disengage, AddressOf DefaultDisengage}
                    },
                    flags:=New List(Of String) From {FlagTypes.Enemy},
                    initializer:=AddressOf InitializeGoblin)
            }
        }

    Private Sub InitializeGoblin(character As ICharacter)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.GoblinCorpse))
    End Sub

    Private Sub DefaultAttack(source As ICharacter, target As ICharacter)
        source.Attack(target)
        Dim enemies = source.AdjacentEnemies
        Dim index = 1
        For Each adjacentEnemy In enemies
            If source.IsTitsUp Then
                Exit For
            End If
            adjacentEnemy.Attack(source, (LightGray, $"Counter Attack {index} of {enemies.Count}"))
            index += 1
        Next
    End Sub

    Private Sub DefaultDisengage(source As ICharacter, target As ICharacter)
        source.SetTargetCharacter(Nothing)
    End Sub

    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
