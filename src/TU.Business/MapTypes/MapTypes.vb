Imports System.Data.Common
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
                    (9, 9),
                    TerrainTypes.Empty,
                    spawnCharacters:=New Dictionary(Of String, Integer) From
                                      {
                                        {CharacterTypes.Tizzy, 1},
                                        {CharacterTypes.Goblin, 6}
                                      },
                    customInitializer:=AddressOf InitializeInitialMap)
            }
        }

    Private Sub InitializeInitialMap(map As IMap)
        For Each column In Enumerable.Range(0, map.Columns)
            map.GetCell(column, 0).TerrainType = TerrainTypes.Wall
            map.GetCell(column, map.Rows - 1).TerrainType = TerrainTypes.Wall
        Next
        For Each row In Enumerable.Range(1, map.Rows - 2)
            map.GetCell(0, row).TerrainType = TerrainTypes.Wall
            map.GetCell(map.Columns - 1, row).TerrainType = TerrainTypes.Wall
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
