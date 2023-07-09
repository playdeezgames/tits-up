Imports TU.Persistence

Public Interface IGameContext
    Sub Embark()
    Sub Abandon()
    Sub Load(filename As String)
    Sub Save(filename As String)
End Interface
