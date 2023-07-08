Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CharacterTypes
    Friend Const Dude = "Dude"
    Friend Const Snek = "Snek"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Dude,
                New CharacterTypeDescriptor(
                    "dude",
                    "$"c,
                    Hue.Brown,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 100},
                        {StatisticTypes.MaximumHealth, 100},
                        {StatisticTypes.Satiety, 100},
                        {StatisticTypes.MaximumSatiety, 100},
                        {StatisticTypes.HungerRate, 1},
                        {StatisticTypes.MovesMade, 0},
                        {StatisticTypes.MinimumAttack, 10},
                        {StatisticTypes.MaximumAttack, 20},
                        {StatisticTypes.MinimumDefend, 0},
                        {StatisticTypes.MaximumDefend, 10}
                    })
            },
            {
                Snek,
                New CharacterTypeDescriptor(
                    "snek",
                    "2"c,
                    Hue.LightGreen,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 10},
                        {StatisticTypes.MaximumHealth, 10},
                        {StatisticTypes.Satiety, 0},
                        {StatisticTypes.MaximumSatiety, 0},
                        {StatisticTypes.HungerRate, 0},
                        {StatisticTypes.MovesMade, 0},
                        {StatisticTypes.MinimumAttack, 10},
                        {StatisticTypes.MaximumAttack, 25},
                        {StatisticTypes.MinimumDefend, 5},
                        {StatisticTypes.MaximumDefend, 15}
                    },
                    initializer:=AddressOf InitializeSnek)
            }
        }

    Private Sub InitializeSnek(character As ICharacter)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.SnekCorpse))
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
