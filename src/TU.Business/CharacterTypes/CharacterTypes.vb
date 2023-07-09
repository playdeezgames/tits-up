Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module CharacterTypes
    Friend Const Tizzy = "Tizzy"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Tizzy,
                New CharacterTypeDescriptor(
                    "Tizzy",
                    ChrW(1),
                    Hue.Brown)
            }
        }
    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
