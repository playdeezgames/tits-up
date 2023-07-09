Imports TU.Persistence

Friend Class CharacterTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property Initializer As Action(Of ICharacter)
    Friend ReadOnly Property Verbs As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICharacter))
    Friend ReadOnly Property AvailableVerbs As IEnumerable(Of String)
        Get
            Return Verbs.Keys
        End Get
    End Property
    Friend Sub New(
                  name As String,
                  glyph As Char,
                  hue As Integer,
                  Optional statistics As IReadOnlyDictionary(Of String, Integer) = Nothing,
                  Optional initializer As Action(Of ICharacter) = Nothing,
                  Optional verbs As IReadOnlyDictionary(Of String, Action(Of ICharacter, ICharacter)) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.Verbs = If(verbs, New Dictionary(Of String, Action(Of ICharacter, ICharacter)))
        Me.Initializer = If(initializer, Sub(x)
                                         End Sub)
    End Sub
End Class
