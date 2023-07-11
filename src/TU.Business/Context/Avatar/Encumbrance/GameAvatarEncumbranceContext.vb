Imports TU.Persistence

Friend Class GameAvatarEncumbranceContext
    Implements IGameAvatarEncumbranceContext

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Current As Integer Implements IGameAvatarEncumbranceContext.Current
        Get
            Return avatar.Encumbrance
        End Get
    End Property

    Public ReadOnly Property Maximum As Integer Implements IGameAvatarEncumbranceContext.Maximum
        Get
            Return avatar.MaximumEncumbrance
        End Get
    End Property

    Public ReadOnly Property IsOver As Boolean Implements IGameAvatarEncumbranceContext.IsOver
        Get
            Return avatar.IsOverencumbered
        End Get
    End Property
End Class
