Imports TU.Persistence
Public Class AvatarInventoryModel
    Implements IAvatarInventoryModel
    Private ReadOnly avatar As ICharacter
    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
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
End Class
