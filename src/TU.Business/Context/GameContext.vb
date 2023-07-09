Imports TU.Persistence

Public Class GameContext
    Implements IGameContext
    Public ReadOnly Property Map As IGameMapContext Implements IGameContext.Map
        Get
            Return New GameMapContext(World)
        End Get
    End Property
    Public ReadOnly Property Avatar As IGameAvatarContext Implements IGameContext.Avatar
        Get
            Return New GameAvatarContext(World)
        End Get
    End Property
    Public ReadOnly Property TargetCharacter As IGameTargetCharacterContext Implements IGameContext.TargetCharacter
        Get
            Return New GameTargetCharacterContext(World)
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
    Public Function GetAvatarStatistic(statisticType As String) As Integer Implements IGameContext.GetAvatarStatistic
        Return World.Avatar.Statistic(statisticType)
    End Function
End Class
