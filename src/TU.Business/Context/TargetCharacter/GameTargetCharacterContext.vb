Friend Class GameTargetCharacterContext
    Implements IGameTargetCharacterContext
    Private ReadOnly world As Persistence.IWorld
    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property Verbs As IEnumerable(Of (String, String)) Implements IGameTargetCharacterContext.Verbs
        Get
            Return world.Avatar.TargetCharacter.Verbs.Select(Function(x) (x.ToVerbTypeDescriptor.Name, x))
        End Get
    End Property
    Public ReadOnly Property Exists As Boolean Implements IGameTargetCharacterContext.Exists
        Get
            Return world.Avatar.TargetCharacter IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property Name As String Implements IGameTargetCharacterContext.Name
        Get
            Return world.Avatar.TargetCharacter.Name
        End Get
    End Property
    Public Sub DoVerb(verbType As String) Implements IGameTargetCharacterContext.DoVerb
        world.Avatar.TargetCharacter.DoVerb(verbType, world.Avatar)
    End Sub
End Class
