Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module MapTypes
    Friend Const Initial = "Initial"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MapTypeDescriptor) =
        New Dictionary(Of String, MapTypeDescriptor) From
        {
            {
                Initial,
                New MapTypeDescriptor(
                    (48, 48),
                    spawnCharacters:=New Dictionary(Of String, Integer) From
                    {
                        {CharacterTypes.Dude, 1},
                        {CharacterTypes.Snek, 500}
                    },
                    spawnItems:=New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Rock, 50}
                    },
                    customInitializer:=AddressOf InitializeInitialMap)
            }
        }

    Private Sub InitializeInitialMap(map As IMap)
        For column = 0 To map.Columns - 1
            map.GetCell(column, 0).TerrainType = TerrainTypes.Tree
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Tree
        Next
        For row = 1 To map.Rows - 2
            map.GetCell(0, row).TerrainType = TerrainTypes.Tree
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Tree
        Next
    End Sub

    <Extension>
    Friend Function ToMapTypeDescriptor(mapType As String) As MapTypeDescriptor
        Return descriptors(mapType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
