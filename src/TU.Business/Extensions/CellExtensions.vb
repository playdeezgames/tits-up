Imports System.Runtime.CompilerServices
Imports TU.Persistence

Public Module CellExtensions
    <Extension>
    Public Function GetGlyph(cell As ICell) As Char
        Return cell.TerrainType.ToTerrainTypeDescriptor.Glyph
    End Function
    <Extension>
    Public Function GetHue(cell As ICell) As Integer
        Return cell.TerrainType.ToTerrainTypeDescriptor.Hue
    End Function
    <Extension>
    Friend Function IsTenable(cell As ICell) As Boolean
        Return cell.TerrainType.ToTerrainTypeDescriptor.Tenable
    End Function
    Private ReadOnly neighborDeltas As IReadOnlyList(Of (Integer, Integer)) =
        New List(Of (Integer, Integer)) From
        {
            (0, -1),
            (1, 0),
            (0, 1),
            (-1, 0)
        }
    <Extension>
    Public Function Neighbors(cell As ICell) As IEnumerable(Of ICell)
        Return neighborDeltas.Select(Function(delta) cell.Map.GetCell(cell.Column + delta.Item1, cell.Row + delta.Item2)).Where(Function(x) x IsNot Nothing)
    End Function
    <Extension>
    Friend Function DoVerb(cell As ICell, verbType As String, character As ICharacter) As Boolean
        cell.TerrainType.ToTerrainTypeDescriptor.VerbTypes(verbType).Invoke(character, cell)
        Return cell.Map.World.HasMessages
    End Function
    <Extension>
    Friend Function CanForage(cell As ICell) As Boolean
        Return ItemTypes.All.Any(Function(itemType) cell.HasStatistic(ForagingWeight(itemType)) AndAlso cell.Statistic(ForagingWeight(itemType)) > 0) AndAlso Not cell.HasEnemies
    End Function
    <Extension>
    Friend Function HasEnemies(cell As ICell) As Boolean
        Return cell.Neighbors.Any(Function(x) x.HasCharacter)
    End Function
End Module
