Imports TU.Persistence

Friend Class GameAvatarEncumbranceContext
    Implements IGameAvatarEncumbranceContext

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property CurrentEncumbrance As Integer Implements IGameAvatarEncumbranceContext.CurrentEncumbrance
        Get
            Return avatar.Encumbrance
        End Get
    End Property

    Public ReadOnly Property MaximumEncumbrance As Integer Implements IGameAvatarEncumbranceContext.MaximumEncumbrance
        Get
            Return avatar.MaximumEncumbrance
        End Get
    End Property

    Public ReadOnly Property IsOverencumbered As Boolean Implements IGameAvatarEncumbranceContext.IsOverencumbered
        Get
            Return avatar.IsOverencumbered
        End Get
    End Property
End Class
