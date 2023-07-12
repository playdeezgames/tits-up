Friend Class AvatarModel
    Implements IAvatarModel
    Private ReadOnly world As Persistence.IWorld
    Public Sub New(world As Persistence.IWorld)
        Me.world = world
    End Sub
    Public ReadOnly Property MaximumAttack As Integer Implements IAvatarModel.MaximumAttack
        Get
            Return world.Avatar.MaximumAttack
        End Get
    End Property

    Public ReadOnly Property MaximumDefend As Integer Implements IAvatarModel.MaximumDefend
        Get
            Return world.Avatar.MaximumDefend
        End Get
    End Property

    Public ReadOnly Property Health As Integer Implements IAvatarModel.Health
        Get
            Return world.Avatar.Health
        End Get
    End Property

    Public ReadOnly Property MaximumHealth As Integer Implements IAvatarModel.MaximumHealth
        Get
            Return world.Avatar.MaximumHealth
        End Get
    End Property

    Public ReadOnly Property HasGroundItems As Boolean Implements IAvatarModel.HasGroundItems
        Get
            Return world.Avatar.Cell.HasItems
        End Get
    End Property

    Public ReadOnly Property GroundItems As IEnumerable(Of (String, String)) Implements IAvatarModel.GroundItems
        Get
            Return world.Avatar.Cell.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property

    Public Property SelectedItemName As String Implements IAvatarModel.SelectedItemName
        Get
            Return world.Avatar.Metadata(Metadatas.SelectedItemName)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.SelectedItemName) = value
        End Set
    End Property

    Public ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer)) Implements IAvatarModel.GroundItemsByName
        Get
            Return world.Avatar.Cell.Items.Where(Function(x) x.Name = SelectedItemName).Select(Function(x) (x.Name, x.Id))
        End Get
    End Property

    Public Property SelectedItemId As Integer? Implements IAvatarModel.SelectedItemId
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

    Public ReadOnly Property Encumbrance As IAvatarEncumbranceModel Implements IAvatarModel.Encumbrance
        Get
            Return New AvatarEncumbranceModel(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property IsTitsUp As Boolean Implements IAvatarModel.IsTitsUp
        Get
            Return world.Avatar.IsTitsUp
        End Get
    End Property

    Public ReadOnly Property Inventory As IAvatarInventoryModel Implements IAvatarModel.Inventory
        Get
            Return New AvatarInventoryModel(world.Avatar, Me)
        End Get
    End Property

    Public Property Epitaph As String Implements IAvatarModel.Epitaph
        Get
            Return world.Avatar.Metadata(Metadatas.Epitaph)
        End Get
        Set(value As String)
            world.Avatar.Metadata(Metadatas.Epitaph) = value
        End Set
    End Property

    Public Sub Move(delta As (Integer, Integer)) Implements IAvatarModel.Move
        world.Avatar.Move(delta)
    End Sub

    Public Sub TakeSelectedItem() Implements IAvatarModel.TakeSelectedItem
        Dim item = world.Item(SelectedItemId.Value)
        world.Avatar.TakeItem(item)
        SelectedItemName = Nothing
        SelectedItemId = Nothing
    End Sub
End Class
