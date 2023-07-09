Imports TU.Persistence

Public Interface IGameContext
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    Function GetOffsetX(viewWidth As Integer, cellWidth As Integer) As Integer
    Function GetOffsetY(viewHeight As Integer, cellHeight As Integer) As Integer
    Function GetTerrainGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    Function GetCharacterGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    Function HasCharacter(location As (Integer, Integer)) As Boolean
    Sub Move(delta As (Integer, Integer))
    Function GetAvatarStatistic(statisticType As String) As Integer
    ReadOnly Property MapColumnCount As Integer
    ReadOnly Property MapRowCount As Integer
End Interface
