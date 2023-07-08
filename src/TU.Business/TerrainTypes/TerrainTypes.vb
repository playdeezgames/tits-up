Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module TerrainTypes
    Friend Const Grass = "Grass"
    Friend Const Tree = "Tree"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {
                Grass,
                New TerrainTypeDescriptor(
                    "Grass",
                    "\"c,
                    Hue.Green,
                    tenable:=True,
                    canForage:=True,
                    cellInitializer:=AddressOf InitializeGrass)
            },
            {
                Tree,
                New TerrainTypeDescriptor(
                    "Tree",
                    "k"c,
                    Hue.Green,
                    tenable:=False,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, ICell)) From
                    {
                        {VerbTypes.TakeStick, AddressOf DoTakeStick}
                    })
            }
        }

    Private Sub InitializeGrass(cell As ICell)
        cell.Statistic(StatisticTypes.ForageAttempts) = 0
        cell.Statistic(StatisticTypes.MossWeight) = 4
        cell.Statistic(StatisticTypes.FiberWeight) = 16
    End Sub

    Private Sub DoTakeStick(character As ICharacter, cell As ICell)
        Dim item = ItemInitializer.CreateItem(character.World, ItemTypes.Stick)
        character.AddItem(item)
        character.World.CreateMessage().
            AddLine(LightGray, "You take a sturdy stick,").
            AddLine(LightGray, "suitable for snake clubbing!").
            AddLine(LightGray, "(you have to equip it first)")
    End Sub

    <Extension>
    Friend Function ToTerrainTypeDescriptor(terrainType As String) As TerrainTypeDescriptor
        Return descriptors(terrainType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
