Public Interface IGameTargetCharacterContext
    ReadOnly Property Verbs As IEnumerable(Of (String, String))
    Sub DoVerb(verbType As String)
    ReadOnly Property Exists As Boolean
    ReadOnly Property Name As String
End Interface
