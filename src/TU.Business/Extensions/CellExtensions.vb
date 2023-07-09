Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CellExtensions
    <Extension>
    Friend Function IsTenable(cell As ICell) As Boolean
        Return cell.TerrainType.ToTerrainTypeDescriptor.Tenable
    End Function
End Module
