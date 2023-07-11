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

    Public ReadOnly Property GroundItems As IEnumerable(Of (String, String)) Implements IGameAvatarContext.GroundItems
        Get
            Return world.Avatar.Cell.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property

    Public Property SelectedItemName As String Implements IGameAvatarContext.SelectedItemName
        Get
            Return world.Avatar.Metadata(Metadatas.SelectedItemName)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.SelectedItemName) = value
        End Set
    End Property

    Public ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer)) Implements IGameAvatarContext.GroundItemsByName
        Get
            Return world.Avatar.Cell.Items.Where(Function(x) x.Name = SelectedItemName).Select(Function(x) (x.Name, x.Id))
        End Get
    End Property

    Public Property SelectedItemId As Integer? Implements IGameAvatarContext.SelectedItemId
        Get
            If world.Avatar.HasStatistic(StatisticTypes.SelectedItemId) Then
                Return world.Avatar.Statistic(StatisticTypes.SelectedItemId)
            End If
            Return Nothing
        End Get
        Set(value As Integer?)
            If value.HasValue Then
                world.Avatar.Statistic(StatisticTypes.SelectedItemId) = value.Value
                Return
            End If
            world.Avatar.RemoveStatistic(StatisticTypes.SelectedItemId)
        End Set
    End Property

    Public ReadOnly Property Encumbrance As IGameAvatarEncumbranceContext Implements IGameAvatarContext.Encumbrance
        Get
            Return New GameAvatarEncumbranceContext(world.Avatar)
        End Get
    End Property

    Public Sub Move(delta As (Integer, Integer)) Implements IGameAvatarContext.Move
        world.Avatar.Move(delta)
    End Sub

    Public Sub TakeSelectedItem() Implements IGameAvatarContext.TakeSelectedItem
        Dim item = world.Item(SelectedItemId.Value)
        world.Avatar.TakeItem(item)
        SelectedItemName = Nothing
        SelectedItemId = Nothing
    End Sub
End Class
