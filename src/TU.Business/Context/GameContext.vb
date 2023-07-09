Imports TU.Persistence

Public Class GameContext
    Implements IGameContext
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
End Class
