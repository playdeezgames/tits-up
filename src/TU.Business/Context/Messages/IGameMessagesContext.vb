Imports TU.Persistence

Public Interface IGameMessagesContext
    ReadOnly Property HasAny As Boolean
    ReadOnly Property Current As IMessage
    Sub Dismiss()
End Interface
