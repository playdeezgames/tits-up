Friend Class GameMessagesContext
    Implements IGameMessagesContext

    Private world As Persistence.IWorld

    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property HasAny As Boolean Implements IGameMessagesContext.HasAny
        Get
            Return world.HasMessages
        End Get
    End Property

    Public ReadOnly Property Current As Persistence.IMessage Implements IGameMessagesContext.Current
        Get
            Return world.CurrentMessage
        End Get
    End Property

    Public Sub Dismiss() Implements IGameMessagesContext.Dismiss
        world.DismissMessage()
    End Sub
End Class
