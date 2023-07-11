Friend Class GameAvatarContext
    Implements IGameAvatarContext
    Private ReadOnly world As Persistence.IWorld
    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property MaximumAttack As Integer Implements IGameAvatarContext.MaximumAttack
        Get
            Return world.Avatar.MaximumAttack
        End Get
    End Property

    Public ReadOnly Property MaximumDefend As Integer Implements IGameAvatarContext.MaximumDefend
        Get
            Return world.Avatar.MaximumDefend
        End Get
    End Property

    Public ReadOnly Property Health As Integer Implements IGameAvatarContext.Health
        Get
            Return world.Avatar.Health
        End Get
    End Property

    Public ReadOnly Property MaximumHealth As Integer Implements IGameAvatarContext.MaximumHealth
        Get
            Return world.Avatar.MaximumHealth
        End Get
    End Property

    Public ReadOnly Property HasGroundItems As Boolean Implements IGameAvatarContext.HasGroundItems
        Get
            Return world.Avatar.Cell.HasItems
        End Get
    End Property

    Public ReadOnly Property GroundItems As List(Of (String, String)) Implements IGameAvatarContext.GroundItems
        Get
            Return world.Avatar.Cell.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key)).ToList
        End Get
    End Property

    Public Sub Move(delta As (Integer, Integer)) Implements IGameAvatarContext.Move
        world.Avatar.Move(delta)
    End Sub
End Class
