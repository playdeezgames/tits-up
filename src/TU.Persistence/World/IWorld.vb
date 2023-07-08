Public Interface IWorld
    Sub Save(filename As String)
    Function CreateMap(mapType As String, size As (Integer, Integer), terrainType As String) As IMap
    Function CreateCharacter(characterType As String, cell As ICell) As ICharacter
    Sub DismissMessage()
    ReadOnly Property CurrentMessage As IMessage
    Property Avatar As ICharacter
    ReadOnly Property Characters As IEnumerable(Of ICharacter)
    ReadOnly Property HasMessages As Boolean
    Function CreateMessage() As IMessage
    Function CreateItem(itemType As String) As IItem
End Interface
