Friend Class GameMapContext
    Implements IGameMapContext
    Private ReadOnly world As Persistence.IWorld
    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property Columns As Integer Implements IGameMapContext.Columns
        Get
            Return world.Avatar.Cell.Map.Columns
        End Get
    End Property
    Public ReadOnly Property Rows As Integer Implements IGameMapContext.Rows
        Get
            Return world.Avatar.Cell.Map.Rows
        End Get
    End Property
    Public Function GetOffsetX(viewWidth As Integer, cellWidth As Integer) As Integer Implements IGameMapContext.GetOffsetX
        Return viewWidth \ 2 - cellWidth \ 2 - world.Avatar.Cell.Column * cellWidth
    End Function
    Public Function GetOffsetY(viewHeight As Integer, cellHeight As Integer) As Integer Implements IGameMapContext.GetOffsetY
        Return viewHeight \ 2 - cellHeight \ 2 - world.Avatar.Cell.Row * cellHeight
    End Function
    Public Function TerrainGlyphAndColor(location As (Integer, Integer)) As (Char, Integer) Implements IGameMapContext.TerrainGlyphAndColor
        Dim descriptor = world.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).TerrainType.ToTerrainTypeDescriptor
        Return (descriptor.Glyph, descriptor.Hue)
    End Function
    Public Function HasCharacter(location As (Integer, Integer)) As Boolean Implements IGameMapContext.HasCharacter
        Return world.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).HasCharacter
    End Function
    Public Function CharacterGlyphAndColor(location As (Integer, Integer)) As (Char, Integer) Implements IGameMapContext.CharacterGlyphAndColor
        Dim descriptor = world.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).Character.CharacterType.ToCharacterTypeDescriptor
        Return (descriptor.Glyph, descriptor.Hue)
    End Function

    Public Function HasEnemy(location As (Integer, Integer)) As Boolean Implements IGameMapContext.HasEnemy
        Return If(world.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).Character?.IsEnemy, False)
    End Function

    Public Function IsAdjacent(location As (Integer, Integer)) As Boolean Implements IGameMapContext.IsAdjacent
        Return (Math.Abs(location.Item1 - world.Avatar.Cell.Column) + Math.Abs(location.Item2 - world.Avatar.Cell.Row)) = 1
    End Function
End Class
