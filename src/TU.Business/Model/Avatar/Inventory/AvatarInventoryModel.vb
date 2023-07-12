Imports TU.Persistence
Public Class AvatarInventoryModel
    Implements IAvatarInventoryModel
    Private ReadOnly avatar As ICharacter
    Private ReadOnly model As IAvatarModel
    Public Sub New(avatar As ICharacter, model As IAvatarModel)
        Me.avatar = avatar
        Me.model = model
    End Sub
    Public ReadOnly Property Exists As Boolean Implements IAvatarInventoryModel.Exists
        Get
            Return avatar.HasItems
        End Get
    End Property
    Public ReadOnly Property Items As IEnumerable(Of (String, String)) Implements IAvatarInventoryModel.Items
        Get
            Return avatar.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property
    Public ReadOnly Property ItemsByName As IEnumerable(Of (String, Integer)) Implements IAvatarInventoryModel.ItemsByName
        Get
            Return avatar.Items.Where(Function(x) x.Name = model.SelectedItemName).Select(Function(x) ($"{x.Name}", x.Id))
        End Get
    End Property

    Public Sub DropSelected() Implements IAvatarInventoryModel.DropSelected
        Dim item = avatar.World.Item(model.SelectedItemId.Value)
        avatar.DropItem(item)
        model.SelectedItemName = Nothing
        model.SelectedItemId = Nothing
    End Sub
End Class
