Imports TU.Persistence

Public Interface IGameContext
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
    ReadOnly Property Map As IGameMapContext

    Function GetMapOffsetX(viewWidth As Integer, cellWidth As Integer) As Integer
    Function GetMapOffsetY(viewHeight As Integer, cellHeight As Integer) As Integer
    Function GetTerrainGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    Function HasCharacter(location As (Integer, Integer)) As Boolean
    Function GetCharacterGlyphAndColor(location As (Integer, Integer)) As (Char, Integer)
    ReadOnly Property MapColumnCount As Integer
    ReadOnly Property MapRowCount As Integer


    Sub Move(delta As (Integer, Integer))
    Function GetAvatarStatistic(statisticType As String) As Integer

    Function TargetCharacterVerbs() As IEnumerable(Of (String, String))
    Sub DoTargetCharacterVerb(verbType As String)
    ReadOnly Property HasTargetCharacter As Boolean
End Interface
