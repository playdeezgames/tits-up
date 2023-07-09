﻿Friend Class GameAvatarContext
    Implements IGameAvatarContext
    Private ReadOnly world As Persistence.IWorld
    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property Statistic(statisticType As String) As Integer Implements IGameAvatarContext.Statistic
        Get
            Return world.Avatar.Statistic(statisticType)
        End Get
    End Property
    Public Sub Move(delta As (Integer, Integer)) Implements IGameAvatarContext.Move
        world.Avatar.Move(delta)
    End Sub
End Class
