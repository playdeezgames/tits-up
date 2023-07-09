Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module MapTypes
    Friend Const Initial = "Initial"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MapTypeDescriptor) =
        New Dictionary(Of String, MapTypeDescriptor) From
        {
            {
                Initial,
                New MapTypeDescriptor((9, 9), TerrainTypes.Empty, New Dictionary(Of String, Integer) From
                                      {
                                        {CharacterTypes.Tizzy, 1}
                                      })
            }
        }
    <Extension>
    Friend Function ToMapTypeDescriptor(mapType As String) As MapTypeDescriptor
        Return descriptors(mapType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
