Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module TerrainTypes
    Friend Const Empty = "Empty"
    Friend Const Wall = "Wall"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor) =
        New Dictionary(Of String, TerrainTypeDescriptor) From
        {
            {Empty, New TerrainTypeDescriptor("Empty", ChrW(0), Black)},
            {Wall, New TerrainTypeDescriptor("Wall", ChrW(3), LightGray, tenable:=False)}
        }

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
