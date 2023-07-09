Imports TU.Persistence

Public Class GameContext
    Implements IGameContext

    Public ReadOnly Property MapColumnCount As Integer Implements IGameContext.MapColumnCount
        Get
            Return World.Avatar.Cell.Map.Columns
        End Get
    End Property

    Public ReadOnly Property MapRowCount As Integer Implements IGameContext.MapRowCount
        Get
            Return World.Avatar.Cell.Map.Rows
        End Get
    End Property

    Private Property World As IWorld
    Public Sub Embark() Implements IGameContext.Embark
        World = New World(New Data.WorldData)
        WorldInitializer.Initialize(World)
    End Sub
    Public Sub Abandon() Implements IGameContext.Abandon
        World = Nothing
    End Sub
    Public Sub Load(filename As String) Implements IGameContext.Load
        World = Persistence.World.Load(filename)
    End Sub
    Public Sub Save(filename As String) Implements IGameContext.Save
        World.Save(filename)
    End Sub

    Public Sub Move(delta As (Integer, Integer)) Implements IGameContext.Move
        World.Avatar.Move(delta)
    End Sub

    Public Function GetOffsetX(viewWidth As Integer, cellWidth As Integer) As Integer Implements IGameContext.GetOffsetX
        Return viewWidth \ 2 - cellWidth \ 2 - World.Avatar.Cell.Column * cellWidth
    End Function

    Public Function GetOffsetY(viewHeight As Integer, cellHeight As Integer) As Integer Implements IGameContext.GetOffsetY
        Return viewHeight \ 2 - cellHeight \ 2 - World.Avatar.Cell.Row * cellHeight
    End Function

    Public Function GetTerrainGlyphAndColor(location As (Integer, Integer)) As (Char, Integer) Implements IGameContext.GetTerrainGlyphAndColor
        Dim descriptor = World.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).TerrainType.ToTerrainTypeDescriptor
        Return (descriptor.Glyph, descriptor.Hue)
    End Function

    Public Function GetCharacterGlyphAndColor(location As (Integer, Integer)) As (Char, Integer) Implements IGameContext.GetCharacterGlyphAndColor
        Dim descriptor = World.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).Character.CharacterType.ToCharacterTypeDescriptor
        Return (descriptor.Glyph, descriptor.Hue)
    End Function

    Public Function HasCharacter(location As (Integer, Integer)) As Boolean Implements IGameContext.HasCharacter
        Return World.Avatar.Cell.Map.GetCell(location.Item1, location.Item2).HasCharacter
    End Function

    Public Function GetAvatarStatistic(statisticType As String) As Integer Implements IGameContext.GetAvatarStatistic
        Return World.Avatar.Statistic(statisticType)
    End Function
End Class
