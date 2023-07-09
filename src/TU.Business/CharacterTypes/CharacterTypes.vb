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
                        {StatisticTypes.MaximumHealth, 3}
                    })
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
                        {StatisticTypes.MaximumHealth, 1}
                    }, verbs:=New Dictionary(Of String, Action(Of ICharacter, ICharacter)) From
                    {
                        {VerbTypes.Disengage, AddressOf DefaultDisengage}
                    })
            }
        }

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
